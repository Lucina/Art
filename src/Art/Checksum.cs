namespace Art;

/// <summary>
/// Represents a checksum pairing an algorithm ID and checksum value.
/// </summary>
/// <param name="Id">Algorithm ID.</param>
/// <param name="Value">Checksum value.</param>
public record Checksum(string Id, byte[] Value)
{
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
            return first.Id == second.Id && first.Value.AsSpan().SequenceEqual(second.Value);
        return true;
    }
}
