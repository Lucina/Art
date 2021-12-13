using System.Security.Cryptography;

namespace Art;

/// <summary>
/// Crypto algorithm.
/// </summary>
/// <param name="Algorithm">Algorithm.</param>
/// <param name="EncKey">Key.</param>
/// <param name="KeySize">Key size.</param>
/// <param name="Mode">Cipher mode.</param>
/// <param name="EncIv">IV.</param>
public record EncryptionInfo(CryptoAlgorithm Algorithm, ReadOnlyMemory<byte> EncKey, CipherMode? Mode = null, int? KeySize = null, ReadOnlyMemory<byte>? EncIv = null)
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
            CryptoAlgorithm.Xor => new XorSymmetricAlgorithm(),
            _ => throw new NotSupportedException(Algorithm.ToString()),
        };
        if (KeySize is { } keySize)
            alg.KeySize = keySize;
        if (Mode is { } mode)
            alg.Mode = mode;
        alg.Key = EncKey.ToArray();
        if (EncIv is { } encIv)
            alg.IV = encIv.ToArray();
        using ICryptoTransform dec = alg.CreateDecryptor();
        using CryptoStream cryptoStream = new(stream, dec, CryptoStreamMode.Read, true);
        await cryptoStream.CopyToAsync(outStream, cancellationToken);
    }
}

internal class XorSymmetricAlgorithm : SymmetricAlgorithm
{
    public const int DefaultKeySize = 16 * 8;

    public XorSymmetricAlgorithm(int keySizeValue = DefaultKeySize)
    {
        LegalKeySizesValue = new KeySizes[] { new(8, int.MaxValue, 8) };
        LegalBlockSizesValue = new KeySizes[] { new(8, int.MaxValue, 8) };
        KeySizeValue = keySizeValue;
    }

    public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[]? rgbIV) => new XorCryptoTransform(rgbKey);

    public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[]? rgbIV) => new XorCryptoTransform(rgbKey);

    public override void GenerateIV()
    {
    }

    public override void GenerateKey()
    {
        using RandomNumberGenerator rng = RandomNumberGenerator.Create();
        DoFill(KeyValue);
        KeyValue = new byte[KeySize / 8];
        rng.GetBytes(KeyValue);
    }

    protected override void Dispose(bool disposing)
    {
        DoFill(KeyValue);
        DoFill(IVValue);
        base.Dispose(disposing);
    }

    private static void DoFill(byte[]? v)
    {
        if (v == null) return;
        Array.Fill<byte>(v, 0);
    }

    private class XorCryptoTransform : ICryptoTransform
    {
        private readonly byte[] _key;

        public XorCryptoTransform(byte[] key) => _key = key;

        public bool CanReuseTransform => true;

        public bool CanTransformMultipleBlocks => false;

        public int InputBlockSize => _key.Length;

        public int OutputBlockSize => _key.Length;

        public void Dispose()
        {
        }

        public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
            Decode(inputBuffer.AsSpan(inputOffset, inputCount), outputBuffer.AsSpan(outputOffset, inputCount));
            return inputCount;
        }

        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            byte[] res = new byte[inputCount];
            Decode(inputBuffer.AsSpan(inputOffset, inputCount), res);
            return res;
        }

        private void Decode(ReadOnlySpan<byte> srcBuffer, Span<byte> resBuffer)
        {
            if (srcBuffer.Length > _key.Length) throw new ArgumentException("Cannot process block greater than key size");
            if (resBuffer.Length != srcBuffer.Length) throw new ArgumentException("Result buffer size must be equal to source buffer");
            for (int i = 0; i < resBuffer.Length; i++)
                resBuffer[i] = (byte)(srcBuffer[i] ^ _key[i]);
        }
    }
}
