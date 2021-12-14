using Microsoft.EntityFrameworkCore;

namespace Art.EF;

/// <summary>
/// DbContext for artifact registration.
/// </summary>
public class ArtifactContext : DbContext
{
    /*private static readonly Type[] s_types = { typeof(ArtifactInfoModel), typeof(ArtifactResourceInfoModel) };*/

    /// <summary>
    /// Artifact info.
    /// </summary>
    public DbSet<ArtifactInfoModel> ArtifactInfoModels { get; set; } = null!;

    /// <summary>
    /// Artifact resource info.
    /// </summary>
    public DbSet<ArtifactResourceInfoModel> ArtifactResourceInfoModels { get; set; } = null!;

    /// <summary>
    /// Creates an instance of <see cref="ArtifactContext"/> with the specified options.
    /// </summary>
    /// <param name="options">Options.</param>
    public ArtifactContext(DbContextOptions<ArtifactContext> options) : base(options)
    {
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
        ((string tool, string group, string id), string? name, DateTimeOffset? date, DateTimeOffset? updateDate, bool full) = artifactInfo;
        ArtifactInfoModel? model = await ArtifactInfoModels.FindAsync(new object?[] { tool, group, id }, cancellationToken);
        if (model == null)
        {
            model = new ArtifactInfoModel
            {
                Tool = tool,
                Group = group,
                Id = id,
                Name = name,
                Date = date,
                UpdateDate = updateDate,
                Full = full
            };
            ArtifactInfoModels.Add(model);
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

    /// <summary>
    /// Registers artifact resource as known.
    /// </summary>
    /// <param name="artifactResourceInfo">Artifact resource to register.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public async ValueTask AddResourceAsync(ArtifactResourceInfo artifactResourceInfo, CancellationToken cancellationToken = default)
    {
        (((string tool, string group, string id), string file, string? path), DateTimeOffset? updated, string? version) = artifactResourceInfo;
        ArtifactInfoModel? model = await ArtifactInfoModels.FindAsync(new object?[] { tool, group, id }, cancellationToken);
        if (model == null) throw new InvalidOperationException("Can't add resource for missing artifact");
        ArtifactResourceInfoModel? model2 = await ArtifactResourceInfoModels.FindAsync(new object?[] { tool, group, id, file, path }, cancellationToken);
        if (model2 == null)
        {
            model2 = new ArtifactResourceInfoModel
            {
                ArtifactTool = tool,
                ArtifactGroup = group,
                ArtifactId = id,
                File = file,
                Path = path,
                Updated = updated,
                Version = version
            };
            ArtifactResourceInfoModels.Add(model2);
            await SaveChangesAsync(cancellationToken);
        }
        else
        {
            model2.Version = version;
            ArtifactResourceInfoModels.Update(model2);
            await SaveChangesAsync(cancellationToken);
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
        (string tool, string group, string id) = key;
        ArtifactInfoModel? model = await ArtifactInfoModels.FindAsync(new object?[] { tool, group, id }, cancellationToken);
        return model != null ? new ArtifactInfo(key, model.Name, model.Date, model.UpdateDate, model.Full) : null;
    }

    /// <summary>
    /// Attempts to get info for the resource with the specified key.
    /// </summary>
    /// <param name="key">Resource key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning retrieved resource, if it exists.</returns>
    public async ValueTask<ArtifactResourceInfo?> TryGetResourceAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default)
    {
        ((string tool, string group, string id), string file, string? path) = key;
        ArtifactResourceInfoModel? model = await ArtifactResourceInfoModels.FindAsync(new object?[] { tool, group, id, file, path }, cancellationToken);
        return model != null ? new ArtifactResourceInfo(key, model.Updated, model.Version) : null;
    }

    /// <summary>
    /// Removes artifact with the specified key.
    /// </summary>
    /// <param name="key">Artifact key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public async ValueTask RemoveArtifactAsync(ArtifactKey key, CancellationToken cancellationToken = default)
    {
        (string tool, string group, string id) = key;
        ArtifactInfoModel? model = await ArtifactInfoModels.FindAsync(new object?[] { tool, group, id }, cancellationToken);
        if (model != null)
        {
            ArtifactInfoModels.Remove(model);
            await SaveChangesAsync(cancellationToken);
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
        ((string tool, string group, string id), string file, string? path) = key;
        ArtifactResourceInfoModel? model = await ArtifactResourceInfoModels.FindAsync(new object?[] { tool, group, id, file, path }, cancellationToken);
        if (model != null)
        {
            ArtifactResourceInfoModels.Remove(model);
            await SaveChangesAsync(cancellationToken);
        }
    }

    /// <inheritdoc />
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies();
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        /*object[] args = { modelBuilder };
        foreach (Type type in s_types)
        foreach (MethodInfo method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                     .Where(m => m.GetCustomAttribute<ModelBuilderCallbackAttribute>() != null))
            method.Invoke(null, args);*/
        ArtifactInfoModel.OnModelCreating(modelBuilder);
        ArtifactResourceInfoModel.OnModelCreating(modelBuilder);
    }
}
