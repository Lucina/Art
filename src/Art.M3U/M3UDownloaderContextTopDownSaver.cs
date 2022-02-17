using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;

namespace Art.M3U;

/// <summary>
/// Represents a top-down saver.
/// </summary>
public class M3UDownloaderContextTopDownSaver : ISaver
{
    private static readonly Regex s_bitRegex = new(@"(^[\S\s]*[^\d]|)\d+(\.\w+)$");

    /// <inheritdoc />
    public Func<Task>? HeartbeatCallback { get; set; }

    /// <inheritdoc />
    public Func<HttpRequestException, Task>? RecoveryCallback { get; set; }

    private readonly M3UDownloaderContext _context;
    private readonly long _top;
    private readonly Func<string, long, string> _nameTransform;

    internal M3UDownloaderContextTopDownSaver(M3UDownloaderContext context, long top)
        : this(context, top, TranslateNameDefault)
    {
    }

    internal M3UDownloaderContextTopDownSaver(M3UDownloaderContext context, long top, Func<long, string> idFormatter)
        : this(context, top, (a, b) => TranslateNameDefault(a, idFormatter(b)))
    {
    }

    internal M3UDownloaderContextTopDownSaver(M3UDownloaderContext context, long top, Func<string, long, string> nameTransform)
    {
        _context = context;
        _top = top;
        _nameTransform = nameTransform;
    }

    /// <summary>
    /// Translates a generic filename.
    /// </summary>
    /// <param name="name">Donor filename.</param>
    /// <param name="i">Index.</param>
    /// <returns>Generated filename.</returns>
    /// <exception cref="InvalidDataException">Regex match failed.</exception>
    public static string TranslateNameDefault(string name, long i) => TranslateNameDefault(name, i.ToString(CultureInfo.InvariantCulture));

    /// <summary>
    /// Translates a generic filename.
    /// </summary>
    /// <param name="name">Donor filename.</param>
    /// <param name="i">Index.</param>
    /// <returns>Generated filename.</returns>
    /// <exception cref="InvalidDataException">Regex match failed.</exception>
    public static string TranslateNameDefault(string name, string i)
    {
        if (s_bitRegex.Match(name) is not { Success: true } bits) throw new InvalidDataException();
        return $"{bits.Groups[1].Value}{i}{bits.Groups[2].Value}";
    }

    /// <inheritdoc />
    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        int failCtr = 0;
        long top = _top;
        while (true)
        {
            try
            {
                if (top < 0) break;
                if (HeartbeatCallback != null) await HeartbeatCallback();
                _context.Tool.LogInformation("Reading main...");
                M3UFile m3 = await _context.GetAsync(cancellationToken);
                string str = m3.DataLines.First();
                Uri origUri = new(_context.MainUri, str);
                Uri uri = new UriBuilder(new Uri(_context.MainUri, _nameTransform(str, top))) { Query = origUri.Query }.Uri;
                _context.Tool.LogInformation($"Downloading segment {uri.Segments[^1]}...");
                try
                {
                    // Don't assume MSN, and just accept failure (exception) when trying to decrypt with no IV
                    // Also don't depend on current file since it probably won't do us good anyway for this use case
                    await _context.DownloadSegmentAsync(uri, null, null, cancellationToken);
                    top--;
                }
                catch (HttpRequestException hre)
                {
                    if (hre.StatusCode == HttpStatusCode.NotFound)
                    {
                        _context.Tool.LogInformation("HTTP NotFound returned, ending operation");
                        return;
                    }
                }
                await Task.Delay(500, cancellationToken);
                failCtr = 0;
            }
            catch (HttpRequestException hre)
            {
                _context.Tool.LogInformation("HTTP error encountered", hre.ToString());
                if (hre.StatusCode == HttpStatusCode.Forbidden)
                {
                    failCtr++;
                    if (failCtr > _context.Config.MaxFails) throw new AggregateException($"Failed {failCtr} times in a row (exceeded threshold), aborting", hre);
                    if (RecoveryCallback == null) throw new AggregateException($"Failed {failCtr} times in a row and no recovery callback implemented, aborting", hre);
                    await RecoveryCallback(hre);
                }
            }
        }
    }
}
