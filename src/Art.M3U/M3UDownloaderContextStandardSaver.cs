using System.Diagnostics;
using Art.Http;

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
    public override Task RunAsync(CancellationToken cancellationToken = default)
    {
        return OperateAsync(false, null, cancellationToken);
    }

    /// <inheritdoc />
    public override Task ExportAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        return OperateAsync(true, stream, cancellationToken);
    }

    private async Task OperateAsync(bool noFiles, Stream? targetStream = null, CancellationToken cancellationToken = default)
    {
        FailCounter = 0;
        HashSet<string> hs = new();
        Stopwatch sw = new();
        while (true)
        {
            try
            {
                if (HeartbeatCallback != null) await HeartbeatCallback().ConfigureAwait(false);
                Context.Tool.LogInformation("Reading main...");
                M3UFile m3 = await Context.GetAsync(cancellationToken).ConfigureAwait(false);
                if (Context.StreamInfo.EncryptionInfo is { Encrypted: true } ei && m3.EncryptionInfo is { Encrypted: true } ei2 && ei.Method == ei2.Method)
                {
                    ei2.Key ??= ei.Key; // assume key kept if it was supplied in the first place
                    ei2.Iv ??= ei.Iv; // assume IV kept if it was supplied in the first place
                }
                Context.Tool.LogInformation($"{m3.DataLines.Count} segments...");
                int i = 0;
                foreach (string entry in m3.DataLines)
                {
                    long msn = m3.FirstMediaSequenceNumber + i++;
                    var entryUri = new Uri(Context.MainUri, entry);
                    // source could possibly be wonky and use query to differentiate?
                    string entryKey = entry; //entryUri.Segments[^1];
                    if (hs.Contains(entryKey))
                    {
                        continue;
                    }
                    Context.Tool.LogInformation($"Downloading segment {entry}...");
                    if (targetStream != null)
                    {
                        await Context.StreamSegmentAsync(targetStream, noFiles, entryUri, m3, msn, cancellationToken).ConfigureAwait(false);
                    }
                    else
                    {
                        await Context.DownloadSegmentAsync(entryUri, m3, msn, cancellationToken).ConfigureAwait(false);
                    }
                    hs.Add(entryKey);
                }
                if (i != 0)
                {
                    sw.Restart();
                }
                else if (sw.IsRunning && sw.Elapsed > _timeout)
                {
                    Context.Tool.LogInformation($"No new entries for timeout {_timeout}, stopping");
                    return;
                }
                if (_oneOff) break;
                await Task.Delay(1000, cancellationToken).ConfigureAwait(false);
                FailCounter = 0;
            }
            catch (ArtHttpResponseMessageException e)
            {
                await HandleRequestExceptionAsync(e, cancellationToken).ConfigureAwait(false);
                sw.Restart();
            }
        }
    }
}
