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

    public override IToolLogHandler GetStreamToolLogHandler(IOutputPair console)
    {
        return new PlainLogHandler(console.Out, console.Error, true);
    }

    public override IToolLogHandler GetDefaultToolLogHandler(IOutputPair console)
    {
        return new PlainLogHandler(console.Out, console.Error, false);
    }
}
