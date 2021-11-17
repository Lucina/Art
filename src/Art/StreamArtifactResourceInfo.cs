using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Art;

/// <summary>
/// Provides artifact information.
/// </summary>
/// <param name="Resource">Resource.</param>
/// <param name="ArtifactId">Artifact ID.</param>
/// <param name="File">Filename.</param>
/// <param name="Path">Path.</param>
/// <param name="InArtifactFolder">If false, sent to common directory.</param>
/// <param name="Properties">Resource properties.</param>
public record StreamArtifactResourceInfo(Stream Resource, string ArtifactId, string File, string? Path, bool InArtifactFolder, IReadOnlyDictionary<string, JsonElement> Properties) : ArtifactResourceInfo(ArtifactId, File, Path, InArtifactFolder, Properties)
{
    /// <summary>
    /// Creates a new instance of <see cref="StreamArtifactResourceInfo"/>.
    /// </summary>
    /// <param name="resource">Resource.</param>
    /// <param name="artifactId">Artifact ID.</param>
    /// <param name="file">Filename.</param>
    /// <param name="path">Path.</param>
    /// <param name="inArtifactFolder">If false, sent to common directory.</param>
    /// <param name="properties">Resource properties.</param>
    /// <returns>Value.</returns>
    public static StreamArtifactResourceInfo Create(Stream resource, string artifactId, string file, string? path = null, bool inArtifactFolder = false, IReadOnlyDictionary<string, JsonElement>? properties = null)
        => new(resource, artifactId, file, path, inArtifactFolder, properties ?? EmptyProperties);

    /// <inheritdoc/>
    public override bool Exportable => true;

    /// <inheritdoc/>
    public override async ValueTask ExportAsync(Stream stream)
    {
        await Resource.CopyToAsync(stream).ConfigureAwait(false);
    }
}
