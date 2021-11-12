using System.Diagnostics.CodeAnalysis;

namespace Art;

/// <summary>
/// Represents a manager for artifact data.
/// </summary>
public abstract class ArtifactDataManager
{
    /// <summary>
    /// Attempts to get info for the artifact with the specified ID.
    /// </summary>
    /// <param name="id">Artifact ID.</param>
    /// <param name="artifactInfo">Retrieved artifact, if it exists.</param>
    /// <returns>True if artifact located.</returns>
    public abstract bool TryGetInfo(string id, [NotNullWhen(true)] out ArtifactInfo? artifactInfo);

    /// <summary>
    /// Creates an output directory for the specified artifact.
    /// </summary>
    /// <param name="artifactInfo">Artifact target.</param>
    /// <returns>String path to created directory.</returns>
    public abstract string CreateOutputDirectory(ArtifactInfo artifactInfo);

    /// <summary>
    /// Registers artifact as known.
    /// </summary>
    /// <param name="artifactInfo">Artifact to register.</param>
    public abstract void AddInfo(ArtifactInfo artifactInfo);

    /// <summary>
    /// Tests if artifact is recognizably new.
    /// </summary>
    /// <param name="artifactInfo">Artifact to check.</param>
    /// <returns>True if this is a new artifact (newer than whatever exists with the same ID).</returns>
    public bool IsNewArtifact(ArtifactInfo artifactInfo)
    {
        return !TryGetInfo(artifactInfo.Id, out var oldArtifact) || oldArtifact.UpdateDate == null || artifactInfo.UpdateDate != null && oldArtifact.UpdateDate.Value < artifactInfo.UpdateDate.Value;
    }
}
