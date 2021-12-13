using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Art.EF.Sqlite;

/// <summary>
/// Factory for sqlite-backed context for artifact registration..
/// </summary>
public class SqliteArtifactContextFactory : ArtifactContextFactoryBase
{
    /// <summary>
    /// Environment variable for path to sqlite storage file.
    /// </summary>
    public virtual string EnvStorageFile { get; set; } = "art_ef_sqlite_backing_file";

    /// <summary>
    /// Path to sqlite storage file.
    /// </summary>
    public string? StorageFile { get; set; }

    /// <summary>
    /// Assembly with migrations for the database
    /// </summary>
    public virtual Assembly MigrationAssembly => GetType().Assembly;

    /// <inheritdoc />
    public override ArtifactContext CreateDbContext(string[] args)
    {
        string? file = StorageFile ?? Environment.GetEnvironmentVariable(EnvStorageFile);
        if (file == null)
            throw new ApplicationException($"ENV {EnvStorageFile} not set");
        var ob = new DbContextOptionsBuilder<ArtifactContext>();
        ob.UseSqlite($"DataSource={file};",
            b => b.MigrationsAssembly(MigrationAssembly.FullName));
        return ArtifactContext.Create(ob.Options);
    }
}
