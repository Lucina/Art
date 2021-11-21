using System.Globalization;

namespace Art;

/// <summary>
/// Represents a simple <see cref="ArtifactRegistrationManager"/> with purely file-based tracking.
/// </summary>
public class DiskArtifactRegistrationManager : ArtifactRegistrationManager
{
    private const string ArtifactDir = ".artifacts";
    private const string ArtifactFileName = "{0}.json";

    /// <summary>
    /// Base directory.
    /// </summary>
    public string BaseDirectory { get; }

    /// <summary>
    /// Creates a new instance of <see cref="DiskArtifactDataManager"/>.
    /// </summary>
    /// <param name="baseDirectory">Base directory.</param>
    public DiskArtifactRegistrationManager(string baseDirectory)
    {
        BaseDirectory = baseDirectory;
    }

    /// <inheritdoc/>
    public override async ValueTask<ArtifactInfo?> TryGetArtifactAsync(string toolId, string group, string id)
    {
        string dir = GetArtifactInfoDir(toolId, group);
        string path = GetArtifactInfoFilePath(dir, id);
        return File.Exists(path) ? await ArtExtensions.LoadFromFileAsync<ArtifactInfo>(path).ConfigureAwait(false) : null;
    }

    /// <inheritdoc/>
    public override async ValueTask AddArtifactAsync(ArtifactInfo artifactInfo)
    {
        string dir = GetArtifactInfoDir(artifactInfo.Tool, artifactInfo.Group);
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        string path = GetArtifactInfoFilePath(dir, artifactInfo.Id);
        await artifactInfo.WriteToFileAsync(path).ConfigureAwait(false);
    }

    private string GetArtifactInfoDir(string toolId, string group) => Path.Combine(DiskPaths.GetBasePath(BaseDirectory, toolId, group), ArtifactDir);

    private static string GetArtifactInfoFilePath(string dir, string id) => Path.Combine(dir, string.Format(CultureInfo.InvariantCulture, ArtifactFileName, id));
}
