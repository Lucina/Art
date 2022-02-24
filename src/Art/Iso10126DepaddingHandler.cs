using System.Security.Cryptography;

namespace Art;

/// <summary>
/// Handler for ISO 10126 depadding.
/// </summary>
public class Iso10126DepaddingHandler : LastByteCountDepaddingHandler
{
    private static readonly KeySizes s_supportedBlockSize = new(1, int.MaxValue, 1);

    /// <summary>
    /// Initializes a new instance of <see cref="Iso10126DepaddingHandler"/>.
    /// </summary>
    /// <param name="blockSize">Block size, in bytes.</param>
    /// <exception cref="ArgumentException">Thrown for invalid <see cref="blockSize"/>.</exception>
    public Iso10126DepaddingHandler(int blockSize) : base(s_supportedBlockSize, blockSize)
    {
    }
}
