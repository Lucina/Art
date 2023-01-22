using System.Security.Cryptography;

namespace Art.Common.CipherPadding;

/// <summary>
/// Handler for PKCS#7 depadding.
/// </summary>
public class Pkcs7DepaddingHandler : PkcsDepaddingHandler
{
    private static readonly KeySizes s_supportedBlockSize = new(1, 255, 1);

    /// <summary>
    /// Initializes a new instance of <see cref="Pkcs5DepaddingHandler"/>.
    /// </summary>
    /// <param name="blockSize">Block size, in bytes.</param>
    /// <exception cref="ArgumentException">Thrown for invalid <paramref name="blockSize"/>.</exception>
    public Pkcs7DepaddingHandler(int blockSize) : base(s_supportedBlockSize, blockSize)
    {
    }
}
