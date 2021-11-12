namespace Art;

/// <summary>
/// Represents an artifact.
/// </summary>
public interface IArtifact
{
    /// <summary>
    /// True if this artifact is a valid, saveable artifact.
    /// </summary>
    bool ArtifactValid { get; }

    /// <summary>
    /// Artifact ID.
    /// </summary>
    string ArtifactId { get; }
}
