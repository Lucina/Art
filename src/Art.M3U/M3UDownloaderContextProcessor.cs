using System.Diagnostics;
using System.Net;
using Art.Http;

namespace Art.M3U;

/// <summary>
/// Base class for <see cref="M3UDownloaderContext"/>-bound savers.
/// </summary>
public abstract class M3UDownloaderContextProcessor
{
    private static readonly TimeSpan s_playlistDelay = TimeSpan.FromSeconds(1);
    private static readonly Guid s_operationWaitingForResult = Guid.ParseExact("4fd5c851a88c430c8f8da54dbcf70ab2", "N");

    /// <summary>
    /// Heartbeat callback to use before an iteration.
    /// </summary>
    public Func<Task>? HeartbeatCallback { get; set; }

    /// <summary>
    /// Recovery callback for errors.
    /// </summary>
    public Func<Exception, Task>? RecoveryCallback { get; set; }

    /// <summary>
    /// Timeout for HTTP error 500 (Internal Server Error).
    /// </summary>
    /// <remarks>
    /// Default value: 10 seconds.
    /// </remarks>
    public TimeSpan? Timeout500 { get; set; } = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Timeout for HTTP error 503 (Service Unavailable).
    /// </summary>
    /// <remarks>
    /// Default value: 10 seconds.
    /// </remarks>
    public TimeSpan? Timeout503 { get; set; } = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Parent context.
    /// </summary>
    protected readonly M3UDownloaderContext Context;

    /// <summary>
    /// Failure counter.
    /// </summary>
    protected volatile int FailCounter;

    /// <summary>
    /// Initializes a new instance of <see cref="M3UDownloaderContextProcessor"/>.
    /// </summary>
    /// <param name="context">Parent context.</param>
    protected M3UDownloaderContextProcessor(M3UDownloaderContext context)
    {
        Context = context;
    }

    /// <summary>
    /// Handles HTTP request exception.
    /// </summary>
    /// <param name="exception">Original exception.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="AggregateException">Thrown when handling exception failed.</exception>
    protected virtual async Task HandleRequestExceptionAsync(ArtHttpResponseMessageException exception, CancellationToken cancellationToken)
    {
        Interlocked.Increment(ref FailCounter);
        Context.Tool.LogInformation("HTTP error encountered", exception.ToString());
        switch (exception.StatusCode)
        {
            case HttpStatusCode.Forbidden: // 403
                ThrowForExceedFails(exception);
                await GetRecoveryCallbackOrThrow(exception)(exception).ConfigureAwait(false);
                return;
            case HttpStatusCode.InternalServerError: // 500
                ThrowForExceedFails(exception);
                await DelayOrThrowAsync(exception, Timeout500, null, cancellationToken).ConfigureAwait(false);
                return;
            case HttpStatusCode.ServiceUnavailable: // 503
                ThrowForExceedFails(exception);
                await DelayOrThrowAsync(exception, Timeout503, null, cancellationToken).ConfigureAwait(false);
                return;
        }
        await GetRecoveryCallbackOrThrow(exception)(exception).ConfigureAwait(false);
    }

    private void ThrowForExceedFails(Exception exception)
    {
        if (FailCounter > Context.Config.MaxFails) throw new AggregateException($"Failed {FailCounter} times in a row (exceeded threshold", exception);
    }

    private Func<Exception, Task> GetRecoveryCallbackOrThrow(Exception exception)
    {
        if (RecoveryCallback == null) throw new AggregateException("No recovery callback implemented", exception);
        return RecoveryCallback;
    }

    private static async Task DelayOrThrowAsync(ArtHttpResponseMessageException exception, TimeSpan? delay, ArtHttpResponseMessageException? responseMessageException, CancellationToken cancellationToken)
    {
        TimeSpan? delayMake = delay ?? responseMessageException?.RetryCondition?.Delta;
        if (delayMake is not { } delayActual) throw new AggregateException($"No retry delay specified for HTTP response {exception.StatusCode?.ToString() ?? "<unknown>"} and no default value provided", exception);
        await Task.Delay(delayActual, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Processes a playlist.
    /// </summary>
    /// <param name="oneOff">If true, complete after one pass through playlist.</param>
    /// <param name="timeout">Timeout to use to determine when a stream seems to have ended.</param>
    /// <param name="playlistElementProcessor">Processor to handle playlist elements.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    protected async Task ProcessPlaylistAsync(bool oneOff, TimeSpan timeout, IPlaylistElementProcessor playlistElementProcessor, CancellationToken cancellationToken = default)
    {
        IOperationProgressContext? operationProgressContext = null;
        try
        {
            FailCounter = 0;
            HashSet<string> hs = new();
            Stopwatch sw = new();
            sw.Start();
            TimeSpan remainingTimeout = timeout;
            while (true)
            {
                try
                {
                    if (HeartbeatCallback != null) await HeartbeatCallback().ConfigureAwait(false);
                    if (operationProgressContext == null)
                    {
                        if (Context.Tool.LogHandler?.TryGetOperationProgressContext("Waiting for new segments", s_operationWaitingForResult, out var op) ?? false)
                        {
                            operationProgressContext = op;
                        }
                        else
                        {
                            operationProgressContext = null;
                        }
                    }
                    if (operationProgressContext != null)
                    {
                        operationProgressContext.Report(Math.Clamp(1.0f - (float)remainingTimeout.Divide(timeout), 0.0f, 1.0f));
                    }
                    else
                    {
                        Context.Tool.LogInformation($"Waiting up to {remainingTimeout.TotalSeconds:F3}s for new segments...");
                    }
                    M3UFile m3 = await Context.GetAsync(cancellationToken).ConfigureAwait(false);
                    if (Context.StreamInfo.EncryptionInfo is { Encrypted: true } ei && m3.EncryptionInfo is { Encrypted: true } ei2 && ei.Method == ei2.Method)
                    {
                        ei2.Key ??= ei.Key; // assume key kept if it was supplied in the first place
                        ei2.Iv ??= ei.Iv; // assume IV kept if it was supplied in the first place
                    }
                    //Context.Tool.LogInformation($"{m3.DataLines.Count} segments...");
                    int i = 0, j = 0;
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
                        if (operationProgressContext != null)
                        {
                            operationProgressContext.Dispose();
                            operationProgressContext = null;
                        }
                        await playlistElementProcessor.ProcessPlaylistElementAsync(entryUri, m3, msn, entry, new ItemNo(i, m3.DataLines.Count), cancellationToken).ConfigureAwait(false);
                        hs.Add(entryKey);
                        j++;
                    }
                    if (j != 0)
                    {
                        sw.Restart();
                    }
                    else if (sw.IsRunning)
                    {
                        var elapsed = sw.Elapsed;
                        if (elapsed >= timeout)
                        {
                            if (operationProgressContext != null)
                            {
                                operationProgressContext.Dispose();
                                operationProgressContext = null;
                            }
                            Context.Tool.LogInformation($"No new entries for timeout {timeout}, stopping");
                            return;
                        }
                        remainingTimeout = timeout.Subtract(elapsed);
                    }
                    else
                    {
                        if (operationProgressContext != null)
                        {
                            operationProgressContext.Dispose();
                            operationProgressContext = null;
                        }
                        Context.Tool.LogError("Timer stopped running (error?)");
                        return;
                    }
                    if (oneOff) break;
                    await Task.Delay(s_playlistDelay, cancellationToken).ConfigureAwait(false);
                    FailCounter = 0;
                }
                catch (ArtHttpResponseMessageException e)
                {
                    if (operationProgressContext != null)
                    {
                        operationProgressContext.Dispose();
                        operationProgressContext = null;
                    }
                    await HandleRequestExceptionAsync(e, cancellationToken).ConfigureAwait(false);
                    sw.Restart();
                }
            }
        }
        finally
        {
            if (operationProgressContext != null)
            {
                operationProgressContext.Dispose();
            }
        }
    }
}
