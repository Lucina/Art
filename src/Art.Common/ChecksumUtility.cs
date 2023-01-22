namespace Art.Common;

/// <summary>
/// Utility for <see cref="Checksum"/>.
/// </summary>
public class ChecksumUtility
{
    /// <summary>
    /// Initializes a new instance of <see cref="Checksum"/>.
    /// </summary>
    /// <param name="id">ID.</param>
    /// <param name="value">Hex string containing checksum value.</param>
    public static Checksum CreateChecksum(string id, string value)
    {
        return new Checksum(id, Dehex(value));
    }

    /// <summary>
    /// Compares two values for data equality.
    /// </summary>
    /// <param name="first">First value.</param>
    /// <param name="second">Other value.</param>
    /// <returns>True if equal.</returns>
    public static bool DatawiseEquals(Checksum? first, Checksum? second)
    {
        if (first != null && second == null || first == null && second != null) return false;
        if (first != null && second != null)
            return string.Equals(first.Id, second.Id, StringComparison.InvariantCultureIgnoreCase) && first.Value.AsSpan().SequenceEqual(second.Value);
        return true;
    }

    private static byte[] Dehex(ReadOnlySpan<char> hex)
    {
        if (hex.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase)) hex = hex[2..];
        return Convert.FromHexString(hex);
    }
}
