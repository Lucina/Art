namespace Art;

/// <summary>
/// Contains utilities for padding.
/// </summary>
public class Padding
{
    /// <summary>
    /// Get padded message length using specified padding mode
    /// </summary>
    /// <param name="length">Input message length</param>
    /// <param name="paddingMode">Padding mode to use</param>
    /// <param name="blockSize">Cipher block size</param>
    /// <returns>Padded length of message</returns>
    public static int GetPaddedLength(int length, PaddingMode paddingMode, int blockSize) =>
        paddingMode switch
        {
            PaddingMode.Zero => length + (blockSize - length % blockSize),
            PaddingMode.Iso_Iec_7816_4 or PaddingMode.AnsiX9_23 or PaddingMode.Iso10126 or PaddingMode.Pkcs7 or PaddingMode.Pkcs5 => length + 1 + (blockSize - (length + 1) % blockSize),
            _ => throw new ArgumentOutOfRangeException(nameof(paddingMode), paddingMode, null)
        };

    /// <summary>
    /// Get depadded message length using specified padding mode
    /// </summary>
    /// <param name="span">Message</param>
    /// <param name="paddingMode">Padding mode to use</param>
    /// <returns>Depadded length of message</returns>
    public static int GetDepaddedLength(Span<byte> span, PaddingMode paddingMode) =>
        paddingMode switch
        {
            PaddingMode.Zero => GetDepaddedLengthZero(span),
            PaddingMode.Iso_Iec_7816_4 => GetDepaddedLengthIso_Iec_7816_4(span),
            PaddingMode.AnsiX9_23 or PaddingMode.Iso10126 or PaddingMode.Pkcs7 or PaddingMode.Pkcs5 => GetDepaddedLengthLastByteSubtract(span),
            _ => throw new ArgumentOutOfRangeException(nameof(paddingMode), paddingMode, null)
        };

    private static int GetDepaddedLengthZero(Span<byte> span)
    {
        for (int i = span.Length; i > 0; i--)
        {
            if (span[i - 1] != 0)
            {
                return i;
            }
        }

        return 0;
    }

    private static int GetDepaddedLengthIso_Iec_7816_4(Span<byte> span)
    {
        for (int i = span.Length - 1; i >= 0; i--)
        {
            switch (span[i])
            {
                case 0x00:
                    break;
                case 0x80:
                    return i;
                default:
                    throw new ArgumentException(
                        $"Invalid padding byte for {nameof(PaddingMode.Iso_Iec_7816_4)}, need 0x80 or 0x00 but got 0x{span[i]:X2}");
            }
        }

        throw new ArgumentException(
            $"Message is all null bytes and {nameof(PaddingMode.Iso_Iec_7816_4)} requires 0x80 to mark beginning of padding");
    }

    private static int GetDepaddedLengthLastByteSubtract(ReadOnlySpan<byte> span) =>
        span.Length == 0 ? 0 : span.Length - span[^1];
}
