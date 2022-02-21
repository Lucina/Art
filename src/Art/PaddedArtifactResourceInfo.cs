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
        MemoryStream stream = new();
        await BaseArtifactResourceInfo.ExportStreamAsync(stream, cancellationToken).ConfigureAwait(false);
        if (!stream.TryGetBuffer(out ArraySegment<byte> buf) || buf.Array == null) throw new InvalidOperationException("unpoggers");
        // TODO some better streaming-only method to depad
        await targetStream.WriteAsync(buf.Array.AsMemory(0, Padding.GetDepaddedLength(buf, PaddingMode)), cancellationToken).ConfigureAwait(false);
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
