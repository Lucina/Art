using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Art.EF.Sqlite;

/// <summary>
/// Factory for sqlite-backed context for artifact registration..
/// </summary>
public class SqliteArtifactContextFactory : ArtifactContextFactoryBase
{
    /// <summary>
    /// Creates a new instance of <see cref="SqliteArtifactContextFactory"/>.
    /// </summary>
    /// <remarks>
    /// Sqlite file backing if environment variable art_ef_sqlite_backing_file is set, otherwise in-memory Sqlite backing
    /// </remarks>
    public SqliteArtifactContextFactory()
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="SqliteArtifactContextFactory"/> with in-memory Sqlite backing.
    /// </summary>
    /// <param name="inMemory">If true, use in-memory otherwise allow fallback to environment variable.</param>
    /// <remarks>
    /// Sqlite file backing if environment variable art_ef_sqlite_backing_file is set and <paramref name="inMemory"/> is false, otherwise in-memory Sqlite backing
    /// </remarks>
    public SqliteArtifactContextFactory(bool inMemory)
    {
        _inMemory = inMemory;
    }

    /// <summary>
    /// Creates a new instance of <see cref="SqliteArtifactContextFactory"/> with Sqlite file backing.
    /// </summary>
    /// <param name="storageFile">Path to sqlite storage file.</param>
    public SqliteArtifactContextFactory(string storageFile)
    {
        StorageFile = storageFile;
    }

    private readonly bool _inMemory;

    internal bool UsingInMemory;

    private const string Memory = "file::memory:?cache=shared";

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
        string? file = StorageFile ?? (_inMemory ? null : Environment.GetEnvironmentVariable(EnvStorageFile));
        if (file == null)
        {
            UsingInMemory = true;
            file = Memory;
        }
        var ob = new DbContextOptionsBuilder<ArtifactContext>();
        ob.UseSqlite($"DataSource={file};",
            b => b.MigrationsAssembly(MigrationAssembly.FullName));
        return new ArtifactContext(ob.Options);
    }
}
