namespace Art.EF;

/// <summary>
/// Represents an EF artifact registration manager.
/// </summary>
public class EFArtifactRegistrationManager : ArtifactRegistrationManager, IDisposable
{
    /// <summary>
    /// Database context.
    /// </summary>
    public readonly ArtifactContext Context;

    /// <summary>
    /// Creates a new instance of <see cref="EFArtifactRegistrationManager"/> using the specified factory.
    /// </summary>
    /// <param name="factory">Context factory.</param>
    public EFArtifactRegistrationManager(ArtifactContextFactoryBase factory)
    {
        Context = factory.CreateDbContext(Array.Empty<string>());
    }

    /// <inheritdoc />
    public override Task<List<ArtifactInfo>> ListArtifactsAsync(CancellationToken cancellationToken = default)
        => Context.ListArtifactsAsync(cancellationToken);

    /// <inheritdoc />
    public override Task<List<ArtifactInfo>> ListArtifactsAsync(string tool, CancellationToken cancellationToken = default)
        => Context.ListArtifactsAsync(tool, cancellationToken);

    /// <inheritdoc />
    public override Task<List<ArtifactInfo>> ListArtifactsAsync(string tool, string group, CancellationToken cancellationToken = default)
        => Context.ListArtifactsAsync(tool, group, cancellationToken);

    /// <inheritdoc />
    public override Task<List<ArtifactResourceInfo>> ListResourcesAsync(ArtifactKey key, CancellationToken cancellationToken = default)
        => Context.ListResourcesAsync(key, cancellationToken);

    /// <inheritdoc />
    public override ValueTask AddArtifactAsync(ArtifactInfo artifactInfo, CancellationToken cancellationToken = default)
        => Context.AddArtifactAsync(artifactInfo, cancellationToken);

    /// <inheritdoc />
    public override ValueTask AddResourceAsync(ArtifactResourceInfo artifactResourceInfo, CancellationToken cancellationToken = default)
        => Context.AddResourceAsync(artifactResourceInfo, cancellationToken);

    /// <inheritdoc />
    public override ValueTask<ArtifactInfo?> TryGetArtifactAsync(ArtifactKey key, CancellationToken cancellationToken = default)
        => Context.TryGetArtifactAsync(key, cancellationToken);

    /// <inheritdoc />
    public override ValueTask<ArtifactResourceInfo?> TryGetResourceAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default)
        => Context.TryGetResourceAsync(key, cancellationToken);

    /// <inheritdoc />
    public override ValueTask RemoveArtifactAsync(ArtifactKey key, CancellationToken cancellationToken = default)
        => Context.RemoveArtifactAsync(key, cancellationToken);

    /// <inheritdoc />
    public override ValueTask RemoveResourceAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default)
        => Context.RemoveResourceAsync(key, cancellationToken);

    /// <summary>
    /// Saves changes.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => Context.SaveChangesAsync(cancellationToken);

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        Context.Dispose();
    }
}
