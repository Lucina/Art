namespace Art;

/// <summary>
/// Represents a checksum pairing an algorithm ID and checksum value.
/// </summary>
/// <param name="Id">Algorithm ID. Case-insensitive.</param>
/// <param name="Value">Checksum value.</param>
public record Checksum(string Id, byte[] Value);
