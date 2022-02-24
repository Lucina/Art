using System.Security.Cryptography;

namespace Art;

/// <summary>
/// Handler for ISO/IEC 7816-4:2005 depadding.
/// </summary>
public class Iso_Iec_7816_4DepaddingHandler : BlockedDepaddingHandler
{
    private static readonly KeySizes s_supportedBlockSize = new(1, int.MaxValue, 1);

    /// <summary>
    /// Initializes a new instance of <see cref="Iso_Iec_7816_4DepaddingHandler"/>.
    /// </summary>
    /// <param name="blockSize">Block size, in bytes.</param>
    /// <exception cref="ArgumentException">Thrown for invalid <see cref="blockSize"/>.</exception>
    public Iso_Iec_7816_4DepaddingHandler(int blockSize) : base(s_supportedBlockSize, blockSize)
    {
    }

    /// <inheritdoc />
    protected override bool ValidateLastBlock(ReadOnlySpan<byte> buffer, out byte b)
    {
        b = 0;
        for (int i = buffer.Length - 1; i >= 0; i--)
        {
            byte v = buffer[i];
            switch (v)
            {
                case 0:
                    b++;
                    break;
                case 0x80:
                    b++;
                    return true;
                default:
                    b = 0;
                    return false;
            }
        }
        b = 0;
        return false;
    }
}
