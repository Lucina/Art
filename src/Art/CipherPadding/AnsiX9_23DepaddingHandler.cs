using System.Security.Cryptography;

namespace Art.CipherPadding;

/// <summary>
/// Handler for ANSI X9.23 depadding.
/// </summary>
public class AnsiX9_23DepaddingHandler : LastByteCountDepaddingHandler
{
    private static readonly KeySizes s_supportedBlockSize = new(8, 8, 0);

    /// <summary>
    /// Initializes a new instance of <see cref="AnsiX9_23DepaddingHandler"/>.
    /// </summary>
    /// <param name="blockSize">Block size, in bytes.</param>
    /// <exception cref="ArgumentException">Thrown for invalid <see cref="blockSize"/>.</exception>
    public AnsiX9_23DepaddingHandler(int blockSize) : base(s_supportedBlockSize, blockSize)
    {
    }
}
