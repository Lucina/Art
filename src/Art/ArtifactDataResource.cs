using System.Security.Cryptography;

namespace Art;

/// <summary>
/// Pending <see cref="ArtifactResourceInfo"/> relative to a <see cref="ArtifactData"/>.
/// </summary>
public record struct ArtifactDataResource(ArtifactData Data, ArtifactResourceInfo Info)
{
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
    /// <returns>Decrypting resource.</returns>
    public ArtifactDataResource WithEncryption(CryptoAlgorithm algorithm, ReadOnlyMemory<byte> encKey, CipherMode? mode = null, int? keySize = null, int? blockSize = null, ReadOnlyMemory<byte>? encIv = null)
        => this with { Info = new EncryptedArtifactResourceInfo(new EncryptionInfo(algorithm, encKey, mode, keySize, blockSize, encIv), Info) };

    /// <summary>
    /// Creates an instance of this resource with an added depadding layer.
    /// </summary>
    /// <param name="paddingMode">Padding mode.</param>
    /// <returns>Decrypting resource.</returns>
    public ArtifactDataResource WithPadding(PaddingMode paddingMode)
        => this with { Info = new PaddedArtifactResourceInfo(paddingMode, Info) };

    /// <summary>
    /// Adds this resource.
    /// </summary>
    public void Commit() => Data.Add(Info);
}
