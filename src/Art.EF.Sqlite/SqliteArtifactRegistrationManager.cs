using Microsoft.EntityFrameworkCore;

namespace Art.EF.Sqlite;

/// <summary>
/// Represents an sqlite-backed artifact registration manager.
/// </summary>
public class SqliteArtifactRegistrationManager : EFArtifactRegistrationManager
{
    private SqliteArtifactRegistrationManager(SqliteArtifactContextFactory factory) : base(factory)
    {
        if (factory.UsingInMemory)
        {
            Context.Database.EnsureDeleted();
            Context.Database.EnsureCreated();
        }
        else
        {
            Context.Database.Migrate();
        }
    }

    /// <summary>
    /// Creates a new instance of <see cref="SqliteArtifactRegistrationManager"/>.
    /// </summary>
    /// <remarks>
    /// Sqlite file backing if environment variable art_ef_sqlite_backing_file is set, otherwise in-memory Sqlite backing
    /// </remarks>
    public SqliteArtifactRegistrationManager() : this(new SqliteArtifactContextFactory())
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="SqliteArtifactRegistrationManager"/> with in-memory Sqlite backing.
    /// </summary>
    /// <remarks>
    ///  Sqlite file backing if environment variable art_ef_sqlite_backing_file is set, otherwise in-memory Sqlite backing
    /// </remarks>
    /// <param name="inMemory">If true, use in-memory otherwise allow fallback to environment variable.</param>
    /// <remarks>
    /// Sqlite file backing if environment variable art_ef_sqlite_backing_file is set and <paramref name="inMemory"/> is false, otherwise in-memory Sqlite backing
    /// </remarks>
    public SqliteArtifactRegistrationManager(bool inMemory) : this(new SqliteArtifactContextFactory(inMemory))
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="SqliteArtifactRegistrationManager"/> with the specified Sqlite backing file.
    /// </summary>
    /// <param name="file">Sqlite backing file.</param>
    public SqliteArtifactRegistrationManager(string file) : this(new SqliteArtifactContextFactory(file))
    {
    }
}
