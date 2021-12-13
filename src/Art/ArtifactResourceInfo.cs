using System.Text.Json.Serialization;

namespace Art;

/// <summary>
/// Provides artifact information.
/// </summary>
/// <param name="Key">Resource key.</param>
/// <param name="Version">Version.</param>
public record ArtifactResourceInfo(ArtifactResourceKey Key, string? Version = null)
{
    /// <summary>
    /// If true, this resource is a file-exportable reference.
    /// </summary>
    [JsonIgnore]
    public virtual bool Exportable => false;

    /// <summary>
    /// If true, this resource can be queried for version.
    /// </summary>
    [JsonIgnore]
    public virtual bool VersionQueryable => false;

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
        if (Version == null && VersionQueryable) return this with { Version = await QueryVersionAsync(cancellationToken).ConfigureAwait(false) };
        return this;
    }
}
