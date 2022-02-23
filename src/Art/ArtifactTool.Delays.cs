namespace Art;

public partial class ArtifactTool
{
    #region Delays

    /// <summary>
    /// Delays this operation for the specified amount of time.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public Task DelayAsync(CancellationToken cancellationToken = default)
        => DelayAsync(DelaySeconds, cancellationToken);

    /// <summary>
    /// Delays this operation for <see cref="DelaySeconds"/> seconds.
    /// </summary>
    /// <param name="delaySeconds">Delay in seconds.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public static Task DelayAsync(double delaySeconds, CancellationToken cancellationToken = default)
        => Task.Delay(TimeSpan.FromSeconds(delaySeconds), cancellationToken);

    /// <summary>
    /// Delays this operation for <see cref="DelaySeconds"/> seconds, after the first call to this method.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public Task DelayAfterFirstAsync(CancellationToken cancellationToken = default)
        => DelayAfterFirstAsync(DelaySeconds, cancellationToken);

    /// <summary>
    /// Delays this operation for the specified amount of time, after the first call to this method.
    /// </summary>
    /// <param name="delaySeconds">Delay in seconds.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public Task DelayAfterFirstAsync(double delaySeconds, CancellationToken cancellationToken = default)
    {
        if (_delayFirstCalled)
            return Task.Delay(TimeSpan.FromSeconds(delaySeconds), cancellationToken);
        _delayFirstCalled = true;
        return Task.CompletedTask;
    }

    #endregion
}
