namespace Art;

/// <summary>
/// Represents a resource with checksum.
/// </summary>
/// <param name="ChecksumValue">Checksum.</param>
/// <param name="BaseArtifactResourceInfo">Base resource.</param>
public record WithChecksumArtifactResourceInfo(Checksum? ChecksumValue, ArtifactResourceInfo BaseArtifactResourceInfo) : ArtifactResourceInfo(BaseArtifactResourceInfo.Key, BaseArtifactResourceInfo.ContentType, BaseArtifactResourceInfo.Updated, BaseArtifactResourceInfo.Version, ChecksumValue)
{
    /// <inheritdoc/>
    public override bool Exportable => BaseArtifactResourceInfo.Exportable;

    /// <inheritdoc/>
    public override ValueTask ExportStreamAsync(Stream targetStream, CancellationToken cancellationToken = default)
        => BaseArtifactResourceInfo.ExportStreamAsync(targetStream, cancellationToken);

    /// <inheritdoc/>
    public override bool UsesMetadata => BaseArtifactResourceInfo.UsesMetadata;

    /// <inheritdoc/>
    public override async ValueTask<ArtifactResourceInfo> WithMetadataAsync(CancellationToken cancellationToken = default)
    {
        ArtifactResourceInfo b = await BaseArtifactResourceInfo.WithMetadataAsync(cancellationToken);
        return this with
        {
            BaseArtifactResourceInfo = b,
            ContentType = b.ContentType,
            Updated = b.Updated,
            Version = b.Version,
            Checksum = ChecksumValue
        };
    }
}
