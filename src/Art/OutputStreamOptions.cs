namespace Art;

/// <summary>
/// Represents options for creating artifact output stream.
/// </summary>
public readonly record struct OutputStreamOptions
{
    /// <summary>
    /// Default options.
    /// </summary>
    public static readonly OutputStreamOptions Default = new();

    /// <summary>
    /// Stream preallocation size.
    /// </summary>
    public readonly int? PreallocationSize { get; init; }


}
