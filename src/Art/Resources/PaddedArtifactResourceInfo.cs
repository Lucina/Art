using Art.CipherPadding;
using Art.Crypto;

namespace Art.Resources;

/// <summary>
/// Represents a resource with padding.
/// </summary>
/// <param name="PaddingMode">Padding mode.</param>
/// <param name="BlockSize">Block size, in bits.</param>
/// <param name="BaseArtifactResourceInfo">Base resource.</param>
public record PaddedArtifactResourceInfo(PaddingMode PaddingMode, int? BlockSize, ArtifactResourceInfo BaseArtifactResourceInfo) : ArtifactResourceInfo(BaseArtifactResourceInfo.Key, BaseArtifactResourceInfo.ContentType, BaseArtifactResourceInfo.Updated, BaseArtifactResourceInfo.Version, BaseArtifactResourceInfo.Checksum)
{
    /// <inheritdoc/>
    public override bool Exportable => BaseArtifactResourceInfo.Exportable;

    /// <inheritdoc/>
    public override async ValueTask ExportStreamAsync(Stream targetStream, CancellationToken cancellationToken = default)
    {
        GetParameters(out int? blockSizeBytesV);
        if (blockSizeBytesV is not { } blockSizeBytes) throw new InvalidOperationException("No block size provided");
        DepaddingHandler dp = PaddingMode switch
        {
            PaddingMode.Zero => new ZeroDepaddingHandler(blockSizeBytes),
            PaddingMode.AnsiX9_23 => new AnsiX9_23DepaddingHandler(blockSizeBytes),
            PaddingMode.Iso10126 => new Iso10126DepaddingHandler(blockSizeBytes),
            PaddingMode.Pkcs7 => new Pkcs7DepaddingHandler(blockSizeBytes),
            PaddingMode.Pkcs5 => new Pkcs5DepaddingHandler(blockSizeBytes),
            PaddingMode.Iso_Iec_7816_4 => new Iso_Iec_7816_4DepaddingHandler(blockSizeBytes),
            _ => throw new InvalidOperationException("Invalid padding mode")
        };
        await ExportStreamWithDepaddingHandlerAsync(dp, targetStream, cancellationToken).ConfigureAwait(false);
    }

    private void GetParameters(out int? blockSizeBytes)
    {
        blockSizeBytes = BlockSize is { } blockSize ? blockSize / 8 : null;
        if (BaseArtifactResourceInfo is EncryptedArtifactResourceInfo({ } encryptionInfo, _))
        {
            using var alg = encryptionInfo.CreateSymmetricAlgorithm();
            if (alg.Padding == System.Security.Cryptography.PaddingMode.None) // Pass through block size if source didn't pad
                blockSizeBytes ??= alg.BlockSize / 8;
            // It's possible for base to specify its own padding method and actual data was padded even before that (nest), so allow this kind of stuff too
        }
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
