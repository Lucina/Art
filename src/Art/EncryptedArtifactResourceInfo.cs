namespace Art;

/// <summary>
/// Represents an encrypted resource.
/// </summary>
/// <param name="EncryptionInfo">Encryption information.</param>
/// <param name="BaseArtifactResourceInfo">Base resource.</param>
public record EncryptedArtifactResourceInfo(EncryptionInfo EncryptionInfo, ArtifactResourceInfo BaseArtifactResourceInfo)
    : ArtifactResourceInfo(BaseArtifactResourceInfo.Key, BaseArtifactResourceInfo.Version)
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
    public override bool VersionQueryable => BaseArtifactResourceInfo.VersionQueryable;

    /// <inheritdoc/>
    public override ValueTask<string?> QueryVersionAsync(CancellationToken cancellationToken = default) => BaseArtifactResourceInfo.QueryVersionAsync(cancellationToken);
}
