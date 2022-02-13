using System.Net;

namespace Art.M3U;

/// <summary>
/// Represents a top-down saver.
/// </summary>
public class M3UDownloaderContextTopDownSaver : ISaver
{
    /// <inheritdoc />
    public Func<Task>? HeartbeatCallback { get; set; }

    /// <inheritdoc />
    public Func<HttpRequestException, Task>? RecoveryCallback { get; set; }

    private readonly M3UDownloaderContext _context;
    private readonly long _top;
    private readonly Func<string, long, string> _nameTransform;

    internal M3UDownloaderContextTopDownSaver(M3UDownloaderContext context, long top, Func<string, long, string> nameTransform)
    {
        _context = context;
        _top = top;
        _nameTransform = nameTransform;
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
                    await _context.DownloadSegmentAsync(uri, cancellationToken);
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
