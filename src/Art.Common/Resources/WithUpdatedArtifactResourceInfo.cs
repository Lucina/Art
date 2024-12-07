﻿namespace Art.Common.Resources;

/// <summary>
/// Represents a resource with version.
/// </summary>
/// <param name="UpdatedValue">Updated date.</param>
/// <param name="BaseArtifactResourceInfo">Base resource.</param>
public record WithUpdatedArtifactResourceInfo(DateTimeOffset? UpdatedValue, ArtifactResourceInfo BaseArtifactResourceInfo)
    : ArtifactResourceInfo(
        BaseArtifactResourceInfo.Key,
        BaseArtifactResourceInfo.ContentType,
        UpdatedValue,
        BaseArtifactResourceInfo.Retrieved,
        BaseArtifactResourceInfo.Version)
{
    /// <inheritdoc/>
    public override bool CanExportStream => BaseArtifactResourceInfo.CanExportStream;

    /// <inheritdoc />
    public override bool CanGetStream => BaseArtifactResourceInfo.CanGetStream;

    /// <inheritdoc/>
    public override ValueTask ExportStreamAsync(Stream targetStream, CancellationToken cancellationToken = default)
        => BaseArtifactResourceInfo.ExportStreamAsync(targetStream, cancellationToken);

    /// <inheritdoc/>
    public override ValueTask<Stream> GetStreamAsync(CancellationToken cancellationToken = default)
        => BaseArtifactResourceInfo.GetStreamAsync(cancellationToken);

    /// <inheritdoc/>
    public override bool UsesMetadata => BaseArtifactResourceInfo.UsesMetadata;

    /// <inheritdoc/>
    public override async ValueTask<ArtifactResourceInfo> WithMetadataAsync(CancellationToken cancellationToken = default)
    {
        ArtifactResourceInfo b = await BaseArtifactResourceInfo.WithMetadataAsync(cancellationToken).ConfigureAwait(false);
        return this with
        {
            BaseArtifactResourceInfo = b,
            ContentType = b.ContentType,
            Updated = UpdatedValue,
            Version = b.Version,
            Checksum = b.Checksum
        };
    }

    /// <inheritdoc />
    public override void AugmentOutputStreamOptions(ref OutputStreamOptions options)
        => BaseArtifactResourceInfo.AugmentOutputStreamOptions(ref options);
}
