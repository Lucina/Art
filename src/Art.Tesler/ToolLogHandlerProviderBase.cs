namespace Art.Tesler;

public abstract class ToolLogHandlerProviderBase : IToolLogHandlerProvider
{
    protected static readonly char[] EnvironmentNewLine = Environment.NewLine.ToCharArray();

    protected readonly char[] NewLine;

    protected ToolLogHandlerProviderBase(char[] newLine)
    {
        NewLine = newLine;
    }

    public abstract IToolLogHandler GetStreamToolLogHandler(IOutputPair console);

    public abstract IToolLogHandler GetDefaultToolLogHandler(IOutputPair console);
}
