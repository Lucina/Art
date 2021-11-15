namespace Art;

/// <summary>
/// Log level.
/// </summary>
[Flags]
public enum LogLevel
{
    /// <summary>
    /// Information log.
    /// </summary>
    Information = 1,

    /// <summary>
    /// Title information log.
    /// </summary>
    Title = 1 << 1,

    /// <summary>
    /// Warning log.
    /// </summary>
    Warning = 1 << 2,

    /// <summary>
    /// Error log.
    /// </summary>
    Error = 1 << 3
}
