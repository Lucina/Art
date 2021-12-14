namespace Art;

/// <summary>
/// Represents an encrypted resource.
/// </summary>
/// <param name="EncryptionInfo">Encryption information.</param>
/// <param name="BaseArtifactResourceInfo">Base resource.</param>
public record EncryptedArtifactResourceInfo(EncryptionInfo EncryptionInfo, ArtifactResourceInfo BaseArtifactResourceInfo)
    : ArtifactResourceInfo(BaseArtifactResourceInfo.Key, BaseArtifactResourceInfo.Updated, BaseArtifactResourceInfo.Version)
{
    /// <inheritdoc/>
    public override bool Exportable => BaseArtifactResourceInfo.Exportable;

    /// <inheritdoc/>
    public override async ValueTask<Stream> ExportStreamAsync(CancellationToken cancellationToken = default)
    {
        await using Stream baseStream = await BaseArtifactResourceInfo.ExportStreamAsync(cancellationToken).ConfigureAwait(false);
        Stream stream = new MemoryStream();
        await EncryptionInfo.DecryptStreamAsync(baseStream, stream, cancellationToken).ConfigureAwait(false);
        return stream;
    }

    /// <inheritdoc/>
    public override bool MetadataQueryable => BaseArtifactResourceInfo.MetadataQueryable;

    /// <inheritdoc/>
    public override ValueTask<ArtifactResourceInfo> WithMetadataAsync(CancellationToken cancellationToken = default) => BaseArtifactResourceInfo.WithMetadataAsync(cancellationToken);
}
