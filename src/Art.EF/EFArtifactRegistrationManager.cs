using System.Linq.Expressions;
using Art.Management;
using Art.Resources;

namespace Art.EF;

/// <summary>
/// Represents an EF artifact registration manager.
/// </summary>
public class EFArtifactRegistrationManager : ArtifactRegistrationManager, IDisposable
{
    private bool _disposed;

    /// <summary>
    /// Database context.
    /// </summary>
    public ArtifactContext Context { get; private set; }

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
    {
        EnsureNotDisposed();
        return Context.ListArtifactsAsync(cancellationToken);
    }

    /// <inheritdoc />
    public override Task<List<ArtifactInfo>> ListArtifactsAsync(Expression<Func<ArtifactInfoModel, bool>> predicate, CancellationToken cancellationToken = default)
    {
        EnsureNotDisposed();
        return Context.ListArtifactsAsync(predicate, cancellationToken);
    }

    /// <inheritdoc />
    public override Task<List<ArtifactInfo>> ListArtifactsAsync(string tool, CancellationToken cancellationToken = default)
    {
        EnsureNotDisposed();
        return Context.ListArtifactsAsync(tool, cancellationToken);
    }

    /// <inheritdoc />
    public override Task<List<ArtifactInfo>> ListArtifactsAsync(string tool, string group, CancellationToken cancellationToken = default)
    {
        EnsureNotDisposed();
        return Context.ListArtifactsAsync(tool, group, cancellationToken);
    }

    /// <inheritdoc />
    public override ValueTask AddArtifactAsync(ArtifactInfo artifactInfo, CancellationToken cancellationToken = default)
    {
        EnsureNotDisposed();
        return Context.AddArtifactAsync(artifactInfo, cancellationToken);
    }

    /// <inheritdoc />
    public override Task<List<ArtifactResourceInfo>> ListResourcesAsync(ArtifactKey key, CancellationToken cancellationToken = default)
    {
        EnsureNotDisposed();
        return Context.ListResourcesAsync(key, cancellationToken);
    }

    /// <inheritdoc />
    public override ValueTask AddResourceAsync(ArtifactResourceInfo artifactResourceInfo, CancellationToken cancellationToken = default)
    {
        EnsureNotDisposed();
        return Context.AddResourceAsync(artifactResourceInfo, cancellationToken);
    }

    /// <inheritdoc />
    public override ValueTask<ArtifactInfo?> TryGetArtifactAsync(ArtifactKey key, CancellationToken cancellationToken = default)
    {
        EnsureNotDisposed();
        return Context.TryGetArtifactAsync(key, cancellationToken);
    }

    /// <inheritdoc />
    public override ValueTask<ArtifactResourceInfo?> TryGetResourceAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default)
    {
        EnsureNotDisposed();
        return Context.TryGetResourceAsync(key, cancellationToken);
    }

    /// <inheritdoc />
    public override ValueTask RemoveArtifactAsync(ArtifactKey key, CancellationToken cancellationToken = default)
    {
        EnsureNotDisposed();
        return Context.RemoveArtifactAsync(key, cancellationToken);
    }

    /// <inheritdoc />
    public override ValueTask RemoveResourceAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default)
    {
        EnsureNotDisposed();
        return Context.RemoveResourceAsync(key, cancellationToken);
    }

    /// <summary>
    /// Saves changes.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        EnsureNotDisposed();
        return Context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed) return;
        Context.Dispose();
        Context = null!;
        _disposed = true;
    }

    private void EnsureNotDisposed()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(EFArtifactRegistrationManager));
    }
}
