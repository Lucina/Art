using System.Diagnostics;
using System.Net;

namespace Art.M3U;

/// <summary>
/// Represents a basic saver.
/// </summary>
public class M3UDownloaderContextSaver : ISaver
{
    /// <inheritdoc />
    public Func<Task>? HeartbeatCallback { get; set; }

    /// <inheritdoc />
    public Func<HttpRequestException, Task>? RecoveryCallback { get; set; }

    private readonly M3UDownloaderContext _context;
    private readonly bool _oneOff;
    private readonly TimeSpan _timeout;

    internal M3UDownloaderContextSaver(M3UDownloaderContext context, bool oneOff, TimeSpan timeout)
    {
        _context = context;
        _oneOff = oneOff;
        _timeout = timeout;
    }

    /// <inheritdoc />
    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        int failCtr = 0;
        HashSet<string> hs = new();
        Stopwatch sw = new();
        while (true)
        {
            try
            {
                if (HeartbeatCallback != null) await HeartbeatCallback();
                _context.Tool.LogInformation("Reading main...");
                HashSet<string> entries = new();
                M3UFile m3 = await _context.GetAsync(cancellationToken);
                entries.UnionWith(m3.DataLines);
                entries.ExceptWith(hs);
                _context.Tool.LogInformation($"{entries.Count} new segments...");
                if (entries.Count != 0)
                {
                    sw.Restart();
                }
                else if (sw.IsRunning && sw.Elapsed > _timeout)
                {
                    _context.Tool.LogInformation($"No new entries for timeout {_timeout}, stopping");
                    return;
                }
                int i = 0;
                foreach (string entry in entries)
                {
                    _context.Tool.LogInformation($"Downloading segment {entry}...");
                    await _context.DownloadSegmentAsync(new Uri(_context.MainUri, entry), m3, m3.FirstMediaSequenceNumber + i, cancellationToken);
                    i++;
                }
                hs.UnionWith(entries);
                if (_oneOff) break;
                await Task.Delay(1000, cancellationToken);
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
