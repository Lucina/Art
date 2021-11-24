using System.Text.Json;

namespace Art;

/// <summary>
/// Provides artifact information.
/// </summary>
/// <param name="Key">Resource key.</param>
/// <param name="Version">Version.</param>
/// <param name="Properties">Resource properties.</param>
public record ArtifactResourceInfo(ArtifactResourceKey Key, string? Version, IReadOnlyDictionary<string, JsonElement> Properties)
{
    /// <summary>
    /// Creates a new instance of <see cref="ArtifactResourceInfo"/>.
    /// </summary>
    /// <param name="key">Artifact key.</param>
    /// <param name="file">Filename.</param>
    /// <param name="path">Path.</param>
    /// <param name="inArtifactFolder">If false, sent to common directory.</param>
    /// <param name="version">Version.</param>
    /// <param name="properties">Resource properties.</param>
    /// <returns>Value.</returns>
    public static ArtifactResourceInfo Create(ArtifactKey key, string file, string? path = null, bool inArtifactFolder = true, string? version = null, IReadOnlyDictionary<string, JsonElement>? properties = null)
        => new(ArtifactResourceKey.Create(key, file, path, inArtifactFolder), version, properties ?? EmptyProperties);

    /// <summary>
    /// Creates a new instance of <see cref="ArtifactResourceInfo"/>.
    /// </summary>
    /// <param name="key">Resource key.</param>
    /// <param name="version">Version.</param>
    /// <param name="properties">Resource properties.</param>
    /// <returns>Value.</returns>
    public static ArtifactResourceInfo Create(ArtifactResourceKey key, string? version = null, IReadOnlyDictionary<string, JsonElement>? properties = null)
        => new(key, version, properties ?? EmptyProperties);

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
