namespace Art.Common.Resources;

/// <summary>
/// Represents a resource with content type.
/// </summary>
/// <param name="ContentTypeValue">Content type.</param>
/// <param name="BaseArtifactResourceInfo">Base resource.</param>
public record WithContentTypeArtifactResourceInfo(string? ContentTypeValue, ArtifactResourceInfo BaseArtifactResourceInfo) : ArtifactResourceInfo(BaseArtifactResourceInfo.Key, ContentTypeValue, BaseArtifactResourceInfo.Updated, BaseArtifactResourceInfo.Version)
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
        ArtifactResourceInfo b = await BaseArtifactResourceInfo.WithMetadataAsync(cancellationToken).ConfigureAwait(false);
        return this with
        {
            BaseArtifactResourceInfo = b,
            ContentType = ContentTypeValue,
            Updated = b.Updated,
            Version = b.Version,
            Checksum = b.Checksum
        };
    }

    /// <inheritdoc />
    public override void AugmentOutputStreamOptions(ref OutputStreamOptions options)
        => BaseArtifactResourceInfo.AugmentOutputStreamOptions(ref options);
}
