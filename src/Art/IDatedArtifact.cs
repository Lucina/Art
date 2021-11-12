namespace Art;

/// <summary>
/// Represents an artifact that has associated dates.
/// </summary>
public interface IDatedArtifact : IArtifact
{
    /// <summary>
    /// Artifact publish date.
    /// </summary>
    DateTimeOffset ArtifactDate { get; }

    /// <summary>
    /// Artifact update date.
    /// </summary>
    DateTimeOffset ArtifactUpdateDate { get; }
}
