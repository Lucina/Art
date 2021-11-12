namespace Art;

/// <summary>
/// Represents constraints on retrieval.
/// </summary>
public record RetrievalConstraints
{
    /// <summary>
    /// Time constraint, drop all entries on or before this time.
    /// </summary>
    public DateTimeOffset? After { get; init; }

    /// <summary>
    /// Time constraint, drop all entries on or after this time.
    /// </summary>
    public DateTimeOffset? Before { get; init; }
}
