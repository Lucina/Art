using System.Security.Cryptography;

namespace Art;

/// <summary>
/// Crypto algorithm.
/// </summary>
/// <param name="Algorithm">Algorithm.</param>
/// <param name="EncKey">Key.</param>
/// <param name="KeySize">Key size, in bits.</param>
/// <param name="BlockSize">Block size, in bits.</param>
/// <param name="Mode">Cipher mode.</param>
/// <param name="EncIv">IV.</param>
public record EncryptionInfo(CryptoAlgorithm Algorithm, ReadOnlyMemory<byte> EncKey, CipherMode? Mode = null, int? KeySize = null, int? BlockSize = null, ReadOnlyMemory<byte>? EncIv = null)
{
    /// <summary>
    /// Empty 128-bit buffer.
    /// </summary>
    public static readonly ReadOnlyMemory<byte> Empty128 = new byte[128 / 8];

    /// <summary>
    /// Empty 192-bit buffer.
    /// </summary>
    public static readonly ReadOnlyMemory<byte> Empty192 = new byte[192 / 8];

    /// <summary>
    /// Empty 256-bit buffer.
    /// </summary>
    public static readonly ReadOnlyMemory<byte> Empty256 = new byte[256 / 8];

    /// <summary>
    /// Decrypts a source stream.
    /// </summary>
    /// <param name="stream">Source stream.</param>
    /// <param name="outStream">Output stream.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task DecryptStreamAsync(Stream stream, Stream outStream, CancellationToken cancellationToken = default)
    {
        using SymmetricAlgorithm alg = Algorithm switch
        {
            CryptoAlgorithm.Aes => Aes.Create(),
            CryptoAlgorithm.Blowfish => new BlowfishSymmetricAlgorithm(),
            CryptoAlgorithm.Xor => new XorSymmetricAlgorithm(),
            CryptoAlgorithm.DES => DES.Create(),
            CryptoAlgorithm.TripleDES => TripleDES.Create(),
            CryptoAlgorithm.RC2 => RC2.Create(),
            _ => throw new NotSupportedException(Algorithm.ToString()),
        };
        if (KeySize is { } keySize)
            alg.KeySize = keySize;
        if (BlockSize is { } blockSize)
            alg.BlockSize = blockSize;
        if (Mode is { } mode)
            alg.Mode = mode;
        alg.Key = EncKey.ToArray();
        if (EncIv is { } encIv)
            alg.IV = encIv.ToArray();
        using ICryptoTransform dec = alg.CreateDecryptor();
        await using CryptoStream cryptoStream = new(stream, dec, CryptoStreamMode.Read, true);
        await cryptoStream.CopyToAsync(outStream, cancellationToken).ConfigureAwait(false);
    }
}
