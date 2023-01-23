using System.Linq.Expressions;

namespace Art;

internal class HiddenNullArtifactRegistrationManager : IArtifactRegistrationManager
{
    /// <summary>
    /// Shared instance.
    /// </summary>
    public static readonly HiddenNullArtifactRegistrationManager Instance = new();

    /// <inheritdoc />
    public Task<List<ArtifactInfo>> ListArtifactsAsync(CancellationToken cancellationToken = default) => Task.FromResult(new List<ArtifactInfo>());

    /// <inheritdoc />
    public Task<List<ArtifactInfo>> ListArtifactsAsync(Expression<Func<ArtifactInfoModel, bool>> predicate, CancellationToken cancellationToken = default) => Task.FromResult(new List<ArtifactInfo>());

    /// <inheritdoc />
    public Task<List<ArtifactInfo>> ListArtifactsAsync(string tool, CancellationToken cancellationToken = default) => Task.FromResult(new List<ArtifactInfo>());

    /// <inheritdoc />
    public Task<List<ArtifactInfo>> ListArtifactsAsync(string tool, string group, CancellationToken cancellationToken = default) => Task.FromResult(new List<ArtifactInfo>());

    /// <inheritdoc />
    public ValueTask AddArtifactAsync(ArtifactInfo artifactInfo, CancellationToken cancellationToken = default) => default;

    /// <inheritdoc />
    public Task<List<ArtifactResourceInfo>> ListResourcesAsync(ArtifactKey key, CancellationToken cancellationToken = default) => Task.FromResult(new List<ArtifactResourceInfo>());

    /// <inheritdoc />
    public ValueTask AddResourceAsync(ArtifactResourceInfo artifactResourceInfo, CancellationToken cancellationToken = default) => default;

    /// <inheritdoc />
    public ValueTask<ArtifactInfo?> TryGetArtifactAsync(ArtifactKey key, CancellationToken cancellationToken = default) => new((ArtifactInfo?)null);

    /// <inheritdoc />
    public ValueTask<ArtifactResourceInfo?> TryGetResourceAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default) => new((ArtifactResourceInfo?)null);

    /// <inheritdoc />
    public ValueTask RemoveArtifactAsync(ArtifactKey key, CancellationToken cancellationToken = default) => default;

    /// <inheritdoc />
    public ValueTask RemoveResourceAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default) => default;
}
