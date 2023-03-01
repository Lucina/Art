using System.CommandLine;
using System.Text;

namespace Art.Tesler;

public abstract class ToolLogHandlerProviderBase : IToolLogHandlerProvider
{
    protected static readonly char[] EnvironmentNewLine = Environment.NewLine.ToCharArray();

    protected readonly char[] NewLine;

    protected ToolLogHandlerProviderBase(char[] newLine)
    {
        NewLine = newLine;
    }

    protected void CreateTextWriters(IConsole console, out TextWriter outWriter, out TextWriter errorWriter)
    {
        var encoding = Encoding.UTF8; // big assumption, there... what choice is there
        outWriter = new ConsoleProxyTextWriter(console.Out, NewLine, encoding);
        errorWriter = new ConsoleProxyTextWriter(console.Error, NewLine, encoding);
    }

    public abstract IToolLogHandler GetStreamToolLogHandler(IConsole console);

    public abstract IToolLogHandler GetDefaultToolLogHandler(IConsole console);
}
