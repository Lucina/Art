namespace Art;

/// <summary>
/// Represents an encrypted resource.
/// </summary>
/// <param name="EncryptionInfo">Encryption information.</param>
/// <param name="BaseArtifactResourceInfo">Base resource.</param>
public record EncryptedArtifactResourceInfo(EncryptionInfo EncryptionInfo, ArtifactResourceInfo BaseArtifactResourceInfo)
    : ArtifactResourceInfo(BaseArtifactResourceInfo.Key, BaseArtifactResourceInfo.Version, BaseArtifactResourceInfo.Properties)
{
    /// <summary>
    /// Creates a new instance of <see cref="EncryptedArtifactResourceInfo"/>.
    /// </summary>
    /// <param name="encryptionInfo">Encryption information.</param>
    /// <param name="baseArtifactResourceInfo">Base resource.</param>
    /// <returns>Value.</returns>
    public static EncryptedArtifactResourceInfo Create(EncryptionInfo encryptionInfo, ArtifactResourceInfo baseArtifactResourceInfo)
        => new(encryptionInfo, baseArtifactResourceInfo);

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
    public override bool Queryable => BaseArtifactResourceInfo.Queryable;

    /// <inheritdoc/>
    public override ValueTask<string?> QueryVersionAsync(CancellationToken cancellationToken = default) => BaseArtifactResourceInfo.QueryVersionAsync(cancellationToken);
}
