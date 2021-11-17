using System.Text.Json;
using System.Text.Json.Serialization;

namespace Art;

/// <summary>
/// Provides artifact information.
/// </summary>
/// <param name="ArtifactDumper">Artifact dumper.</param>
/// <param name="Uri">URI.</param>
/// <param name="ArtifactId">Artifact ID.</param>
/// <param name="File">Filename.</param>
/// <param name="Path">Path.</param>
/// <param name="InArtifactFolder">If false, sent to common directory.</param>
/// <param name="Properties">Resource properties.</param>
public record UriStringArtifactResourceInfo(HttpArtifactDumper ArtifactDumper, string Uri, string ArtifactId, string File, string? Path, bool InArtifactFolder, IReadOnlyDictionary<string, JsonElement> Properties) : ArtifactResourceInfo(ArtifactId, File, Path, InArtifactFolder, Properties)
{
    /// <summary>
    /// Creates a new instance of <see cref="ArtifactInfo"/>.
    /// </summary>
    /// <param name="artifactDumper">Artifact dumper.</param>
    /// <param name="uri">URI.</param>
    /// <param name="artifactId">Artifact ID.</param>
    /// <param name="file">Filename.</param>
    /// <param name="path">Path.</param>
    /// <param name="inArtifactFolder">If false, sent to common directory.</param>
    /// <param name="properties">Resource properties.</param>
    /// <returns>Value.</returns>
    public static UriStringArtifactResourceInfo Create(HttpArtifactDumper artifactDumper, string uri, string artifactId, string file, string? path = null, bool inArtifactFolder = false, IReadOnlyDictionary<string, JsonElement>? properties = null)
        => new(artifactDumper, uri, artifactId, file, path, inArtifactFolder, properties ?? EmptyProperties);

    /// <inheritdoc/>
    public override bool Exportable => true;

    /// <inheritdoc/>
    public override ValueTask ExportAsync(Stream stream)
        => ArtifactDumper.DownloadResourceAsync(Uri, stream);
}
