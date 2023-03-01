namespace Art.Tesler;

public abstract class ToolLogHandlerProviderBase : IToolLogHandlerProvider
{
    protected static readonly char[] EnvironmentNewLine = Environment.NewLine.ToCharArray();

    public TextWriter Out { get; }

    public TextWriter Error { get; }

    public Func<Stream> OutStreamAccessFunc { get; }

    protected ToolLogHandlerProviderBase(TextWriter outWriter, TextWriter errorWriter, Func<Stream> outStreamAccessFunc)
    {
        Out = outWriter;
        Error = errorWriter;
        OutStreamAccessFunc = outStreamAccessFunc;
    }

    public abstract IToolLogHandler GetStreamToolLogHandler();

    public abstract IToolLogHandler GetDefaultToolLogHandler();

    public Stream GetOutStream() => OutStreamAccessFunc();
}
