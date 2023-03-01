using System.CommandLine;
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
        CreateTextWriters(console, out var outWriter, out var errorWriter);
        return new PlainLogHandler(outWriter, errorWriter, true);
    }

    public override IToolLogHandler GetDefaultToolLogHandler(IConsole console)
    {
        CreateTextWriters(console, out var outWriter, out var errorWriter);
        return new PlainLogHandler(outWriter, errorWriter, false);
    }
}
