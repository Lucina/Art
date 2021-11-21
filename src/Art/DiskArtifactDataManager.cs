namespace Art;

/// <summary>
/// Represents a simple <see cref="ArtifactRegistrationManager"/> with purely file-based tracking.
/// </summary>
public class DiskArtifactDataManager : ArtifactDataManager
{
    /// <summary>
    /// Base directory.
    /// </summary>
    public string BaseDirectory { get; }

    /// <summary>
    /// Creates a new instance of <see cref="DiskArtifactDataManager"/>.
    /// </summary>
    /// <param name="baseDirectory">Base directory.</param>
    public DiskArtifactDataManager(string baseDirectory)
    {
        BaseDirectory = baseDirectory;
    }

    /// <inheritdoc/>
    public override ValueTask<Stream> CreateOutputStreamAsync(string file, ArtifactInfo artifactInfo, string? path = null, bool inArtifactFolder = true)
    {
        string dir = DiskPaths.GetBasePath(BaseDirectory, artifactInfo.Tool, artifactInfo.Group);
        dir = inArtifactFolder ? Path.Combine(dir, artifactInfo.Id) : dir;
        if (!string.IsNullOrEmpty(path)) dir = Path.Combine(dir, path);
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        return new ValueTask<Stream>(File.Create(Path.Combine(dir, file)));
    }
}
