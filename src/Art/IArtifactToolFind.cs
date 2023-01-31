namespace Art;

/// <summary>
/// Represents an artifact tool that finds.
/// </summary>
public interface IArtifactToolFind : IArtifactTool
{
    /// <summary>
    /// Finds an artifact with the specified id.
    /// </summary>
    /// <param name="id">Artifact id.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning found artifact or null.</returns>
    Task<IArtifactData?> FindAsync(string id, CancellationToken cancellationToken = default);
}
