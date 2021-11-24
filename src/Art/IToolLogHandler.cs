namespace Art;

/// <summary>
/// Represents log handler for a tool.
/// </summary>
public interface IToolLogHandler : ILogHandler
{
    /// <summary>
    /// Logs a message.
    /// </summary>
    /// <param name="tool">Tool ID.</param>
    /// <param name="group">Tool group.</param>
    /// <param name="title">Log title.</param>
    /// <param name="body">Log body.</param>
    /// <param name="level">Log level.</param>
    void Log(string tool, string group, string? title, string? body, LogLevel level);
}
