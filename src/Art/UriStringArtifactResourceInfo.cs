using System.Text.Json;

namespace Art;

/// <summary>
/// Provides artifact information.
/// </summary>
/// <param name="ArtifactTool">Artifact tool.</param>
/// <param name="Uri">URI.</param>
/// <param name="Key">Artifact key.</param>
/// <param name="File">Filename.</param>
/// <param name="Path">Path.</param>
/// <param name="InArtifactFolder">If false, sent to common directory.</param>
/// <param name="Properties">Resource properties.</param>
public record UriStringArtifactResourceInfo(HttpArtifactTool ArtifactTool, string Uri, ArtifactKey Key, string File, string? Path, bool InArtifactFolder, IReadOnlyDictionary<string, JsonElement> Properties) : ArtifactResourceInfo(Key, File, Path, InArtifactFolder, Properties)
{
    /// <summary>
    /// Creates a new instance of <see cref="UriStringArtifactResourceInfo"/>.
    /// </summary>
    /// <param name="artifactTool">Artifact tool.</param>
    /// <param name="uri">URI.</param>
    /// <param name="key">Artifact key.</param>
    /// <param name="file">Filename.</param>
    /// <param name="path">Path.</param>
    /// <param name="inArtifactFolder">If false, sent to common directory.</param>
    /// <param name="properties">Resource properties.</param>
    /// <returns>Value.</returns>
    public static UriStringArtifactResourceInfo Create(HttpArtifactTool artifactTool, string uri, ArtifactKey key, string file, string? path = null, bool inArtifactFolder = true, IReadOnlyDictionary<string, JsonElement>? properties = null)
        => new(artifactTool, uri, key, file, path, inArtifactFolder, properties ?? EmptyProperties);

    /// <inheritdoc/>
    public override bool Exportable => true;

    /// <inheritdoc/>
    public override ValueTask ExportAsync(Stream stream)
        => ArtifactTool.DownloadResourceAsync(Uri, stream);
}
