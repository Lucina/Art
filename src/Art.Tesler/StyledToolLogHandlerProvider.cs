using System.CommandLine;
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
        CreateTextWriters(console, out var outWriter, out var errorWriter);
        return new StyledLogHandler(outWriter, errorWriter, true);
    }

    public override IToolLogHandler GetDefaultToolLogHandler(IConsole console)
    {
        CreateTextWriters(console, out var outWriter, out var errorWriter);
        return new StyledLogHandler(outWriter, errorWriter, false, OperatingSystem.IsMacOS());
    }
}
