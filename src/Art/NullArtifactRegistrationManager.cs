using System.Linq.Expressions;

namespace Art;

/// <summary>
/// Represents an artifact registration manager that does not preserve data.
/// </summary>
public class NullArtifactRegistrationManager : ArtifactRegistrationManager
{
    /// <summary>
    /// Shared instance.
    /// </summary>
    public static readonly NullArtifactRegistrationManager Instance = new();

    /// <inheritdoc />
    public override Task<List<ArtifactInfo>> ListArtifactsAsync(CancellationToken cancellationToken = default) => Task.FromResult(new List<ArtifactInfo>());

    /// <inheritdoc />
    public override Task<List<ArtifactInfo>> ListArtifactsAsync(Expression<Func<ArtifactInfoModel, bool>> predicate, CancellationToken cancellationToken = default) => Task.FromResult(new List<ArtifactInfo>());

    /// <inheritdoc />
    public override Task<List<ArtifactInfo>> ListArtifactsAsync(string tool, CancellationToken cancellationToken = default) => Task.FromResult(new List<ArtifactInfo>());

    /// <inheritdoc />
    public override Task<List<ArtifactInfo>> ListArtifactsAsync(string tool, string group, CancellationToken cancellationToken = default) => Task.FromResult(new List<ArtifactInfo>());

    /// <inheritdoc />
    public override ValueTask AddArtifactAsync(ArtifactInfo artifactInfo, CancellationToken cancellationToken = default) => default;

    /// <inheritdoc />
    public override Task<List<ArtifactResourceInfo>> ListResourcesAsync(ArtifactKey key, CancellationToken cancellationToken = default) => Task.FromResult(new List<ArtifactResourceInfo>());

    /// <inheritdoc />
    public override ValueTask AddResourceAsync(ArtifactResourceInfo artifactResourceInfo, CancellationToken cancellationToken = default) => default;

    /// <inheritdoc />
    public override ValueTask<ArtifactInfo?> TryGetArtifactAsync(ArtifactKey key, CancellationToken cancellationToken = default) => new((ArtifactInfo?)null);

    /// <inheritdoc />
    public override ValueTask<ArtifactResourceInfo?> TryGetResourceAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default) => new((ArtifactResourceInfo?)null);

    /// <inheritdoc />
    public override ValueTask RemoveArtifactAsync(ArtifactKey key, CancellationToken cancellationToken = default) => default;

    /// <inheritdoc />
    public override ValueTask RemoveResourceAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default) => default;
}
