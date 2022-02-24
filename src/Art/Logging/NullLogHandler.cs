namespace Art.Logging;

/// <summary>
/// Log handler with no output.
/// </summary>
public class NullLogHandler : IToolLogHandler
{
    /// <summary>
    /// Default instance.
    /// </summary>
    public static readonly NullLogHandler Default = new();

    /// <inheritdoc/>
    public void Log(string tool, string group, string? title, string? body, LogLevel level)
    {
    }

    /// <inheritdoc/>
    public void Log(string? title, string? body, LogLevel level)
    {
    }
}
