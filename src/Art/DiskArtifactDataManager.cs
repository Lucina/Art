using System.Diagnostics.CodeAnalysis;

namespace Art;

/// <summary>
/// Represents a simple <see cref="ArtifactRegistrationManager"/> with purely file-based tracking.
/// </summary>
public class DiskArtifactDataManager : ArtifactDataManager
{
    /// <summary>
    /// Main artifact file name.
    /// </summary>
    public const string ArtifactFileName = "artifact.json";

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
    public override ValueTask<Stream> CreateOutputStreamAsync(string file, ArtifactInfo artifactInfo, bool inArtifactFolder = true, string? path = null)
    {
        string dir = inArtifactFolder ? Path.Combine(BaseDirectory, artifactInfo.Id) : BaseDirectory;
        if (!string.IsNullOrEmpty(path)) dir = Path.Combine(dir, path);
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        return new ValueTask<Stream>(File.Create(Path.Combine(dir, file)));
    }
}
