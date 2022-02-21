namespace Art.M3U;

/// <summary>
/// Represents a general runnable stream saver.
/// </summary>
public interface ISaver
{
    /// <summary>
    /// Heartbeat callback to use before an iteration.
    /// </summary>
    Func<Task>? HeartbeatCallback { get; set; }

    /// <summary>
    /// Recovery callback for errors.
    /// </summary>
    Func<Exception, Task>? RecoveryCallback { get; set; }

    /// <summary>
    /// Runs implementation.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    /// <exception cref="AggregateException">Thrown with <see cref="HttpRequestException"/> and <see cref="ExHttpResponseMessageException"/> on HTTP error.</exception>
    Task RunAsync(CancellationToken cancellationToken = default);
}
