using System.Text.Json;
using System.Text.Json.Serialization;

namespace Art;

/// <summary>
/// Provides artifact information.
/// </summary>
/// <param name="ArtifactId">Artifact ID.</param>
/// <param name="File">Filename.</param>
/// <param name="Path">Path.</param>
/// <param name="Properties">Resource properties.</param>
public record ArtifactResourceInfo(string ArtifactId, string File, string? Path, IReadOnlyDictionary<string, JsonElement> Properties)
{
    /// <summary>
    /// Creates a new instance of <see cref="ArtifactInfo"/>.
    /// </summary>
    /// <param name="artifactId">Artifact ID.</param>
    /// <param name="file">Filename.</param>
    /// <param name="path">Path.</param>
    /// <param name="properties">Resource properties.</param>
    /// <returns>Value.</returns>
    public static ArtifactResourceInfo Create(string artifactId, string file, string? path = null, IReadOnlyDictionary<string, JsonElement>? properties = null)
        => new(artifactId, file, path, properties ?? EmptyProperties);

    /// <summary>
    /// Singleton dummy no-entry properties.
    /// </summary>
    public static readonly IReadOnlyDictionary<string, JsonElement> EmptyProperties = new Dictionary<string, JsonElement>();
}
