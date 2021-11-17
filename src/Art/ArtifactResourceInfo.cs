using System.Text.Json;
using System.Text.Json.Serialization;

namespace Art;

/// <summary>
/// Provides artifact information.
/// </summary>
/// <param name="ArtifactId">Artifact ID.</param>
/// <param name="File">Filename.</param>
/// <param name="Path">Path.</param>
/// <param name="InArtifactFolder">If false, sent to common directory.</param>
/// <param name="Properties">Resource properties.</param>
public record ArtifactResourceInfo(string ArtifactId, string File, string? Path, bool InArtifactFolder, IReadOnlyDictionary<string, JsonElement> Properties)
{
    /// <summary>
    /// Creates a new instance of <see cref="ArtifactInfo"/>.
    /// </summary>
    /// <param name="artifactId">Artifact ID.</param>
    /// <param name="file">Filename.</param>
    /// <param name="path">Path.</param>
    /// <param name="inArtifactFolder">If false, sent to common directory.</param>
    /// <param name="properties">Resource properties.</param>
    /// <returns>Value.</returns>
    public static ArtifactResourceInfo Create(string artifactId, string file, string? path = null, bool inArtifactFolder = false, IReadOnlyDictionary<string, JsonElement>? properties = null)
        => new(artifactId, file, path, inArtifactFolder, properties ?? EmptyProperties);

    /// <summary>
    /// Singleton dummy no-entry properties.
    /// </summary>
    public static readonly IReadOnlyDictionary<string, JsonElement> EmptyProperties = new Dictionary<string, JsonElement>();

    /// <summary>
    /// If true, this resource is a file-exportable reference.
    /// </summary>
    public virtual bool Exportable => false;

    /// <summary>
    /// Exports a resource.
    /// </summary>
    /// <param name="stream">Stream to export to.</param>
    /// <returns>Task.</returns>
    /// <exception cref="NotSupportedException">Thrown if this instance cannot be exported.</exception>
    public virtual ValueTask ExportAsync(Stream stream)
        => throw new NotSupportedException($"This is a raw instance of {nameof(ArtifactResourceInfo)} that is not exportable");
}
