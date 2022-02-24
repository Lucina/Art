using System.Security.Cryptography;

namespace Art;

/// <summary>
/// Handler for PKCS#5 depadding.
/// </summary>
public class Pkcs5DepaddingHandler : PkcsDepaddingHandler
{
    private static readonly KeySizes s_supportedBlockSize = new(8, 8, 0);

    /// <summary>
    /// Initializes a new instance of <see cref="Pkcs5DepaddingHandler"/>.
    /// </summary>
    /// <param name="blockSize">Block size, in bytes.</param>
    /// <exception cref="ArgumentException">Thrown for invalid <paramref name="blockSize"/>.</exception>
    public Pkcs5DepaddingHandler(int blockSize) : base(s_supportedBlockSize, blockSize)
    {
    }
}
