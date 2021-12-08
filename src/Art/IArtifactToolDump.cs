namespace Art;

/// <summary>
/// Represents an artifact tool that dumps.
/// </summary>
public interface IArtifactToolDump
{
    /// <summary>
    /// Dumps artifacts.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    Task DumpAsync(CancellationToken cancellationToken = default);
}
