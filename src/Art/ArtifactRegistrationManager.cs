namespace Art;

/// <summary>
/// Represents a manager for artifact data.
/// </summary>
public abstract class ArtifactRegistrationManager
{
    /// <summary>
    /// Lists all artifacts.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Artifact.</returns>
    public abstract IAsyncEnumerable<ArtifactInfo> ListArtifactsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists all artifacts for the specified tool.
    /// </summary>
    /// <param name="tool">Tool.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Artifact.</returns>
    public abstract IAsyncEnumerable<ArtifactInfo> ListArtifactsAsync(string tool, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists all artifacts for the specified tool and group.
    /// </summary>
    /// <param name="tool">Tool.</param>
    /// <param name="group">Group.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Artifact.</returns>
    public abstract IAsyncEnumerable<ArtifactInfo> ListArtifactsAsync(string tool, string group, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists all resources for the specified artifact key.
    /// </summary>
    /// <param name="key">Artifact key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Resources.</returns>
    public abstract IAsyncEnumerable<ArtifactResourceInfo> ListResourcesAsync(ArtifactKey key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers artifact as known.
    /// </summary>
    /// <param name="artifactInfo">Artifact to register.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public abstract ValueTask AddArtifactAsync(ArtifactInfo artifactInfo, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers artifact resource as known.
    /// </summary>
    /// <param name="artifactResourceInfo">Artifact resource to register.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public abstract ValueTask AddResourceAsync(ArtifactResourceInfo artifactResourceInfo, CancellationToken cancellationToken = default);

    /// <summary>
    /// Attempts to get info for the artifact with the specified key.
    /// </summary>
    /// <param name="key">Artifact key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning retrieved artifact, if it exists.</returns>
    public abstract ValueTask<ArtifactInfo?> TryGetArtifactAsync(ArtifactKey key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Attempts to get info for the resource with the specified key.
    /// </summary>
    /// <param name="key">Resource key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning retrieved resource, if it exists.</returns>
    public abstract ValueTask<ArtifactResourceInfo?> TryGetResourceAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes artifact with the specified key.
    /// </summary>
    /// <param name="key">Artifact key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public abstract ValueTask RemoveArtifactAsync(ArtifactKey key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes resource with the specified key.
    /// </summary>
    /// <param name="key">Resource key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public abstract ValueTask RemoveResourceAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tests if artifact is recognizably new.
    /// </summary>
    /// <param name="artifactInfo">Artifact to check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if this is a new artifact (newer than whatever exists with the same ID).</returns>
    public virtual async ValueTask<bool> IsNewArtifactAsync(ArtifactInfo artifactInfo, CancellationToken cancellationToken = default)
    {
        return (await TryGetArtifactAsync(artifactInfo.Key, cancellationToken).ConfigureAwait(false)) is not { } oldArtifact
            || oldArtifact.UpdateDate == null
            || artifactInfo.UpdateDate != null && oldArtifact.UpdateDate.Value < artifactInfo.UpdateDate.Value;
    }
}
