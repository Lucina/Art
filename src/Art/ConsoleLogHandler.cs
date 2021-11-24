namespace Art;

/// <summary>
/// Log handler output to console.
/// </summary>
public class ConsoleLogHandler : IToolLogHandler
{
    /// <summary>
    /// Default instance.
    /// </summary>
    public static readonly ConsoleLogHandler Default = new();

    /// <inheritdoc/>
    public void Log(string tool, string group, string? title, string? body, LogLevel level)
    {
        if (title != null) WriteTitle(level, title, group);
        if (body != null) Console.WriteLine(body);
    }

    /// <inheritdoc/>
    public void Log(string? title, string? body, LogLevel level)
    {
        if (title != null) WriteTitle(level, title);
        if (body != null) Console.WriteLine(body);
    }

    private static void WriteTitle(LogLevel level, string title, string? group = null)
        => Console.WriteLine(level switch
        {
            LogLevel.Information => group != null ? $"-- {group} -- {title}" : $"-- {title}",
            LogLevel.Title => group != null ? $">> {group} -- {title}" : $">> {title}",
            LogLevel.Warning => group != null ? $"?? {group} ?? {title}" : $"?? {title}",
            LogLevel.Error => group != null ? $"!! {group} !! {title}" : $"!! {title}",
            _ => throw new ArgumentOutOfRangeException(nameof(level)),
        });
}
