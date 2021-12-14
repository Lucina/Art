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
    public override async ValueTask ExportAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        using MemoryStream ms = new();
        await BaseArtifactResourceInfo.ExportAsync(ms, cancellationToken);
        ms.Position = 0;
        await EncryptionInfo.DecryptStreamAsync(ms, stream, cancellationToken);
    }

    /// <inheritdoc/>
    public override bool MetadataQueryable => BaseArtifactResourceInfo.MetadataQueryable;

    /// <inheritdoc/>
    public override ValueTask<ArtifactResourceInfo> WithMetadataAsync(CancellationToken cancellationToken = default) => BaseArtifactResourceInfo.WithMetadataAsync(cancellationToken);
}
