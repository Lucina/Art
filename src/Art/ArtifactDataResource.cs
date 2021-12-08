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
    /// <param name="EncryptionInfo">Encryption information.</param>
    /// <returns>Decrypting resource.</returns>
    public ArtifactDataResource WithEncryption(EncryptionInfo EncryptionInfo) => this with { Info = new EncryptedArtifactResourceInfo(EncryptionInfo, Info) };

    /// <summary>
    /// Creates an instance of this resource with an added decryption layer.
    /// </summary>
    /// <param name="algorithm">Algorithm.</param>
    /// <param name="encKey">Key.</param>
    /// <param name="keySize">Key size.</param>
    /// <param name="mode">Cipher mode.</param>
    /// <param name="encIv">IV.</param>
    /// <returns>Decrypting resource.</returns>
    public ArtifactDataResource WithEncryption(CryptoAlgorithm algorithm, ReadOnlyMemory<byte> encKey, CipherMode? mode = null, int? keySize = null, ReadOnlyMemory<byte>? encIv = null)
        => this with { Info = new EncryptedArtifactResourceInfo(new EncryptionInfo(algorithm, encKey, mode, keySize, encIv), Info) };

    /// <summary>
    /// Adds this resource.
    /// </summary>
    public void Commit() => Data.Add(Info);
}
