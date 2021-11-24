using System.Text.Json;

namespace Art;

/// <summary>
/// Provides artifact information.
/// </summary>
/// <param name="ArtifactTool">Artifact tool.</param>
/// <param name="Uri">URI.</param>
/// <param name="Key">Resource key.</param>
/// <param name="Version">Version.</param>
/// <param name="Properties">Resource properties.</param>
public record UriStringArtifactResourceInfo(HttpArtifactTool ArtifactTool, string Uri, ArtifactResourceKey Key, string? Version, IReadOnlyDictionary<string, JsonElement> Properties)
    : ArtifactResourceInfo(Key, Version, Properties)
{
    /// <summary>
    /// Creates a new instance of <see cref="UriStringArtifactResourceInfo"/>.
    /// </summary>
    /// <param name="artifactTool">Artifact tool.</param>
    /// <param name="uri">URI.</param>
    /// <param name="key">Resource key.</param>
    /// <param name="version">Version.</param>
    /// <param name="properties">Resource properties.</param>
    /// <returns>Value.</returns>
    public static UriStringArtifactResourceInfo Create(HttpArtifactTool artifactTool, string uri, ArtifactResourceKey key, string? version = null, IReadOnlyDictionary<string, JsonElement>? properties = null)
        => new(artifactTool, uri, key, version, properties ?? EmptyProperties);

    /// <inheritdoc/>
    public override bool Exportable => true;

    /// <inheritdoc/>
    public override ValueTask ExportAsync(Stream stream)
        => ArtifactTool.DownloadResourceAsync(Uri, stream);
}
