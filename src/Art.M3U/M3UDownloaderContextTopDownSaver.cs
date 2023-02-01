using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using Art.Http;

namespace Art.M3U;

/// <summary>
/// Represents a top-down saver.
/// </summary>
public partial class M3UDownloaderContextTopDownSaver : M3UDownloaderContextSaver
{
    [GeneratedRegex("(^[\\S\\s]*[^\\d]|)\\d+(\\.\\w+)$")]
    private static partial Regex GetBitRegex();

    [GeneratedRegex("(?<prefix>^[\\S\\s]*[^\\d]|)(?<number>\\d+)(?<suffix>\\.\\w+)$")]
    private static partial Regex GetBit2Regex();

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

    internal M3UDownloaderContextTopDownSaver(M3UDownloaderContext context, long top, Func<string, long, string> nameTransform) : base(context)
    {
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
        if (GetBitRegex().Match(name) is not { Success: true } bits)
        {
            throw new InvalidDataException();
        }
        return $"{bits.Groups[1].Value}{i}{bits.Groups[2].Value}";
    }

    /// <summary>
    /// Translates a generic filename, with padding to match the string length of the numeric part.
    /// </summary>
    /// <param name="name">Donor filename.</param>
    /// <param name="i">Index.</param>
    /// <returns>Generated filename.</returns>
    /// <exception cref="InvalidDataException">Regex match failed.</exception>
    public static string TranslateNameMatchLength(string name, string i)
    {
        if (GetBit2Regex().Match(name) is not { Success: true } match)
        {
            throw new InvalidDataException();
        }
        int nameLength = match.Groups["number"].Length;
        string paddedI = i.PadLeft(nameLength, '0');
        return match.Groups["prefix"].Value + paddedI + match.Groups["suffix"].Value;
    }

    /// <inheritdoc />
    public override async Task RunAsync(CancellationToken cancellationToken = default)
    {
        FailCounter = 0;
        long top = _top;
        while (true)
        {
            try
            {
                if (top < 0) break;
                if (HeartbeatCallback != null) await HeartbeatCallback().ConfigureAwait(false);
                Context.Tool.LogInformation("Reading main...");
                M3UFile m3 = await Context.GetAsync(cancellationToken).ConfigureAwait(false);
                string str = m3.DataLines.First();
                Uri origUri = new(Context.MainUri, str);
                int idx = str.IndexOf('?');
                if (idx >= 0) str = str[..idx];
                Uri uri = new UriBuilder(new Uri(Context.MainUri, _nameTransform(str, top))) { Query = origUri.Query }.Uri;
                Context.Tool.LogInformation($"Downloading segment {uri.Segments[^1]}...");
                try
                {
                    // Don't assume MSN, and just accept failure (exception) when trying to decrypt with no IV
                    // Also don't depend on current file since it probably won't do us good anyway for this use case
                    await Context.DownloadSegmentAsync(uri, null, null, cancellationToken).ConfigureAwait(false);
                    top--;
                }
                catch (ArtHttpResponseMessageException e)
                {
                    if (e.StatusCode == HttpStatusCode.NotFound)
                    {
                        Context.Tool.LogInformation("HTTP NotFound returned, ending operation");
                        return;
                    }
                    await HandleRequestExceptionAsync(e, cancellationToken).ConfigureAwait(false);
                }
                await Task.Delay(500, cancellationToken).ConfigureAwait(false);
                FailCounter = 0;
            }
            catch (ArtHttpResponseMessageException e)
            {
                await HandleRequestExceptionAsync(e, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
