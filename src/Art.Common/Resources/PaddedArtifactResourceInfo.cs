using Art.Common.CipherPadding;
using Art.Common.Crypto;

namespace Art.Common.Resources;

/// <summary>
/// Represents a resource with padding.
/// </summary>
/// <param name="ArtPaddingMode">Padding mode.</param>
/// <param name="BlockSize">Block size, in bits.</param>
/// <param name="BaseArtifactResourceInfo">Base resource.</param>
public record PaddedArtifactResourceInfo(ArtPaddingMode ArtPaddingMode, int? BlockSize, ArtifactResourceInfo BaseArtifactResourceInfo) : ArtifactResourceInfo(BaseArtifactResourceInfo.Key, BaseArtifactResourceInfo.ContentType, BaseArtifactResourceInfo.Updated, BaseArtifactResourceInfo.Version, BaseArtifactResourceInfo.Checksum)
{
    /// <inheritdoc/>
    public override bool CanExportStream => BaseArtifactResourceInfo.CanExportStream;

    /// <inheritdoc />
    public override bool CanGetStream => BaseArtifactResourceInfo.CanGetStream;

    /// <inheritdoc/>
    public override ValueTask ExportStreamAsync(Stream targetStream, CancellationToken cancellationToken = default)
    {
        var dp = GetDepaddingHandler();
        return ExportStreamWithDepaddingHandlerAsync(dp, targetStream, cancellationToken);
    }

    /// <inheritdoc />
    public override async ValueTask<Stream> GetStreamAsync(CancellationToken cancellationToken = default)
    {
        var dp = GetDepaddingHandler();
        return new DepaddingReadStream(dp, await BaseArtifactResourceInfo.GetStreamAsync(cancellationToken).ConfigureAwait(false));
    }

    private DepaddingHandler GetDepaddingHandler()
    {
        GetParameters(out int? blockSizeBytesV);
        if (blockSizeBytesV is not { } blockSizeBytes) throw new InvalidOperationException("No block size provided");
        return ArtPaddingMode switch
        {
            ArtPaddingMode.Zero => new ZeroDepaddingHandler(blockSizeBytes),
            ArtPaddingMode.AnsiX9_23 => new AnsiX9_23DepaddingHandler(blockSizeBytes),
            ArtPaddingMode.Iso10126 => new Iso10126DepaddingHandler(blockSizeBytes),
            ArtPaddingMode.Pkcs7 => new Pkcs7DepaddingHandler(blockSizeBytes),
            ArtPaddingMode.Pkcs5 => new Pkcs5DepaddingHandler(blockSizeBytes),
            ArtPaddingMode.Iso_Iec_7816_4 => new Iso_Iec_7816_4DepaddingHandler(blockSizeBytes),
            _ => throw new InvalidOperationException("Invalid padding mode")
        };
    }

    private void GetParameters(out int? blockSizeBytes)
    {
        if (BlockSize is { } blockSize)
        {
            blockSizeBytes = blockSize / 8;
        }
        else if (BaseArtifactResourceInfo is EncryptedArtifactResourceInfo({ PaddingMode: System.Security.Cryptography.PaddingMode.None } encryptionInfo, _))
        {
            // Fallback to trying to retrieve block size from base encrypted resource
            // Only do this if the base resource isn't configured to pad (doesn't make sense to depad a depadded output)
            using var alg = encryptionInfo.CreateSymmetricAlgorithm();
            blockSizeBytes = alg.BlockSize / 8;
        }
        else
        {
            blockSizeBytes = null;
        }
    }

    private async ValueTask ExportStreamWithDepaddingHandlerAsync(DepaddingHandler handler, Stream targetStream, CancellationToken cancellationToken)
    {
        await using DepaddingWriteStream ds = new(handler, targetStream, true);
        await BaseArtifactResourceInfo.ExportStreamAsync(ds, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override bool UsesMetadata => BaseArtifactResourceInfo.UsesMetadata;

    /// <inheritdoc/>
    public override async ValueTask<ArtifactResourceInfo> WithMetadataAsync(CancellationToken cancellationToken = default)
    {
        ArtifactResourceInfo b = await BaseArtifactResourceInfo.WithMetadataAsync(cancellationToken).ConfigureAwait(false);
        return this with { BaseArtifactResourceInfo = b, ContentType = b.ContentType, Updated = b.Updated, Version = b.Version };
    }
}
