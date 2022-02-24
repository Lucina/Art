using System.Linq.Expressions;
using Art.Crypto;
using Art.Resources;
using Microsoft.EntityFrameworkCore;

namespace Art.EF;

/// <summary>
/// DbContext for artifact registration.
/// </summary>
public class ArtifactContext : DbContext
{
    private readonly AutoResetEvent _wh;
    private bool _disposed;

    /*private static readonly Type[] s_types = { typeof(ArtifactInfoModel), typeof(ArtifactResourceInfoModel) };*/

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
        /*MethodInfo baseMethod = typeof(DbContext).GetMethod(nameof(Set), 1, Array.Empty<Type>()) ??
                                throw new ApplicationException();
        object[] args = Array.Empty<object>();
        foreach (Type type in s_types) baseMethod.MakeGenericMethod(type).Invoke(this, args);*/
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
        _wh.WaitOne();
        try
        {
            ((string tool, string group, string id), string? _, DateTimeOffset? date, DateTimeOffset? updateDate, bool full) = artifactInfo;
            ArtifactInfoModel? model = await ArtifactInfoModels.FindAsync(new object?[] { tool, group, id }, cancellationToken);
            if (model == null)
            {
                ArtifactInfoModels.Add(artifactInfo);
                await SaveChangesAsync(cancellationToken);
            }
            else
            {
                model.Date = date;
                model.UpdateDate = updateDate;
                model.Full = full;
                ArtifactInfoModels.Update(model);
                await SaveChangesAsync(cancellationToken);
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
        _wh.WaitOne();
        try
        {
            List<ArtifactInfoModel> results = await ArtifactInfoModels.ToListAsync(cancellationToken);
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
    public async Task<List<ArtifactInfo>> ListArtifactsAsync(Expression<Func<ArtifactInfoModel, bool>> predicate, CancellationToken cancellationToken = default)
    {
        _wh.WaitOne();
        try
        {
            List<ArtifactInfoModel> results = await ArtifactInfoModels.Where(predicate).ToListAsync(cancellationToken);
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
        _wh.WaitOne();
        try
        {
            List<ArtifactInfoModel> results = await ArtifactInfoModels.Where(v => v.Tool == tool).ToListAsync(cancellationToken);
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
        _wh.WaitOne();
        try
        {
            List<ArtifactInfoModel> results = await ArtifactInfoModels.Where(v => v.Tool == tool && v.Group == group).ToListAsync(cancellationToken);
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
        _wh.WaitOne();
        try
        {
            (((string tool, string group, string id), string file, string? path), string? contentType, DateTimeOffset? updated, string? version, Checksum? checksum) = artifactResourceInfo;
            ArtifactInfoModel? model = await ArtifactInfoModels.FindAsync(new object?[] { tool, group, id }, cancellationToken);
            if (model == null) throw new InvalidOperationException("Can't add resource for missing artifact");
            ArtifactResourceInfoModel? model2 = await ArtifactResourceInfoModels.FindAsync(new object?[] { tool, group, id, file, path }, cancellationToken);
            if (model2 == null)
            {
                ArtifactResourceInfoModels.Add(artifactResourceInfo);
                await SaveChangesAsync(cancellationToken);
            }
            else
            {
                model2.ContentType = contentType;
                model2.Updated = updated;
                model2.Version = version;
                model2.ChecksumId = checksum?.Id;
                model2.ChecksumValue = checksum?.Value;
                ArtifactResourceInfoModels.Update(model2);
                await SaveChangesAsync(cancellationToken);
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
        _wh.WaitOne();
        (string? tool, string? group, string? id) = key;
        try
        {
            List<ArtifactResourceInfoModel> results = await ArtifactResourceInfoModels.Where(v => v.ArtifactTool == tool && v.ArtifactGroup == group && v.ArtifactId == id).ToListAsync(cancellationToken);
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
        _wh.WaitOne();
        try
        {
            (string tool, string group, string id) = key;
            ArtifactInfoModel? model = await ArtifactInfoModels.FindAsync(new object?[] { tool, group, id }, cancellationToken);
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
        _wh.WaitOne();
        try
        {
            ((string tool, string group, string id), string file, string? path) = key;
            ArtifactResourceInfoModel? model = await ArtifactResourceInfoModels.FindAsync(new object?[] { tool, group, id, file, path }, cancellationToken);
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
        _wh.WaitOne();
        try
        {
            (string tool, string group, string id) = key;
            ArtifactInfoModel? model = await ArtifactInfoModels.FindAsync(new object?[] { tool, group, id }, cancellationToken);
            if (model != null)
            {
                ArtifactInfoModels.Remove(model);
                await SaveChangesAsync(cancellationToken);
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
        _wh.WaitOne();
        try
        {
            ((string tool, string group, string id), string file, string? path) = key;
            ArtifactResourceInfoModel? model = await ArtifactResourceInfoModels.FindAsync(new object?[] { tool, group, id, file, path }, cancellationToken);
            if (model != null)
            {
                ArtifactResourceInfoModels.Remove(model);
                await SaveChangesAsync(cancellationToken);
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
        /*object[] args = { modelBuilder };
        foreach (Type type in s_types)
        foreach (MethodInfo method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                     .Where(m => m.GetCustomAttribute<ModelBuilderCallbackAttribute>() != null))
            method.Invoke(null, args);*/
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
        base.Dispose();
        if (_disposed) return;
        _disposed = true;
        _wh.Dispose();
    }

    /// <inheritdoc />
    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        if (_disposed) return;
        _disposed = true;
        _wh.Dispose();
    }

    private void EnsureNotDisposed()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(ArtifactContext));
    }
}
