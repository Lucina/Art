using System.Diagnostics.CodeAnalysis;

namespace Art;

/// <summary>
/// Represents a simple <see cref="ArtifactDataManager"/> with purely file-based tracking.
/// </summary>
public class SimpleArtifactDataManager : ArtifactDataManager
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
    /// Creates a new instance of <see cref="SimpleArtifactDataManager"/>.
    /// </summary>
    /// <param name="baseDirectory">Base directory.</param>
    public SimpleArtifactDataManager(string baseDirectory)
    {
        BaseDirectory = baseDirectory;
    }

    /// <inheritdoc/>
    public override async Task<ArtifactInfo?> TryGetInfoAsync(string id)
    {
        string path = Path.Combine(BaseDirectory, id, ArtifactFileName);
        return File.Exists(path) ? await Extensions.LoadFromFileAsync<ArtifactInfo>(path).ConfigureAwait(false) : null;
    }

    /// <inheritdoc/>
    public override Task<Stream> CreateOutputStreamAsync(ArtifactInfo artifactInfo, string file, string? path = null)
    {

        string id = artifactInfo.Id;
        string dir = Path.Combine(BaseDirectory, id);
        if (!string.IsNullOrEmpty(path)) dir = Path.Combine(dir, path);
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        return Task.FromResult((Stream)File.Create(Path.Combine(dir, file)));
    }

    /// <inheritdoc/>
    public override async Task AddInfoAsync(ArtifactInfo artifactInfo)
    {
        if (artifactInfo.Id is not { } id) return;
        string dir = Path.Combine(BaseDirectory, id);
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        string path = Path.Combine(BaseDirectory, id, ArtifactFileName);
        await artifactInfo.WriteToFileAsync(path).ConfigureAwait(false);
    }
}
