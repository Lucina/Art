using System.Text.Json;

namespace Art;

/// <summary>
/// Provides artifact information.
/// </summary>
/// <param name="Resource">Resource.</param>
/// <param name="SerializerOptions">Serializer options.</param>
/// <param name="Key">Resource key.</param>
/// <param name="Version">Version.</param>
/// <param name="Properties">Resource properties.</param>
public record JsonArtifactResourceInfo<T>(T Resource, JsonSerializerOptions? SerializerOptions, ArtifactResourceKey Key, string? Version, IReadOnlyDictionary<string, JsonElement> Properties)
    : ArtifactResourceInfo(Key, Version, Properties)
{
    /// <summary>
    /// Creates a new instance of <see cref="ArtifactInfo"/>.
    /// </summary>
    /// <param name="resource">Resource.</param>
    /// <param name="serializerOptions">Serializer options.</param>
    /// <param name="key">Resource key.</param>
    /// <param name="version">Version.</param>
    /// <param name="properties">Resource properties.</param>
    /// <returns>Value.</returns>
    public static JsonArtifactResourceInfo<T> Create(T resource, JsonSerializerOptions? serializerOptions, ArtifactResourceKey key, string? version = null, IReadOnlyDictionary<string, JsonElement>? properties = null)
        => new(resource, serializerOptions, key, version, properties ?? EmptyProperties);

    /// <inheritdoc/>
    public override bool Exportable => true;

    /// <inheritdoc/>
    public override async ValueTask ExportAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        await JsonSerializer.SerializeAsync<T>(stream, Resource, SerializerOptions, cancellationToken).ConfigureAwait(false);
    }
}
