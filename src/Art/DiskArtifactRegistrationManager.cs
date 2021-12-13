using System.Globalization;
using System.Runtime.CompilerServices;

namespace Art;

/// <summary>
/// Represents a simple <see cref="ArtifactRegistrationManager"/> with purely file-based tracking.
/// </summary>
public class DiskArtifactRegistrationManager : ArtifactRegistrationManager
{
    private const string ArtifactDir = ".artifacts";
    private const string ArtifactFileName = "{0}.json";
    private const string ArtifactFileNameEnd = ".json";
    internal const string ResourceDir = ".resources";
    private const string ResourceFileName = "{0}.RTFTRSRC.json";
    private const string ResourceFileNameEnd = ".RTFTRSRC.json";

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
    public override async IAsyncEnumerable<ArtifactInfo> ListArtifactsAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string dir = GetArtifactInfoDir();
        if (!Directory.Exists(dir)) yield break;
        foreach (string toolDir in Directory.EnumerateDirectories(dir))
        foreach (string groupDir in Directory.EnumerateDirectories(toolDir))
        foreach (string file in Directory.EnumerateFiles(groupDir).Where(v => v.EndsWith(ArtifactFileNameEnd)))
            if (await ArtExtensions.LoadFromFileAsync<ArtifactInfo>(file, cancellationToken).ConfigureAwait(false) is { } v)
                yield return v;
    }

    /// <inheritdoc/>
    public override async IAsyncEnumerable<ArtifactInfo> ListArtifactsAsync(string tool, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string toolDir = GetArtifactInfoDir(tool);
        if (!Directory.Exists(toolDir)) yield break;
        foreach (string groupDir in Directory.EnumerateDirectories(toolDir))
        foreach (string file in Directory.EnumerateFiles(groupDir).Where(v => v.EndsWith(ArtifactFileNameEnd)))
            if (await ArtExtensions.LoadFromFileAsync<ArtifactInfo>(file, cancellationToken).ConfigureAwait(false) is { } v)
                yield return v;
    }

    /// <inheritdoc/>
    public override async IAsyncEnumerable<ArtifactInfo> ListArtifactsAsync(string tool, string group, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string groupDir = GetArtifactInfoDir(tool, group);
        if (!Directory.Exists(groupDir)) yield break;
        foreach (string file in Directory.EnumerateFiles(groupDir).Where(v => v.EndsWith(ArtifactFileNameEnd)))
            if (await ArtExtensions.LoadFromFileAsync<ArtifactInfo>(file, cancellationToken).ConfigureAwait(false) is { } v)
                yield return v;
    }

    /// <inheritdoc/>
    public override async IAsyncEnumerable<ArtifactResourceInfo> ListResourcesAsync(ArtifactKey key, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string dir = GetResourceInfoDir(key);
        Queue<string> dQueue = new();
        dQueue.Enqueue(dir);
        while (dQueue.TryDequeue(out string? dd))
        {
            foreach (string f in Directory.EnumerateFiles(dd).Where(v => v.EndsWith(ResourceFileNameEnd)))
                if (await ArtExtensions.LoadFromFileAsync<ArtifactResourceInfo>(f, cancellationToken).ConfigureAwait(false) is { } v)
                    yield return v;
            foreach (string d in Directory.EnumerateDirectories(dd))
                dQueue.Enqueue(d);
        }
    }

    /// <inheritdoc/>
    public override async ValueTask AddArtifactAsync(ArtifactInfo artifactInfo, CancellationToken cancellationToken = default)
    {
        string dir = GetArtifactInfoDir(artifactInfo.Key);
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        string path = GetArtifactInfoFilePath(dir, artifactInfo.Key);
        await artifactInfo.WriteToFileAsync(path, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async ValueTask AddResourceAsync(ArtifactResourceInfo artifactResourceInfo, CancellationToken cancellationToken = default)
    {
        string dir = GetResourceInfoDir(artifactResourceInfo.Key);
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        string path = GetResourceInfoFilePath(dir, artifactResourceInfo.Key);
        await artifactResourceInfo.WriteToFileAsync(path, cancellationToken).ConfigureAwait(false);
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

    /// <inheritdoc/>
    public override ValueTask RemoveArtifactAsync(ArtifactKey key, CancellationToken cancellationToken = default)
    {
        string dir = GetArtifactInfoDir(key);
        string path = GetArtifactInfoFilePath(dir, key);
        if (File.Exists(path))
            File.Delete(path);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    public override ValueTask RemoveResourceAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default)
    {
        string dir = GetResourceInfoDir(key);
        string path = GetResourceInfoFilePath(dir, key);
        if (File.Exists(path))
            File.Delete(path);
        return ValueTask.CompletedTask;
    }

    private string GetSubPath(string sub)
    {
        return DiskPaths.GetSubPath(BaseDirectory, sub);
    }

    private string GetSubPath(string sub, string tool)
    {
        return DiskPaths.GetSubPath(BaseDirectory, sub, tool);
    }

    private string GetSubPath(string sub, string tool, string group)
    {
        return DiskPaths.GetSubPath(BaseDirectory, sub, tool, group);
    }

    private string GetSubPath(string sub, ArtifactKey key)
    {
        return DiskPaths.GetSubPath(BaseDirectory, sub, key.Tool, key.Group, key.Id);
    }

    private string GetArtifactInfoDir(ArtifactKey key)
        => GetArtifactInfoDir(key.Tool, key.Group);

    private string GetArtifactInfoDir()
        => GetSubPath(ArtifactDir);

    private string GetArtifactInfoDir(string tool)
        => GetSubPath(ArtifactDir, tool);

    private string GetArtifactInfoDir(string tool, string group)
        => GetSubPath(ArtifactDir, tool, group);

    private static string GetArtifactInfoFilePath(string dir, ArtifactKey key)
        => Path.Combine(dir, string.Format(CultureInfo.InvariantCulture, ArtifactFileName, key.Id));

    private string GetResourceInfoDir(ArtifactKey key)
        => GetSubPath(ResourceDir, key);

    private string GetResourceInfoDir(ArtifactResourceKey key)
        => Path.Combine(GetSubPath(ResourceDir, key.Artifact), key.Path);

    private static string GetResourceInfoFilePath(string dir, ArtifactResourceKey key)
        => Path.Combine(dir, string.Format(CultureInfo.InvariantCulture, ResourceFileName, key.File.SafeifyFileName()));
}
