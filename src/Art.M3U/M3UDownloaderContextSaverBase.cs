using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Art.M3U;

/// <summary>
/// Base class for <see cref="M3UDownloaderContext"/>-bound savers.
/// </summary>
public abstract class M3UDownloaderContextSaverBase : ISaver
{
    /// <inheritdoc />
    public Func<Task>? HeartbeatCallback { get; set; }

    /// <inheritdoc />
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
    /// Initializes a new instance of <see cref="M3UDownloaderContextSaverBase"/>.
    /// </summary>
    /// <param name="context">Parent context.</param>
    protected M3UDownloaderContextSaverBase(M3UDownloaderContext context)
    {
        Context = context;
    }

    /// <inheritdoc />
    public abstract Task RunAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Attempts to retrieve a <see cref="HttpRequestException"/> from the specified <see cref="AggregateException"/>.
    /// </summary>
    /// <param name="exception">Exception to extract from.</param>
    /// <param name="requestException">Request exception.</param>
    /// <param name="responseMessageException">Response message exception, if available.</param>
    /// <returns>True if <paramref name="requestException"/> was retrieved.</returns>
    protected static bool TryGetHttpRequestException(AggregateException exception, [NotNullWhen(true)] out HttpRequestException? requestException, out ExHttpResponseMessageException? responseMessageException)
    {
        exception = exception.Flatten();
        requestException = null;
        responseMessageException = null;
        foreach (Exception e in exception.InnerExceptions)
        {
            requestException ??= e as HttpRequestException;
            responseMessageException ??= e as ExHttpResponseMessageException;
        }
        return requestException != null;
    }

    /// <summary>
    /// Handles HTTP request exception.
    /// </summary>
    /// <param name="originalExceptions">Original exceptions.</param>
    /// <param name="requestException">Exception.</param>
    /// <param name="responseMessageException">Response message exception, if available.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="AggregateException">Thrown when handling exception failed.</exception>
    protected virtual async Task HandleHttpRequestExceptionAsync(IReadOnlyCollection<Exception> originalExceptions, HttpRequestException requestException, ExHttpResponseMessageException? responseMessageException, CancellationToken cancellationToken)
    {
        Context.Tool.LogInformation("HTTP error encountered", requestException.ToString());
        Interlocked.Increment(ref FailCounter);
        switch (requestException.StatusCode)
        {
            case HttpStatusCode.Forbidden: // 403
                ThrowForExceedFails(originalExceptions);
                await GetRecoveryCallbackOrThrow(originalExceptions)(requestException);
                break;
            case HttpStatusCode.InternalServerError: // 500
                ThrowForExceedFails(originalExceptions);
                await DelayOrThrowAsync(originalExceptions, Timeout500, requestException, responseMessageException, cancellationToken);
                break;
            case HttpStatusCode.ServiceUnavailable: // 503
                ThrowForExceedFails(originalExceptions);
                await DelayOrThrowAsync(originalExceptions, Timeout503, requestException, responseMessageException, cancellationToken);
                break;
        }
        throw new AggregateException("Unhandled HTTP error", originalExceptions);
    }

    /// <summary>
    /// Handles HTTP request exception.
    /// </summary>
    /// <param name="requestException">Original exception.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="AggregateException">Thrown when handling exception failed.</exception>
    protected virtual async Task HandleHttpRequestExceptionAsync(HttpRequestException requestException, CancellationToken cancellationToken)
    {
        Context.Tool.LogInformation("HTTP error encountered", requestException.ToString());
        Interlocked.Increment(ref FailCounter);
        switch (requestException.StatusCode)
        {
            case HttpStatusCode.Forbidden: // 403
                ThrowForExceedFails(requestException);
                await GetRecoveryCallbackOrThrow(requestException)(requestException);
                break;
            case HttpStatusCode.InternalServerError: // 500
                ThrowForExceedFails(requestException);
                await DelayOrThrowAsync(requestException, Timeout500, requestException, null, cancellationToken);
                break;
            case HttpStatusCode.ServiceUnavailable: // 503
                ThrowForExceedFails(requestException);
                await DelayOrThrowAsync(requestException, Timeout503, requestException, null, cancellationToken);
                break;
        }
        throw new AggregateException("Unhandled HTTP error", requestException);
    }

    private void ThrowForExceedFails(IReadOnlyCollection<Exception> originalExceptions)
    {
        if (FailCounter > Context.Config.MaxFails) throw new AggregateException($"Failed {FailCounter} times in a row (exceeded threshold), aborting", originalExceptions);
    }

    private void ThrowForExceedFails(Exception exception)
    {
        if (FailCounter > Context.Config.MaxFails) throw new AggregateException($"Failed {FailCounter} times in a row (exceeded threshold), aborting", exception);
    }

    private Func<Exception, Task> GetRecoveryCallbackOrThrow(IReadOnlyCollection<Exception> originalExceptions)
    {
        if (RecoveryCallback == null) throw new AggregateException("No recovery callback implemented, aborting", originalExceptions);
        return RecoveryCallback;
    }

    private Func<Exception, Task> GetRecoveryCallbackOrThrow(Exception exception)
    {
        if (RecoveryCallback == null) throw new AggregateException("No recovery callback implemented, aborting", exception);
        return RecoveryCallback;
    }

    private static async Task DelayOrThrowAsync(IReadOnlyCollection<Exception> originalExceptions, TimeSpan? delay, HttpRequestException hre, ExHttpResponseMessageException? responseMessageException, CancellationToken cancellationToken)
    {
        TimeSpan? delayMake = responseMessageException?.RetryCondition?.Delta ?? delay;
        if (delayMake is not { } delayActual) throw new AggregateException($"No retry delay specified for HTTP response {hre.StatusCode?.ToString() ?? "<unknown>"} and no default value provided", originalExceptions);
        await Task.Delay(delayActual, cancellationToken);
    }

    private static async Task DelayOrThrowAsync(Exception exception, TimeSpan? delay, HttpRequestException hre, ExHttpResponseMessageException? responseMessageException, CancellationToken cancellationToken)
    {
        TimeSpan? delayMake = delay ?? responseMessageException?.RetryCondition?.Delta;
        if (delayMake is not { } delayActual) throw new AggregateException($"No retry delay specified for HTTP response {hre.StatusCode?.ToString() ?? "<unknown>"} and no default value provided", exception);
        await Task.Delay(delayActual, cancellationToken);
    }
}
