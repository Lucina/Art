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
    public override ValueTask<Stream> CreateOutputStreamAsync(ArtifactResourceKey key)
    {
        string dir = DiskPaths.GetResourceDir(DiskPaths.GetBasePath(BaseDirectory, key.Artifact.Tool, key.Artifact.Group), key.Artifact.Id, key.Path, key.InArtifactFolder);
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        return new ValueTask<Stream>(File.Create(Path.Combine(dir, key.File.SafeifyFileName())));
    }
}
