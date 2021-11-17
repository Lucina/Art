using System.Text.Json;

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
    /// <param name="ArtifactId">Artifact ID.</param>
    /// <param name="File">Filename.</param>
    /// <param name="Path">Path.</param>
    public ArtifactResourceInfo(string ArtifactId, string File, string? Path = null) : this(ArtifactId, File, Path, EmptyProperties)
    {
    }

    /// <summary>
    /// Singleton dummy no-entry properties.
    /// </summary>
    public static readonly IReadOnlyDictionary<string, JsonElement> EmptyProperties = new Dictionary<string, JsonElement>();
}
