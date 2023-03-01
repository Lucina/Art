namespace Art.Common.Logging;

/// <summary>
/// Log handler output to two backing <see cref="TextWriter"/> (out and err) with styles.
/// </summary>
public class StyledLogHandler : IToolLogHandler
{
    private readonly AutoResetEvent _wh;
    private readonly bool _itsumoError;
    private readonly TextWriter _out;
    private readonly TextWriter _error;

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

    private readonly Dictionary<LogLevel, string> _pre;

    /// <summary>
    /// Initializes an instance of <see cref="StyledLogHandler"/>.
    /// </summary>
    /// <param name="outWriter">Writer for normal output.</param>
    /// <param name="errorWriter">Writer for error output.</param>
    /// <param name="alwaysPrintToErrorStream">If true, always print output to error stream.</param>
    /// <param name="enableFancy">Enable fancy output.</param>
    public StyledLogHandler(TextWriter outWriter, TextWriter errorWriter, bool alwaysPrintToErrorStream, bool enableFancy = false)
    {
        _out = outWriter ?? throw new ArgumentNullException(nameof(outWriter));
        _error = errorWriter ?? throw new ArgumentNullException(nameof(errorWriter));
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
            return _error;
        }
        return level switch
        {
            LogLevel.Information => _out,
            LogLevel.Entry => _out,
            LogLevel.Title => _out,
            LogLevel.Warning => _error,
            LogLevel.Error => _error,
            _ => _error
        };
    }

    private void WriteTitle(TextWriter writer, LogLevel level, string title, string? group = null)
    {
        writer.WriteLine(group != null ? $"{_pre[level]} {group} {_pre[level]} {title}" : $"{_pre[level]} {title}");
    }
}
