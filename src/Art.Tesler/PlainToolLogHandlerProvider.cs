using System.CommandLine;
using System.Text;
using Art.Common.Logging;

namespace Art.Tesler;

public class PlainToolLogHandlerProvider : ToolLogHandlerProviderBase
{
    public PlainToolLogHandlerProvider() : base(EnvironmentNewLine)
    {
    }

    public PlainToolLogHandlerProvider(char[] newLine) : base(newLine)
    {
    }

    public override IToolLogHandler GetStreamToolLogHandler(IConsole console)
    {
        var encoding = Encoding.UTF8; // big assumption, there... what choice is there
        var outWriter = new ConsoleProxyTextWriter(console.Out, NewLine, encoding);
        var errorWriter = new ConsoleProxyTextWriter(console.Error, NewLine, encoding);
        return new PlainLogHandler(outWriter, errorWriter, true);
    }

    public override IToolLogHandler GetDefaultToolLogHandler(IConsole console)
    {
        var encoding = Encoding.UTF8; // big assumption, there... what choice is there
        var outWriter = new ConsoleProxyTextWriter(console.Out, NewLine, encoding);
        var errorWriter = new ConsoleProxyTextWriter(console.Error, NewLine, encoding);
        return new PlainLogHandler(outWriter, errorWriter, false);
    }
}
