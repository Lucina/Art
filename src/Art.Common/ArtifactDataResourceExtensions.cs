using System.Security.Cryptography;
using Art.Common.Resources;
using Art.Crypto;

namespace Art.Common;

/// <summary>
/// Provides extensions for <see cref="ArtifactDataResource"/>.
/// </summary>
public static class ArtifactDataResourceExtensions
{
    /// <summary>
    /// Creates an instance of this resource with a specific content type.
    /// </summary>
    /// <param name="resource">Resource to augment.</param>
    /// <param name="contentType">Content type.</param>
    /// <returns>Resource with specific content type.</returns>
    public static ArtifactDataResource WithContentType(this ArtifactDataResource resource, string? contentType = null)
    {
        return resource with { Info = new WithContentTypeArtifactResourceInfo(contentType, resource.Info) };
    }

    /// <summary>
    /// Creates an instance of this resource with a specific updated date.
    /// </summary>
    /// <param name="resource">Resource to augment.</param>
    /// <param name="updated">Updated date.</param>
    /// <returns>Resource with specific updated date.</returns>
    public static ArtifactDataResource WithUpdated(this ArtifactDataResource resource, DateTimeOffset? updated = null)
    {
        return resource with { Info = new WithUpdatedArtifactResourceInfo(updated, resource.Info) };
    }

    /// <summary>
    /// Creates an instance of this resource with a specific version.
    /// </summary>
    /// <param name="resource">Resource to augment.</param>
    /// <param name="version">Version.</param>
    /// <returns>Resource with specific version.</returns>
    public static ArtifactDataResource WithVersion(this ArtifactDataResource resource, string? version = null)
    {
        return resource with { Info = new WithVersionArtifactResourceInfo(version, resource.Info) };
    }

    /// <summary>
    /// Creates an instance of this resource with an added decryption layer.
    /// </summary>
    /// <param name="resource">Resource to augment.</param>
    /// <param name="encryptionInfo">Encryption information.</param>
    /// <returns>Decrypting resource.</returns>
    public static ArtifactDataResource WithEncryption(this ArtifactDataResource resource, EncryptionInfo encryptionInfo)
    {
        return resource with { Info = new EncryptedArtifactResourceInfo(encryptionInfo, resource.Info) };
    }

    /// <summary>
    /// Creates an instance of this resource with an added decryption layer.
    /// </summary>
    /// <param name="resource">Resource to augment.</param>
    /// <param name="algorithm">Algorithm.</param>
    /// <param name="encKey">Key.</param>
    /// <param name="keySize">Key size.</param>
    /// <param name="blockSize">Block size.</param>
    /// <param name="mode">Cipher mode.</param>
    /// <param name="encIv">IV.</param>
    /// <param name="paddingMode">Padding mode.</param>
    /// <returns>Decrypting resource.</returns>
    public static ArtifactDataResource WithEncryption(this ArtifactDataResource resource, CryptoAlgorithm algorithm, ReadOnlyMemory<byte> encKey, CipherMode? mode = null, int? keySize = null, int? blockSize = null, ReadOnlyMemory<byte>? encIv = null, PaddingMode? paddingMode = null)
    {
        return resource with { Info = new EncryptedArtifactResourceInfo(new EncryptionInfo(algorithm, encKey, mode, keySize, blockSize, encIv, paddingMode), resource.Info) };
    }

    /// <summary>
    /// Creates an instance of this resource with an added depadding layer.
    /// </summary>
    /// <param name="resource">Resource to augment.</param>
    /// <param name="artPaddingMode">Padding mode.</param>
    /// <param name="blockSize">Block size, in bits.</param>
    /// <returns>Depadded resource.</returns>
    public static ArtifactDataResource WithPadding(this ArtifactDataResource resource, ArtPaddingMode artPaddingMode, int? blockSize = null)
    {
        return resource with { Info = new PaddedArtifactResourceInfo(artPaddingMode, blockSize, resource.Info) };
    }
}
