using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Art;

/// <summary>
/// Represents a simple <see cref="ArtifactRegistrationManager"/> with purely file-based tracking.
/// </summary>
public class DiskArtifactRegistrationManager : ArtifactRegistrationManager
{
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
    public override async Task<ArtifactInfo?> TryGetInfoAsync(string id)
    {
        string path = GetArtifactInfoFilePath(id);
        return File.Exists(path) ? await Extensions.LoadFromFileAsync<ArtifactInfo>(path).ConfigureAwait(false) : null;
    }

    /// <inheritdoc/>
    public override async Task AddInfoAsync(ArtifactInfo artifactInfo)
    {
        if (!Directory.Exists(BaseDirectory)) Directory.CreateDirectory(BaseDirectory);
        string path = GetArtifactInfoFilePath(artifactInfo.Id);
        await artifactInfo.WriteToFileAsync(path).ConfigureAwait(false);
    }

    private string GetArtifactInfoFilePath(string id) => Path.Combine(BaseDirectory, string.Format(CultureInfo.InvariantCulture, ArtifactFileName, id));
}
