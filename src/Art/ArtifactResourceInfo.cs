using System.Text.Json.Serialization;

namespace Art;

/// <summary>
/// Provides artifact information.
/// </summary>
/// <param name="Key">Resource key.</param>
/// <param name="ContentType">MIME content type.</param>
/// <param name="Updated">Updated date.</param>
/// <param name="Version">Version.</param>
public record ArtifactResourceInfo(ArtifactResourceKey Key, string? ContentType = "application/octet-stream", DateTimeOffset? Updated = null, string? Version = null)
{
    /// <summary>
    /// If true, this resource is a file-exportable reference.
    /// </summary>
    [JsonIgnore]
    public virtual bool Exportable => false;

    /// <summary>
    /// If true, this resource can be queried for additional metadata.
    /// </summary>
    [JsonIgnore]
    public virtual bool UsesMetadata => false;

    /// <summary>
    /// Exports a resource.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning stream.</returns>
    /// <exception cref="NotSupportedException">Thrown if this instance cannot be exported.</exception>
    public virtual ValueTask<Stream> ExportStreamAsync(CancellationToken cancellationToken = default)
        => throw new NotSupportedException($"This is a raw instance of {nameof(ArtifactResourceInfo)} that is not exportable");

    /// <summary>
    /// Gets this resource with associated metadata, if available.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Resource with metadata if available.</returns>
    public virtual ValueTask<ArtifactResourceInfo> WithMetadataAsync(CancellationToken cancellationToken = default)
        => ValueTask.FromResult(this);

    /// <summary>
    /// Gets informational string.
    /// </summary>
    /// <returns>Info string.</returns>
    public virtual string GetInfoString() => $"{ArtifactTool.Combine("/", Key.Path, Key.File)}{(Updated != null ? $" [{Updated}]" : "")}{(Version != null ? $" [{Version}]" : "")}";
}
