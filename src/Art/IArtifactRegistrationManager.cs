using System.Linq.Expressions;

namespace Art;

/// <summary>
/// Represents a manager for artifact data.
/// </summary>
public interface IArtifactRegistrationManager
{
    /// <summary>
    /// Lists all artifacts.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning artifacts.</returns>
    Task<List<ArtifactInfo>> ListArtifactsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists all artifacts using the specified predicate.
    /// </summary>
    /// <param name="predicate">Predicate.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning artifacts.</returns>
    Task<List<ArtifactInfo>> ListArtifactsAsync(Expression<Func<ArtifactInfoModel, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists all artifacts for the specified tool.
    /// </summary>
    /// <param name="tool">Tool.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning artifacts.</returns>
    Task<List<ArtifactInfo>> ListArtifactsAsync(string tool, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists all artifacts for the specified tool and group.
    /// </summary>
    /// <param name="tool">Tool.</param>
    /// <param name="group">Group.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning artifacts.</returns>
    Task<List<ArtifactInfo>> ListArtifactsAsync(string tool, string group, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers artifact as known.
    /// </summary>
    /// <param name="artifactInfo">Artifact to register.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    ValueTask AddArtifactAsync(ArtifactInfo artifactInfo, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists all resources for the specified artifact key.
    /// </summary>
    /// <param name="key">Artifact key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning resources.</returns>
    Task<List<ArtifactResourceInfo>> ListResourcesAsync(ArtifactKey key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers artifact resource as known.
    /// </summary>
    /// <param name="artifactResourceInfo">Artifact resource to register.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    ValueTask AddResourceAsync(ArtifactResourceInfo artifactResourceInfo, CancellationToken cancellationToken = default);

    /// <summary>
    /// Attempts to get info for the artifact with the specified key.
    /// </summary>
    /// <param name="key">Artifact key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning retrieved artifact, if it exists.</returns>
    ValueTask<ArtifactInfo?> TryGetArtifactAsync(ArtifactKey key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Attempts to get info for the resource with the specified key.
    /// </summary>
    /// <param name="key">Resource key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning retrieved resource, if it exists.</returns>
    ValueTask<ArtifactResourceInfo?> TryGetResourceAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes artifact with the specified key.
    /// </summary>
    /// <param name="key">Artifact key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    ValueTask RemoveArtifactAsync(ArtifactKey key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes resource with the specified key.
    /// </summary>
    /// <param name="key">Resource key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    ValueTask RemoveResourceAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default);
}
