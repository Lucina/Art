using System.Text.Json;
using System.Text.Json.Serialization;

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
    [JsonIgnore]
    public virtual bool Exportable => false;

    /// <summary>
    /// If true, this resource can be queried for version.
    /// </summary>
    [JsonIgnore]
    public virtual bool Queryable => false;

    /// <summary>
    /// Exports a resource.
    /// </summary>
    /// <param name="stream">Stream to export to.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    /// <exception cref="NotSupportedException">Thrown if this instance cannot be exported.</exception>
    public virtual ValueTask ExportAsync(Stream stream, CancellationToken cancellationToken = default)
        => throw new NotSupportedException($"This is a raw instance of {nameof(ArtifactResourceInfo)} that is not exportable");

    /// <summary>
    /// Queries a resource for version.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning version or null.</returns>
    /// <exception cref="NotSupportedException">Thrown if this instance cannot be queried.</exception>
    public virtual ValueTask<string?> QueryVersionAsync(CancellationToken cancellationToken = default)
        => throw new NotSupportedException($"This is a raw instance of {nameof(ArtifactResourceInfo)} that is not queryable");

    /// <summary>
    /// Gets this resource with associated version, if available.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Resource with version if available.</returns>
    public async ValueTask<ArtifactResourceInfo> GetVersionedAsync(CancellationToken cancellationToken = default)
    {
        if (Version == null && Queryable) return this with { Version = await QueryVersionAsync(cancellationToken).ConfigureAwait(false) };
        return this;
    }
}
