namespace Art.Common.Crypto;

/// <summary>
/// Contains utilities for padding.
/// </summary>
public class Padding
{
    /// <summary>
    /// Get padded message length using specified padding mode
    /// </summary>
    /// <param name="length">Input message length</param>
    /// <param name="artPaddingMode">Padding mode to use</param>
    /// <param name="blockSize">Cipher block size</param>
    /// <returns>Padded length of message</returns>
    public static int GetPaddedLength(int length, ArtPaddingMode artPaddingMode, int blockSize) =>
        artPaddingMode switch
        {
            ArtPaddingMode.Zero => length + (blockSize - length % blockSize),
            ArtPaddingMode.Iso_Iec_7816_4 or ArtPaddingMode.AnsiX9_23 or ArtPaddingMode.Iso10126 or ArtPaddingMode.Pkcs7 or ArtPaddingMode.Pkcs5 => length + 1 + (blockSize - (length + 1) % blockSize),
            _ => throw new ArgumentOutOfRangeException(nameof(artPaddingMode), artPaddingMode, null)
        };

    /// <summary>
    /// Get depadded message length using specified padding mode
    /// </summary>
    /// <param name="span">Message</param>
    /// <param name="artPaddingMode">Padding mode to use</param>
    /// <returns>Depadded length of message</returns>
    public static int GetDepaddedLength(Span<byte> span, ArtPaddingMode artPaddingMode) =>
        artPaddingMode switch
        {
            ArtPaddingMode.Zero => GetDepaddedLengthZero(span),
            ArtPaddingMode.Iso_Iec_7816_4 => GetDepaddedLengthIso_Iec_7816_4(span),
            ArtPaddingMode.AnsiX9_23 or ArtPaddingMode.Iso10126 or ArtPaddingMode.Pkcs7 or ArtPaddingMode.Pkcs5 => GetDepaddedLengthLastByteSubtract(span),
            _ => throw new ArgumentOutOfRangeException(nameof(artPaddingMode), artPaddingMode, null)
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
                        $"Invalid padding byte for {nameof(ArtPaddingMode.Iso_Iec_7816_4)}, need 0x80 or 0x00 but got 0x{span[i]:X2}");
            }
        }

        throw new ArgumentException(
            $"Message is all null bytes and {nameof(ArtPaddingMode.Iso_Iec_7816_4)} requires 0x80 to mark beginning of padding");
    }

    private static int GetDepaddedLengthLastByteSubtract(ReadOnlySpan<byte> span) =>
        span.Length == 0 ? 0 : span.Length - span[^1];
}
