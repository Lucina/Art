using System.Diagnostics;

namespace Art.M3U;

/// <summary>
/// Represents a basic saver.
/// </summary>
public class M3UDownloaderContextStandardSaver : M3UDownloaderContextSaver
{
    private readonly bool _oneOff;
    private readonly TimeSpan _timeout;

    internal M3UDownloaderContextStandardSaver(M3UDownloaderContext context, bool oneOff, TimeSpan timeout) : base(context)
    {
        _oneOff = oneOff;
        _timeout = timeout;
    }

    /// <inheritdoc />
    public override async Task RunAsync(CancellationToken cancellationToken = default)
    {
        FailCounter = 0;
        HashSet<string> hs = new();
        Stopwatch sw = new();
        while (true)
        {
            try
            {
                if (HeartbeatCallback != null) await HeartbeatCallback();
                Context.Tool.LogInformation("Reading main...");
                HashSet<string> entries = new();
                M3UFile m3 = await Context.GetAsync(cancellationToken);
                entries.UnionWith(m3.DataLines);
                entries.ExceptWith(hs);
                Context.Tool.LogInformation($"{entries.Count} new segments...");
                if (entries.Count != 0)
                {
                    sw.Restart();
                }
                else if (sw.IsRunning && sw.Elapsed > _timeout)
                {
                    Context.Tool.LogInformation($"No new entries for timeout {_timeout}, stopping");
                    return;
                }
                int i = 0;
                foreach (string entry in entries)
                {
                    Context.Tool.LogInformation($"Downloading segment {entry}...");
                    await Context.DownloadSegmentAsync(new Uri(Context.MainUri, entry), m3, m3.FirstMediaSequenceNumber + i, cancellationToken);
                    i++;
                }
                hs.UnionWith(entries);
                if (_oneOff) break;
                await Task.Delay(1000, cancellationToken);
                FailCounter = 0;
            }
            catch (HttpRequestException requestException)
            {
                await HandleHttpRequestExceptionAsync(requestException, cancellationToken);
            }
            catch (AggregateException aggregateException)
            {
                if (TryGetHttpRequestException(aggregateException, out HttpRequestException? requestException, out ExHttpResponseMessageException? responseMessageException))
                    await HandleHttpRequestExceptionAsync(aggregateException.InnerExceptions, requestException, responseMessageException, cancellationToken);
                throw;
            }
        }
    }
}
