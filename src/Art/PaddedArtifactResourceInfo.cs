namespace Art;

/// <summary>
/// Represents a resource with padding.
/// </summary>
/// <param name="PaddingMode">Padding mode.</param>
/// <param name="BaseArtifactResourceInfo">Base resource.</param>
public record PaddedArtifactResourceInfo(PaddingMode PaddingMode, ArtifactResourceInfo BaseArtifactResourceInfo) : ArtifactResourceInfo(BaseArtifactResourceInfo.Key, BaseArtifactResourceInfo.ContentType, BaseArtifactResourceInfo.Updated, BaseArtifactResourceInfo.Version, BaseArtifactResourceInfo.Checksum)
{
    /// <inheritdoc/>
    public override bool Exportable => BaseArtifactResourceInfo.Exportable;

    /// <inheritdoc/>
    public override async ValueTask ExportStreamAsync(Stream targetStream, CancellationToken cancellationToken = default)
    {
        // TODO add additional streaming de-padding implementations
        switch (PaddingMode)
        {
            case PaddingMode.Pkcs7 when BaseArtifactResourceInfo is EncryptedArtifactResourceInfo ex:
                await ExportStreamWithDepaddingHandlerAsync(new Pkcs7DepaddingHandler(ex.EncryptionInfo.GetBlockSize() / 8), targetStream, cancellationToken).ConfigureAwait(false);
                break;
            case PaddingMode.Pkcs5 when BaseArtifactResourceInfo is EncryptedArtifactResourceInfo ex:
                await ExportStreamWithDepaddingHandlerAsync(new Pkcs5DepaddingHandler(ex.EncryptionInfo.GetBlockSize() / 8), targetStream, cancellationToken).ConfigureAwait(false);
                break;
            default:
                MemoryStream stream = new();
                if (!stream.TryGetBuffer(out ArraySegment<byte> buf) || buf.Array == null) throw new InvalidOperationException("unpoggers");
                await BaseArtifactResourceInfo.ExportStreamAsync(stream, cancellationToken).ConfigureAwait(false);
                await targetStream.WriteAsync(buf.Array.AsMemory(0, Padding.GetDepaddedLength(buf, PaddingMode)), cancellationToken).ConfigureAwait(false);
                break;
        }
    }

    private async Task ExportStreamWithDepaddingHandlerAsync(DepaddingHandler handler, Stream targetStream, CancellationToken cancellationToken)
    {
        await using DepaddingStream ds = new(handler, targetStream, true);
        await BaseArtifactResourceInfo.ExportStreamAsync(ds, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override bool UsesMetadata => BaseArtifactResourceInfo.UsesMetadata;

    /// <inheritdoc/>
    public override async ValueTask<ArtifactResourceInfo> WithMetadataAsync(CancellationToken cancellationToken = default)
    {
        ArtifactResourceInfo b = await BaseArtifactResourceInfo.WithMetadataAsync(cancellationToken);
        return this with { BaseArtifactResourceInfo = b, ContentType = b.ContentType, Updated = b.Updated, Version = b.Version };
    }
}
