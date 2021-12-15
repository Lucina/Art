namespace Art;

/// <summary>
/// Represents a resource with padding.
/// </summary>
/// <param name="PaddingMode">Padding mode.</param>
/// <param name="BaseArtifactResourceInfo">Base resource.</param>
public record PaddedArtifactResourceInfo(PaddingMode PaddingMode, ArtifactResourceInfo BaseArtifactResourceInfo) : ArtifactResourceInfo(BaseArtifactResourceInfo.Key, BaseArtifactResourceInfo.ContentType, BaseArtifactResourceInfo.Updated, BaseArtifactResourceInfo.Version)
{
    /// <inheritdoc/>
    public override bool Exportable => BaseArtifactResourceInfo.Exportable;

    /// <inheritdoc/>
    public override async ValueTask<Stream> ExportStreamAsync(CancellationToken cancellationToken = default)
    {
        await using Stream baseStream = await BaseArtifactResourceInfo.ExportStreamAsync(cancellationToken).ConfigureAwait(false);
        MemoryStream stream = new();
        await baseStream.CopyToAsync(stream, cancellationToken);
        stream.Position = 0;
        if (!stream.TryGetBuffer(out ArraySegment<byte> buf)) throw new InvalidOperationException("unpoggers");
        stream.SetLength(Padding.GetDepaddedLength(buf, PaddingMode));
        return stream;
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
