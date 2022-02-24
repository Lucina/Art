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
        if (BaseArtifactResourceInfo is EncryptedArtifactResourceInfo({ } encryptionInfo, _))
        {
            int blockSize = encryptionInfo.GetBlockSize() / 8;
            DepaddingHandler? dp = PaddingMode switch
            {
                PaddingMode.Zero => new ZeroDepaddingHandler(blockSize),
                PaddingMode.AnsiX9_23 => new AnsiX9_23DepaddingHandler(blockSize),
                PaddingMode.Iso10126 => new Iso10126DepaddingHandler(blockSize),
                PaddingMode.Pkcs7 => new Pkcs7DepaddingHandler(blockSize),
                PaddingMode.Pkcs5 => new Pkcs5DepaddingHandler(blockSize),
                PaddingMode.Iso_Iec_7816_4 => new Iso_Iec_7816_4DepaddingHandler(blockSize),
                _ => null
            };
            if (dp != null) await ExportStreamWithDepaddingHandlerAsync(dp, targetStream, cancellationToken).ConfigureAwait(false);
        }
        MemoryStream stream = new();
        if (!stream.TryGetBuffer(out ArraySegment<byte> buf) || buf.Array == null) throw new InvalidOperationException("unpoggers");
        await BaseArtifactResourceInfo.ExportStreamAsync(stream, cancellationToken).ConfigureAwait(false);
        await targetStream.WriteAsync(buf.Array.AsMemory(0, Padding.GetDepaddedLength(buf, PaddingMode)), cancellationToken).ConfigureAwait(false);
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
