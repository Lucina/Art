namespace Art;

/// <summary>
/// Represents a resource with version.
/// </summary>
/// <param name="VersionValue">Version.</param>
/// <param name="BaseArtifactResourceInfo">Base resource.</param>
public record WithVersionArtifactResourceInfo(string? VersionValue, ArtifactResourceInfo BaseArtifactResourceInfo) : ArtifactResourceInfo(BaseArtifactResourceInfo.Key, BaseArtifactResourceInfo.ContentType, BaseArtifactResourceInfo.Updated, VersionValue)
{
    /// <inheritdoc/>
    public override bool Exportable => BaseArtifactResourceInfo.Exportable;

    /// <inheritdoc/>
    public override ValueTask<Stream> ExportStreamAsync(CancellationToken cancellationToken = default)
        => BaseArtifactResourceInfo.ExportStreamAsync(cancellationToken);

    /// <inheritdoc/>
    public override bool UsesMetadata => BaseArtifactResourceInfo.UsesMetadata;

    /// <inheritdoc/>
    public override async ValueTask<ArtifactResourceInfo> WithMetadataAsync(CancellationToken cancellationToken = default)
    {
        ArtifactResourceInfo b = await BaseArtifactResourceInfo.WithMetadataAsync(cancellationToken);
        return this with { BaseArtifactResourceInfo = b, ContentType = b.ContentType, Updated = b.Updated, Version = VersionValue };
    }
}
