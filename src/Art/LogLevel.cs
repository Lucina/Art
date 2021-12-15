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
    Information = 1 << 0,

    /// <summary>
    /// Entry information log.
    /// </summary>
    Entry = 1 << 1,

    /// <summary>
    /// Title information log.
    /// </summary>
    Title = 1 << 2,

    /// <summary>
    /// Warning log.
    /// </summary>
    Warning = 1 << 3,

    /// <summary>
    /// Error log.
    /// </summary>
    Error = 1 << 4
}
