namespace Art.EF;

/// <summary>
/// Represents an EF artifact registration manager.
/// </summary>
public class EFArtifactRegistrationManager : ArtifactRegistrationManager, IDisposable
{
    private ArtifactContext _context;

    /// <summary>
    /// Creates a new instance of <see cref="EFArtifactRegistrationManager"/> using the specified factory.
    /// </summary>
    /// <param name="factory">Context factory.</param>
    public EFArtifactRegistrationManager(ArtifactContextFactoryBase factory)
    {
        _context = factory.CreateDbContext(Array.Empty<string>());
    }

    /// <inheritdoc />
    public override ValueTask AddArtifactAsync(ArtifactInfo artifactInfo, CancellationToken cancellationToken = default)
        => _context.AddArtifactAsync(artifactInfo, cancellationToken);

    /// <inheritdoc />
    public override ValueTask AddResourceAsync(ArtifactResourceInfo artifactResourceInfo, CancellationToken cancellationToken = default)
        => _context.AddResourceAsync(artifactResourceInfo, cancellationToken);

    /// <inheritdoc />
    public override ValueTask<ArtifactInfo?> TryGetArtifactAsync(ArtifactKey key, CancellationToken cancellationToken = default)
        => _context.TryGetArtifactAsync(key, cancellationToken);

    /// <inheritdoc />
    public override ValueTask<ArtifactResourceInfo?> TryGetResourceAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default)
        => _context.TryGetResourceAsync(key, cancellationToken);

    /// <inheritdoc />
    public override ValueTask RemoveArtifactAsync(ArtifactKey key, CancellationToken cancellationToken = default)
        => _context.RemoveArtifactAsync(key, cancellationToken);

    /// <inheritdoc />
    public override ValueTask RemoveResourceAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default)
        => _context.RemoveResourceAsync(key, cancellationToken);

    /// <summary>
    /// Saves changes.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _context.Dispose();
    }
}
