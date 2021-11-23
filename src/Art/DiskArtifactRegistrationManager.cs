using System.Globalization;

namespace Art;

/// <summary>
/// Represents a simple <see cref="ArtifactRegistrationManager"/> with purely file-based tracking.
/// </summary>
public class DiskArtifactRegistrationManager : ArtifactRegistrationManager
{
    private const string ArtifactDir = ".artifacts";
    private const string ArtifactFileName = "{0}.json";
    private const string ResourceDir = ".resources";
    private const string ResourceFileName = "{0}.RTFTRSRC.json";

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
    public override async ValueTask<ArtifactInfo?> TryGetArtifactAsync(ArtifactKey key)
    {
        string dir = GetArtifactInfoDir(key);
        string path = GetArtifactInfoFilePath(dir, key);
        return File.Exists(path) ? await ArtExtensions.LoadFromFileAsync<ArtifactInfo>(path).ConfigureAwait(false) : null;
    }

    /// <inheritdoc/>
    public override async ValueTask AddArtifactAsync(ArtifactInfo artifactInfo)
    {
        string dir = GetArtifactInfoDir(artifactInfo.Key);
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        string path = GetArtifactInfoFilePath(dir, artifactInfo.Key);
        await artifactInfo.WriteToFileAsync(path).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async ValueTask AddResourceAsync(ArtifactResourceInfo artifactResourceInfo)
    {
        string dir = GetResourceInfoDir(artifactResourceInfo);
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        string path = GetResourceInfoFilePath(dir, artifactResourceInfo);
        await artifactResourceInfo.WriteToFileAsync(path).ConfigureAwait(false);
    }

    private string GetBasePath(ArtifactKey key)
    {
        return DiskPaths.GetBasePath(BaseDirectory, key.Tool, key.Group);
    }

    private string GetArtifactInfoDir(ArtifactKey key)
    {
        return Path.Combine(GetBasePath(key), ArtifactDir);
    }

    private static string GetArtifactInfoFilePath(string dir, ArtifactKey key)
    {
        return Path.Combine(dir, string.Format(CultureInfo.InvariantCulture, ArtifactFileName, key.Id));
    }

    private string GetResourceInfoDir(ArtifactResourceInfo ari)
    {
        string dir = Path.Combine(GetBasePath(ari.Key), ResourceDir);
        dir = DiskPaths.GetResourceDir(dir, ari.Key.Id, ari.Path, ari.InArtifactFolder);
        return dir;
    }

    private static string GetResourceInfoFilePath(string dir, ArtifactResourceInfo ari)
    {
        return Path.Combine(dir, string.Format(CultureInfo.InvariantCulture, ResourceFileName, ari.File.SafeifyFileName()));
    }
}
