using System.Security.Cryptography;

namespace Art.CipherPadding;

/// <summary>
/// Base handler for depadding formats where the last byte specifies # padding bytes.
/// </summary>
public abstract class LastByteCountDepaddingHandler : BlockedDepaddingHandler
{
    /// <summary>
    /// Initializes a new instance of <see cref="PkcsDepaddingHandler"/>.
    /// </summary>
    /// <param name="supportedBlockSize">Supported block sizes.</param>
    /// <param name="blockSize">Block size, in bytes.</param>
    /// <exception cref="ArgumentException">Thrown for invalid <paramref name="blockSize"/> or illegally configured <paramref name="supportedBlockSize"/>.</exception>
    protected LastByteCountDepaddingHandler(KeySizes supportedBlockSize, int blockSize) : base(supportedBlockSize, blockSize)
    {
    }

    /// <inheritdoc />
    protected sealed override bool ValidateLastBlock(ReadOnlySpan<byte> buffer, out byte b)
    {
        if (buffer.Length == 0)
        {
            b = 0;
            return false;
        }
        b = buffer[^1];
        return b <= BlockSize;
    }
}
