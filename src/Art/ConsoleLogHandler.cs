namespace Art;

/// <summary>
/// Log handler output to console.
/// </summary>
public class ConsoleLogHandler : IToolLogHandler
{
    private readonly AutoResetEvent _wh;

    private static readonly Dictionary<LogLevel, string> s_preDefault = new()
    {
        { LogLevel.Information, ">>" },
        { LogLevel.Title, ">>" },
        { LogLevel.Entry, "--" },
        { LogLevel.Warning, "??" },
        { LogLevel.Error, "!!" }
    };

    private static readonly Dictionary<LogLevel, string> s_preOsx = new()
    {
        { LogLevel.Information, "⚪" },
        { LogLevel.Title, "⚪" },
        { LogLevel.Entry, "--" },
        { LogLevel.Warning, "❗" },
        { LogLevel.Error, "⛔" }
    };

    /// <summary>
    /// Default instance.
    /// </summary>
    public static readonly ConsoleLogHandler Default = new();

    private readonly Dictionary<LogLevel, string> _pre;

    /// <summary>
    /// Creates a new instance of <see cref="ConsoleLogHandler"/>.
    /// </summary>
    public ConsoleLogHandler()
    {
        _pre = OperatingSystem.IsMacOS() ? s_preOsx : s_preDefault;
        _wh = new AutoResetEvent(true);
    }

    /// <inheritdoc/>
    public void Log(string tool, string group, string? title, string? body, LogLevel level)
    {
        _wh.WaitOne();
        try
        {
            if (title != null) WriteTitle(level, title, group);
            if (body != null) Console.WriteLine(body);
        }
        finally
        {
            _wh.Set();
        }
    }

    /// <inheritdoc/>
    public void Log(string? title, string? body, LogLevel level)
    {
        _wh.WaitOne();
        try
        {
            if (title != null) WriteTitle(level, title);
            if (body != null) Console.WriteLine(body);
        }
        finally
        {
            _wh.Set();
        }
    }

    private void WriteTitle(LogLevel level, string title, string? group = null)
        => Console.WriteLine(group != null ? $"{_pre[level]} {group} {_pre[level]} {title}" : $"{_pre[level]} {title}");
}
