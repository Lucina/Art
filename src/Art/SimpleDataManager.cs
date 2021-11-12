using System.Diagnostics.CodeAnalysis;

namespace Art;

/// <summary>
/// Represents a simple <see cref="ArtifactDataManager"/> with purely file-based tracking.
/// </summary>
public class SimpleDataManager : ArtifactDataManager
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
    /// Creates a new instance of <see cref="SimpleDataManager"/>.
    /// </summary>
    /// <param name="baseDirectory">Base directory.</param>
    public SimpleDataManager(string baseDirectory)
    {
        BaseDirectory = baseDirectory;
    }

    /// <inheritdoc/>
    public override bool TryGetInfo(string id, [NotNullWhen(true)] out ArtifactInfo? artifactInfo)
    {
        string path = Path.Combine(BaseDirectory, id, ArtifactFileName);
        if (File.Exists(path))
        {
            artifactInfo = Extensions.LoadFromFile<ArtifactInfo>(path);
            return true;
        }
        artifactInfo = null;
        return false;
    }

    /// <inheritdoc/>
    public override string CreateOutputDirectory(ArtifactInfo artifactInfo)
    {
        string id = artifactInfo.Id;
        string dir = Path.Combine(BaseDirectory, id);
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        return dir;
    }

    /// <inheritdoc/>
    public override void AddInfo(ArtifactInfo artifactInfo)
    {
        if (artifactInfo.Id is not { } id) return;
        string dir = Path.Combine(BaseDirectory, id);
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        string path = Path.Combine(BaseDirectory, id, ArtifactFileName);
        artifactInfo.WriteToFile(path);
    }
}
