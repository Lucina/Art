namespace Art;

/// <summary>
/// Represents a manager for artifact data.
/// </summary>
public abstract class ArtifactRegistrationManager
{
    /// <summary>
    /// Registers artifact as known.
    /// </summary>
    /// <param name="artifactInfo">Artifact to register.</param>
    /// <returns>Task.</returns>
    public abstract ValueTask AddArtifactAsync(ArtifactInfo artifactInfo);

    /// <summary>
    /// Registers artifact resource as known.
    /// </summary>
    /// <param name="artifactResourceInfo">Artifact resource to register.</param>
    /// <returns>Task.</returns>
    public abstract ValueTask AddResourceAsync(ArtifactResourceInfo artifactResourceInfo);

    /// <summary>
    /// Attempts to get info for the artifact with the specified key.
    /// </summary>
    /// <param name="key">Artifact key.</param>
    /// <returns>Task returning retrieved artifact, if it exists.</returns>
    public abstract ValueTask<ArtifactInfo?> TryGetArtifactAsync(ArtifactKey key);

    /// <summary>
    /// Attempts to get info for the resource with the specified key.
    /// </summary>
    /// <param name="key">Resource key.</param>
    /// <returns>Task returning retrieved resource, if it exists.</returns>
    public abstract ValueTask<ArtifactResourceInfo?> TryGetResourceAsync(ArtifactResourceKey key);

    /// <summary>
    /// Tests if artifact is recognizably new.
    /// </summary>
    /// <param name="artifactInfo">Artifact to check.</param>
    /// <returns>True if this is a new artifact (newer than whatever exists with the same ID).</returns>
    public virtual async ValueTask<bool> IsNewArtifactAsync(ArtifactInfo artifactInfo)
    {
        return (await TryGetArtifactAsync(artifactInfo.Key).ConfigureAwait(false)) is not { } oldArtifact
            || oldArtifact.UpdateDate == null
            || artifactInfo.UpdateDate != null && oldArtifact.UpdateDate.Value < artifactInfo.UpdateDate.Value;
    }
}
