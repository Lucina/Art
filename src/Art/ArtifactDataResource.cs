using System.Security.Cryptography;

namespace Art;

/// <summary>
/// Pending <see cref="ArtifactResourceInfo"/> relative to a <see cref="ArtifactData"/>.
/// </summary>
public readonly record struct ArtifactDataResource(ArtifactData Data, ArtifactResourceInfo Info)
{
    /// <summary>
    /// Creates an instance of this resource with a specific content type.
    /// </summary>
    /// <param name="contentType">Content type.</param>
    /// <returns>Resource with specific content type.</returns>
    public ArtifactDataResource WithContentType(string? contentType = null)
        => this with { Info = new WithContentTypeArtifactResourceInfo(contentType, Info) };

    /// <summary>
    /// Creates an instance of this resource with a specific updated date.
    /// </summary>
    /// <param name="updated">Updated date.</param>
    /// <returns>Resource with specific updated date.</returns>
    public ArtifactDataResource WithUpdated(DateTimeOffset? updated = null)
        => this with { Info = new WithUpdatedArtifactResourceInfo(updated, Info) };

    /// <summary>
    /// Creates an instance of this resource with a specific version.
    /// </summary>
    /// <param name="version">Version.</param>
    /// <returns>Resource with specific version.</returns>
    public ArtifactDataResource WithVersion(string? version = null)
        => this with { Info = new WithVersionArtifactResourceInfo(version, Info) };

    /// <summary>
    /// Creates an instance of this resource with an added decryption layer.
    /// </summary>
    /// <param name="encryptionInfo">Encryption information.</param>
    /// <returns>Decrypting resource.</returns>
    public ArtifactDataResource WithEncryption(EncryptionInfo encryptionInfo) => this with { Info = new EncryptedArtifactResourceInfo(encryptionInfo, Info) };

    /// <summary>
    /// Creates an instance of this resource with an added decryption layer.
    /// </summary>
    /// <param name="algorithm">Algorithm.</param>
    /// <param name="encKey">Key.</param>
    /// <param name="keySize">Key size.</param>
    /// <param name="blockSize">Block size.</param>
    /// <param name="mode">Cipher mode.</param>
    /// <param name="encIv">IV.</param>
    /// <param name="paddingMode">Padding mode.</param>
    /// <returns>Decrypting resource.</returns>
    public ArtifactDataResource WithEncryption(CryptoAlgorithm algorithm, ReadOnlyMemory<byte> encKey, CipherMode? mode = null, int? keySize = null, int? blockSize = null, ReadOnlyMemory<byte>? encIv = null, System.Security.Cryptography.PaddingMode? paddingMode = null)
        => this with { Info = new EncryptedArtifactResourceInfo(new EncryptionInfo(algorithm, encKey, mode, keySize, blockSize, encIv, paddingMode), Info) };

    /// <summary>
    /// Creates an instance of this resource with an added depadding layer.
    /// </summary>
    /// <param name="paddingMode">Padding mode.</param>
    /// <param name="blockSize">Block size, in bits.</param>
    /// <returns>Depadded resource.</returns>
    public ArtifactDataResource WithPadding(PaddingMode paddingMode, int? blockSize = null)
        => this with { Info = new PaddedArtifactResourceInfo(paddingMode, blockSize, Info) };

    /// <summary>
    /// Adds this resource.
    /// </summary>
    public void Commit() => Data.Add(Info);
}
