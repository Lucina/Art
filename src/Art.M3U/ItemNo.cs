namespace Art.M3U;

/// <summary>
/// Marks item number.
/// </summary>
/// <param name="Number">Item number.</param>
/// <param name="Total">Total.</param>
public readonly record struct ItemNo(long Number, long? Total)
{
    /// <summary>
    /// Gets message.
    /// </summary>
    /// <returns>Message.</returns>
    public string GetMessage() => $"{Number:D}/{Total?.ToString("D") ?? "???"}";
}
