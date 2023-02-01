using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Art.EF;

/// <summary>
/// DbContext for artifact registration.
/// </summary>
public class ArtifactContext : DbContext
{
    private readonly AutoResetEvent _wh;
    private bool _disposed;

    /// <summary>
    /// Artifact info.
    /// </summary>
    private DbSet<ArtifactInfoModel> ArtifactInfoModels { get; set; } = null!;

    /// <summary>
    /// Artifact resource info.
    /// </summary>
    private DbSet<ArtifactResourceInfoModel> ArtifactResourceInfoModels { get; set; } = null!;

    /// <summary>
    /// Creates an instance of <see cref="ArtifactContext"/> with the specified options.
    /// </summary>
    /// <param name="options">Options.</param>
    public ArtifactContext(DbContextOptions<ArtifactContext> options) : base(options)
    {
        _wh = new AutoResetEvent(true);
    }

    /// <summary>
    /// Registers artifact as known.
    /// </summary>
    /// <param name="artifactInfo">Artifact to register.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public async ValueTask AddArtifactAsync(ArtifactInfo artifactInfo, CancellationToken cancellationToken = default)
    {
        EnsureNotDisposed();
        if (!_wh.WaitOne(0))
        {
            throw new InvalidOperationException($"Concurrent access to {nameof(ArtifactContext)} is disallowed");
        }
        try
        {
            ((string tool, string group, string id), string? _, DateTimeOffset? date, DateTimeOffset? updateDate, bool full) = artifactInfo;
            ArtifactInfoModel? model = await ArtifactInfoModels.FindAsync(new object?[] { tool, group, id }, cancellationToken).ConfigureAwait(false);
            if (model == null)
            {
                ArtifactInfoModels.Add(artifactInfo);
                await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
            else
            {
                model.Date = date;
                model.UpdateDate = updateDate;
                model.Full = full;
                ArtifactInfoModels.Update(model);
                await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }
        finally
        {
            _wh.Set();
        }
    }

    /// <summary>
    /// Lists all artifacts.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning artifacts.</returns>
    public async Task<List<ArtifactInfo>> ListArtifactsAsync(CancellationToken cancellationToken = default)
    {
        if (!_wh.WaitOne(0))
        {
            throw new InvalidOperationException($"Concurrent access to {nameof(ArtifactContext)} is disallowed");
        }
        try
        {
            List<ArtifactInfoModel> results = await ArtifactInfoModels.ToListAsync(cancellationToken).ConfigureAwait(false);
            return results.Select(v => (ArtifactInfo)v).ToList();
        }
        finally
        {
            _wh.Set();
        }
    }

    /// <summary>
    /// Lists all artifacts using the specified predicate.
    /// </summary>
    /// <param name="predicate">Predicate.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning artifacts.</returns>
    public async Task<List<ArtifactInfo>> ListArtifactsAsync(Func<ArtifactInfo, bool> predicate, CancellationToken cancellationToken = default)
    {
        if (!_wh.WaitOne(0))
        {
            throw new InvalidOperationException($"Concurrent access to {nameof(ArtifactContext)} is disallowed");
        }
        try
        {
            List<ArtifactInfoModel> results = await ArtifactInfoModels.ToListAsync(cancellationToken).ConfigureAwait(false);
            return results.Select(v => (ArtifactInfo)v).Where(predicate).ToList();
        }
        finally
        {
            _wh.Set();
        }
    }

    /// <summary>
    /// Lists all artifacts using the specified predicate.
    /// </summary>
    /// <param name="predicate">Predicate.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning artifacts.</returns>
    public async Task<List<ArtifactInfo>> ListArtifactsAsync(Expression<Func<ArtifactInfoModel, bool>> predicate, CancellationToken cancellationToken = default)
    {
        if (!_wh.WaitOne(0))
        {
            throw new InvalidOperationException($"Concurrent access to {nameof(ArtifactContext)} is disallowed");
        }
        try
        {
            List<ArtifactInfoModel> results = await ArtifactInfoModels.Where(predicate).ToListAsync(cancellationToken).ConfigureAwait(false);
            return results.Select(v => (ArtifactInfo)v).ToList();
        }
        finally
        {
            _wh.Set();
        }
    }

    /// <summary>
    /// Lists all artifacts for the specified tool.
    /// </summary>
    /// <param name="tool">Tool.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning artifacts.</returns>
    public async Task<List<ArtifactInfo>> ListArtifactsAsync(string tool, CancellationToken cancellationToken = default)
    {
        if (!_wh.WaitOne(0))
        {
            throw new InvalidOperationException($"Concurrent access to {nameof(ArtifactContext)} is disallowed");
        }
        try
        {
            List<ArtifactInfoModel> results = await ArtifactInfoModels.Where(v => v.Tool == tool).ToListAsync(cancellationToken).ConfigureAwait(false);
            return results.Select(v => (ArtifactInfo)v).ToList();
        }
        finally
        {
            _wh.Set();
        }
    }

    /// <summary>
    /// Lists all artifacts for the specified tool and group.
    /// </summary>
    /// <param name="tool">Tool.</param>
    /// <param name="group">Group.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning artifacts.</returns>
    public async Task<List<ArtifactInfo>> ListArtifactsAsync(string tool, string group, CancellationToken cancellationToken = default)
    {
        if (!_wh.WaitOne(0))
        {
            throw new InvalidOperationException($"Concurrent access to {nameof(ArtifactContext)} is disallowed");
        }
        try
        {
            List<ArtifactInfoModel> results = await ArtifactInfoModels.Where(v => v.Tool == tool && v.Group == group).ToListAsync(cancellationToken).ConfigureAwait(false);
            return results.Select(v => (ArtifactInfo)v).ToList();
        }
        finally
        {
            _wh.Set();
        }
    }

    /// <summary>
    /// Registers artifact resource as known.
    /// </summary>
    /// <param name="artifactResourceInfo">Artifact resource to register.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public async ValueTask AddResourceAsync(ArtifactResourceInfo artifactResourceInfo, CancellationToken cancellationToken = default)
    {
        EnsureNotDisposed();
        if (!_wh.WaitOne(0))
        {
            throw new InvalidOperationException($"Concurrent access to {nameof(ArtifactContext)} is disallowed");
        }
        try
        {
            (((string tool, string group, string id), string file, string? path), string? contentType, DateTimeOffset? updated, string? version, Checksum? checksum) = artifactResourceInfo;
            ArtifactInfoModel? model = await ArtifactInfoModels.FindAsync(new object?[] { tool, group, id }, cancellationToken).ConfigureAwait(false);
            if (model == null) throw new InvalidOperationException("Can't add resource for missing artifact");
            ArtifactResourceInfoModel? model2 = await ArtifactResourceInfoModels.FindAsync(new object?[] { tool, group, id, file, path }, cancellationToken).ConfigureAwait(false);
            if (model2 == null)
            {
                ArtifactResourceInfoModels.Add(artifactResourceInfo);
                await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
            else
            {
                model2.ContentType = contentType;
                model2.Updated = updated;
                model2.Version = version;
                model2.ChecksumId = checksum?.Id;
                model2.ChecksumValue = checksum?.Value;
                ArtifactResourceInfoModels.Update(model2);
                await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }
        finally
        {
            _wh.Set();
        }
    }

    /// <summary>
    /// Lists all resources for the specified artifact key.
    /// </summary>
    /// <param name="key">Artifact key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning resources.</returns>
    public async Task<List<ArtifactResourceInfo>> ListResourcesAsync(ArtifactKey key, CancellationToken cancellationToken = default)
    {
        if (!_wh.WaitOne(0))
        {
            throw new InvalidOperationException($"Concurrent access to {nameof(ArtifactContext)} is disallowed");
        }
        (string? tool, string? group, string? id) = key;
        try
        {
            List<ArtifactResourceInfoModel> results = await ArtifactResourceInfoModels.Where(v => v.ArtifactTool == tool && v.ArtifactGroup == group && v.ArtifactId == id).ToListAsync(cancellationToken).ConfigureAwait(false);
            return results.Select(v => (ArtifactResourceInfo)v).ToList();
        }
        finally
        {
            _wh.Set();
        }
    }

    /// <summary>
    /// Attempts to get info for the artifact with the specified key.
    /// </summary>
    /// <param name="key">Artifact key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning retrieved artifact, if it exists.</returns>
    public async ValueTask<ArtifactInfo?> TryGetArtifactAsync(ArtifactKey key, CancellationToken cancellationToken = default)
    {
        EnsureNotDisposed();
        if (!_wh.WaitOne(0))
        {
            throw new InvalidOperationException($"Concurrent access to {nameof(ArtifactContext)} is disallowed");
        }
        try
        {
            (string tool, string group, string id) = key;
            ArtifactInfoModel? model = await ArtifactInfoModels.FindAsync(new object?[] { tool, group, id }, cancellationToken).ConfigureAwait(false);
            return model != null ? (ArtifactInfo)model : null;
        }
        finally
        {
            _wh.Set();
        }
    }

    /// <summary>
    /// Attempts to get info for the resource with the specified key.
    /// </summary>
    /// <param name="key">Resource key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning retrieved resource, if it exists.</returns>
    public async ValueTask<ArtifactResourceInfo?> TryGetResourceAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default)
    {
        EnsureNotDisposed();
        if (!_wh.WaitOne(0))
        {
            throw new InvalidOperationException($"Concurrent access to {nameof(ArtifactContext)} is disallowed");
        }
        try
        {
            ((string tool, string group, string id), string file, string? path) = key;
            ArtifactResourceInfoModel? model = await ArtifactResourceInfoModels.FindAsync(new object?[] { tool, group, id, file, path }, cancellationToken).ConfigureAwait(false);
            return model != null ? (ArtifactResourceInfo)model : null;
        }
        finally
        {
            _wh.Set();
        }
    }

    /// <summary>
    /// Removes artifact with the specified key.
    /// </summary>
    /// <param name="key">Artifact key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public async ValueTask RemoveArtifactAsync(ArtifactKey key, CancellationToken cancellationToken = default)
    {
        EnsureNotDisposed();
        if (!_wh.WaitOne(0))
        {
            throw new InvalidOperationException($"Concurrent access to {nameof(ArtifactContext)} is disallowed");
        }
        try
        {
            (string tool, string group, string id) = key;
            ArtifactInfoModel? model = await ArtifactInfoModels.FindAsync(new object?[] { tool, group, id }, cancellationToken).ConfigureAwait(false);
            if (model != null)
            {
                ArtifactInfoModels.Remove(model);
                await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }
        finally
        {
            _wh.Set();
        }
    }

    /// <summary>
    /// Removes resource with the specified key.
    /// </summary>
    /// <param name="key">Resource key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public async ValueTask RemoveResourceAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default)
    {
        EnsureNotDisposed();
        if (!_wh.WaitOne(0))
        {
            throw new InvalidOperationException($"Concurrent access to {nameof(ArtifactContext)} is disallowed");
        }
        try
        {
            ((string tool, string group, string id), string file, string? path) = key;
            ArtifactResourceInfoModel? model = await ArtifactResourceInfoModels.FindAsync(new object?[] { tool, group, id, file, path }, cancellationToken).ConfigureAwait(false);
            if (model != null)
            {
                ArtifactResourceInfoModels.Remove(model);
                await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }
        finally
        {
            _wh.Set();
        }
    }

    /// <inheritdoc />
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        EnsureNotDisposed();
        optionsBuilder.UseLazyLoadingProxies();
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        EnsureNotDisposed();
        modelBuilder.Entity<ArtifactInfoModel>(b =>
        {
            b.HasKey(x => new { x.Tool, x.Group, x.Id });
            b
                .HasMany(x => x.Resources)
                .WithOne(x => x.Artifact)
                .HasForeignKey(x => new { x.ArtifactTool, x.ArtifactGroup, x.ArtifactId })
                .OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<ArtifactResourceInfoModel>(b =>
        {
            b.HasKey(x => new
            {
                x.ArtifactTool,
                x.ArtifactGroup,
                x.ArtifactId,
                x.File,
                x.Path
            });
            b.HasIndex(x => new { x.ArtifactTool, x.ArtifactGroup, x.ArtifactId });
        });
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        _wh.WaitOne();
        base.Dispose();
        if (_disposed) return;
        _disposed = true;
        _wh.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public override async ValueTask DisposeAsync()
    {
        _wh.WaitOne();
        await base.DisposeAsync().ConfigureAwait(false);
        if (_disposed) return;
        _disposed = true;
        _wh.Dispose();
        GC.SuppressFinalize(this);
    }

    private void EnsureNotDisposed()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(ArtifactContext));
    }
}
