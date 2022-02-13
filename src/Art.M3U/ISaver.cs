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
    Func<HttpRequestException, Task>? RecoveryCallback { get; set; }

    /// <summary>
    /// Runs implementation.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    Task RunAsync(CancellationToken cancellationToken = default);
}
