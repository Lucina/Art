using System.CommandLine;
using System.Text;
using Art.Common.Logging;

namespace Art.Tesler;

public class StyledToolLogHandlerProvider : ToolLogHandlerProviderBase
{
    public StyledToolLogHandlerProvider() : base(EnvironmentNewLine)
    {
    }

    public StyledToolLogHandlerProvider(char[] newLine) : base(newLine)
    {
    }

    public override IToolLogHandler GetStreamToolLogHandler(IConsole console)
    {
        var encoding = Encoding.UTF8; // big assumption, there... what choice is there
        var outWriter = new ConsoleProxyTextWriter(console.Out, NewLine, encoding);
        var errorWriter = new ConsoleProxyTextWriter(console.Error, NewLine, encoding);
        return new StyledLogHandler(outWriter, errorWriter, true);
    }

    public override IToolLogHandler GetDefaultToolLogHandler(IConsole console)
    {
        var encoding = Encoding.UTF8; // big assumption, there... what choice is there
        var outWriter = new ConsoleProxyTextWriter(console.Out, NewLine, encoding);
        var errorWriter = new ConsoleProxyTextWriter(console.Error, NewLine, encoding);
        return new StyledLogHandler(outWriter, errorWriter, false, OperatingSystem.IsMacOS());
    }
}
