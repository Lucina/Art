namespace Art;

/// <summary>
/// Represents a checksum pairing an algorithm ID and checksum value.
/// </summary>
/// <param name="Id">Algorithm ID. Case-insensitive.</param>
/// <param name="Value">Checksum value.</param>
public record Checksum(string Id, byte[] Value)
{
    /// <summary>
    /// Initializes a new instance of <see cref="Checksum"/>.
    /// </summary>
    /// <param name="id">ID.</param>
    /// <param name="value">Hex string containing checksum value.</param>
    public Checksum(string id, string value) : this(id, ArtExtensions.Dehex(value))
    {
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
}
