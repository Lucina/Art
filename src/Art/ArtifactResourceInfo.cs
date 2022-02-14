using System.Text.Json.Serialization;

namespace Art;

/// <summary>
/// Provides artifact information.
/// </summary>
/// <param name="Key">Resource key.</param>
/// <param name="ContentType">MIME content type.</param>
/// <param name="Updated">Updated date.</param>
/// <param name="Version">Version.</param>
/// <param name="Checksum">Checksum.</param>
public record ArtifactResourceInfo(ArtifactResourceKey Key, string? ContentType = "application/octet-stream", DateTimeOffset? Updated = null, string? Version = null, Checksum? Checksum = null)
{
    /// <summary>
    /// Checks if non-identifying metadata (i.e. everything but key, updated date, version) is different.
    /// </summary>
    /// <param name="other">Resource to compare to.</param>
    /// <returns>True if any metadata is different or if other is null.</returns>
    public bool IsMetadataDifferent(ArtifactResourceInfo? other)
    {
        if (other == null) return true;
        return ContentType != other.ContentType;
    }

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
    /// <param name="targetStream">Stream to write resource contents to.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="NotSupportedException">Thrown if this instance cannot be exported.</exception>
    public virtual ValueTask ExportStreamAsync(Stream targetStream, CancellationToken cancellationToken = default)
        => throw new NotSupportedException($"This is a raw instance of {nameof(ArtifactResourceInfo)} that is not exportable");

    /// <summary>
    /// Gets this resource with associated metadata, if available.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Resource with metadata if available.</returns>
    public virtual ValueTask<ArtifactResourceInfo> WithMetadataAsync(CancellationToken cancellationToken = default)
        => ValueTask.FromResult(this);

    /// <summary>
    /// Gets informational path string.
    /// </summary>
    /// <returns>Info path string.</returns>
    public virtual string GetInfoPathString() => $"{ArtifactTool.Combine("/", Key.Path, Key.File)}";

    /// <summary>
    /// Gets informational string.
    /// </summary>
    /// <returns>Info string.</returns>
    public virtual string GetInfoString() => $"Path: {ArtifactTool.Combine("/", Key.Path, Key.File)}{(ContentType != null ? $"\nContent type: {ContentType}" : "")}{(Updated != null ? $"\nUpdated: {Updated}" : "")}{(Version != null ? $"\nVersion: {Version}" : "")}{(Checksum != null ? $"\nChecksum: {Checksum.Id}:{Convert.ToHexString(Checksum.Value)}" : "")}";

    /// <summary>
    /// Converts info record to model.
    /// </summary>
    /// <param name="value">Record.</param>
    /// <returns>Model.</returns>
    public static implicit operator ArtifactResourceInfoModel(ArtifactResourceInfo value) =>
        new()
        {
            ArtifactTool = value.Key.Artifact.Tool,
            ArtifactGroup = value.Key.Artifact.Group,
            ArtifactId = value.Key.Artifact.Id,
            File = value.Key.File,
            Path = value.Key.Path,
            ContentType = value.ContentType,
            Updated = value.Updated,
            Version = value.Version,
            ChecksumId = value.Checksum?.Id,
            ChecksumValue = value.Checksum?.Value
        };
}
