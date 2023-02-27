namespace Art.Common.Logging;

/// <summary>
/// Log handler output to console.
/// </summary>
public class ConsoleLogHandler : IToolLogHandler
{
    private readonly AutoResetEvent _wh;
    private readonly bool _itsumoError;

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
        { LogLevel.Entry, "::" },
        { LogLevel.Warning, "❗" },
        { LogLevel.Error, "⛔" }
    };

    /// <summary>
    /// Default instance.
    /// </summary>
    public static readonly ConsoleLogHandler Default = new(true);

    /// <summary>
    /// Fancy instance.
    /// </summary>
    public static readonly ConsoleLogHandler Fancy = new(true, true);

    private readonly Dictionary<LogLevel, string> _pre;

    /// <summary>
    /// Creates a new instance of <see cref="ConsoleLogHandler"/>.
    /// </summary>
    /// <param name="alwaysPrintToErrorStream">If true, always print output to error stream.</param>
    /// <param name="enableFancy">Enable fancy output.</param>
    public ConsoleLogHandler(bool alwaysPrintToErrorStream, bool enableFancy = false)
    {
        _pre = enableFancy ? s_preOsx : s_preDefault;
        _wh = new AutoResetEvent(true);
        _itsumoError = alwaysPrintToErrorStream;
    }

    /// <inheritdoc/>
    public void Log(string tool, string group, string? title, string? body, LogLevel level)
    {
        _wh.WaitOne();
        try
        {
            var textWriter = SelectTextWriter(level);
            if (title != null) WriteTitle(textWriter, level, title, group);
            if (body != null) textWriter.WriteLine(body);
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
            var textWriter = SelectTextWriter(level);
            if (title != null) WriteTitle(textWriter, level, title);
            if (body != null) textWriter.WriteLine(body);
        }
        finally
        {
            _wh.Set();
        }
    }

    private TextWriter SelectTextWriter(LogLevel level)
    {
        if (_itsumoError)
        {
            return Console.Error;
        }
        return level switch
        {
            LogLevel.Information => Console.Out,
            LogLevel.Entry => Console.Out,
            LogLevel.Title => Console.Out,
            LogLevel.Warning => Console.Error,
            LogLevel.Error => Console.Error,
            _ => Console.Error
        };
    }

    private void WriteTitle(TextWriter writer, LogLevel level, string title, string? group = null)
    {
        writer.WriteLine(group != null ? $"{_pre[level]} {group} {_pre[level]} {title}" : $"{_pre[level]} {title}");
    }
}
