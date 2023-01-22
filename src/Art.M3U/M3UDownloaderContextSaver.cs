using System.Net;
using Art.Http;

namespace Art.M3U;

/// <summary>
/// Base class for <see cref="M3UDownloaderContext"/>-bound savers.
/// </summary>
public abstract class M3UDownloaderContextSaver
{
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
    /// Initializes a new instance of <see cref="M3UDownloaderContextSaver"/>.
    /// </summary>
    /// <param name="context">Parent context.</param>
    protected M3UDownloaderContextSaver(M3UDownloaderContext context)
    {
        Context = context;
    }

    /// <summary>
    /// Runs implementation.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    /// <exception cref="ArtHttpResponseMessageException">Thrown on HTTP response indicating non-successful response.</exception>
    public abstract Task RunAsync(CancellationToken cancellationToken = default);

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
                await GetRecoveryCallbackOrThrow(exception)(exception);
                return;
            case HttpStatusCode.InternalServerError: // 500
                ThrowForExceedFails(exception);
                await DelayOrThrowAsync(exception, Timeout500, null, cancellationToken);
                return;
            case HttpStatusCode.ServiceUnavailable: // 503
                ThrowForExceedFails(exception);
                await DelayOrThrowAsync(exception, Timeout503, null, cancellationToken);
                return;
        }
        await GetRecoveryCallbackOrThrow(exception)(exception);
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
        await Task.Delay(delayActual, cancellationToken);
    }
}
