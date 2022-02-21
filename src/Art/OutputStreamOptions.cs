namespace Art;

/// <summary>
/// Represents options for creating artifact output stream.
/// </summary>
public record OutputStreamOptions
{
    /// <summary>
    /// Default options.
    /// </summary>
    public static readonly OutputStreamOptions Default = new() { PreallocationSize = 0 };

    /// <summary>
    /// Stream preallocation size.
    /// </summary>
    /// <remarks>
    /// A value of 0 is ignored. Negative values are invalid.
    /// </remarks>
    public long PreallocationSize { get; init; }
}
