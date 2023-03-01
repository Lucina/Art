namespace Art.Tesler;

public abstract class ToolLogHandlerProviderBase : IToolLogHandlerProvider
{
    protected static readonly char[] EnvironmentNewLine = Environment.NewLine.ToCharArray();

    public TextWriter Out { get; }

    public TextWriter Error { get; }

    protected ToolLogHandlerProviderBase(TextWriter outWriter, TextWriter errorWriter)
    {
        Out = outWriter;
        Error = errorWriter;
    }

    public abstract IToolLogHandler GetStreamToolLogHandler();

    public abstract IToolLogHandler GetDefaultToolLogHandler();
}
