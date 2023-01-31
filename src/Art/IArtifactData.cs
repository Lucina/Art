namespace Art;

/// <summary>
/// Stores data relevant to an artifact.
/// </summary>
public interface IArtifactData : IReadOnlyDictionary<ArtifactResourceKey, ArtifactResourceInfo>
{
    /// <summary>
    /// Info for this artifact.
    /// </summary>
    ArtifactInfo Info { get; }
}
