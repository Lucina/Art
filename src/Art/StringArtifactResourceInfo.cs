using System.Text;
using System.Text.Json;

namespace Art;

/// <summary>
/// Provides artifact information.
/// </summary>
/// <param name="Resource">Resource.</param>
/// <param name="Key">Resource key.</param>
/// <param name="Version">Version.</param>
/// <param name="Properties">Resource properties.</param>
public record StringArtifactResourceInfo(string Resource, ArtifactResourceKey Key, string? Version, IReadOnlyDictionary<string, JsonElement> Properties)
    : ArtifactResourceInfo(Key, Version, Properties)
{
    /// <summary>
    /// Creates a new instance of <see cref="StringArtifactResourceInfo"/>.
    /// </summary>
    /// <param name="resource">Resource.</param>
    /// <param name="key">Resource key.</param>
    /// <param name="version">Version.</param>
    /// <param name="properties">Resource properties.</param>
    /// <returns>Value.</returns>
    public static StringArtifactResourceInfo Create(string resource, ArtifactResourceKey key, string? version = null, IReadOnlyDictionary<string, JsonElement>? properties = null)
        => new(resource, key, version, properties ?? EmptyProperties);

    /// <inheritdoc/>
    public override bool Exportable => true;

    /// <inheritdoc/>
    public override async ValueTask ExportAsync(Stream stream)
    {
        using var sw = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);
        await sw.WriteAsync(Resource).ConfigureAwait(false);
    }
}
