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
    public override async ValueTask AddArtifactAsync(ArtifactInfo artifactInfo, CancellationToken cancellationToken = default)
    {
        string dir = GetArtifactInfoDir(artifactInfo.Key);
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        string path = GetArtifactInfoFilePath(dir, artifactInfo.Key);
        await artifactInfo.WriteToFileAsync(path, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async ValueTask AddResourceAsync(ArtifactResourceInfo artifactResourceInfo, CancellationToken cancellationToken = default)
    {
        string dir = GetResourceInfoDir(artifactResourceInfo.Key);
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        string path = GetResourceInfoFilePath(dir, artifactResourceInfo.Key);
        await artifactResourceInfo.WriteToFileAsync(path, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async ValueTask<ArtifactInfo?> TryGetArtifactAsync(ArtifactKey key, CancellationToken cancellationToken = default)
    {
        string dir = GetArtifactInfoDir(key);
        string path = GetArtifactInfoFilePath(dir, key);
        return File.Exists(path) ? await ArtExtensions.LoadFromFileAsync<ArtifactInfo>(path, cancellationToken).ConfigureAwait(false) : null;
    }

    /// <inheritdoc/>
    public override async ValueTask<ArtifactResourceInfo?> TryGetResourceAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default)
    {
        string dir = GetResourceInfoDir(key);
        string path = GetResourceInfoFilePath(dir, key);
        return File.Exists(path) ? await ArtExtensions.LoadFromFileAsync<ArtifactResourceInfo>(path, cancellationToken).ConfigureAwait(false) : null;
    }

    private string GetBasePath(ArtifactKey key)
    {
        return DiskPaths.GetBasePath(BaseDirectory, key.Tool, key.Group);
    }

    private string GetBasePath(ArtifactResourceKey key)
    {
        return DiskPaths.GetBasePath(BaseDirectory, key.Artifact.Tool, key.Artifact.Group);
    }

    private string GetArtifactInfoDir(ArtifactKey key)
    {
        return Path.Combine(GetBasePath(key), ArtifactDir);
    }

    private static string GetArtifactInfoFilePath(string dir, ArtifactKey key)
    {
        return Path.Combine(dir, string.Format(CultureInfo.InvariantCulture, ArtifactFileName, key.Id));
    }

    private string GetResourceInfoDir(ArtifactResourceKey key)
    {
        string dir = Path.Combine(GetBasePath(key), ResourceDir);
        dir = DiskPaths.GetResourceDir(dir, key.Artifact.Id, key.Path, key.InArtifactFolder);
        return dir;
    }

    private static string GetResourceInfoFilePath(string dir, ArtifactResourceKey key)
    {
        return Path.Combine(dir, string.Format(CultureInfo.InvariantCulture, ResourceFileName, key.File.SafeifyFileName()));
    }
}
