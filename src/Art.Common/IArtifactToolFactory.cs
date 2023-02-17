namespace Art.Common;

/// <summary>
/// Represents a factory for artifact tools.
/// </summary>
public interface IArtifactToolFactory
{
    /// <summary>
    /// Creates an artifact tool.
    /// </summary>
    /// <returns>Artifact tool.</returns>
    static abstract IArtifactTool CreateArtifactTool();

    /// <summary>
    /// Gets artifact tool ID for this factory.
    /// </summary>
    /// <returns>Artifact tool ID.</returns>
    static abstract ArtifactToolID GetArtifactToolId();
}
