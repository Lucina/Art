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

    public override IToolLogHandler GetStreamToolLogHandler(IOutputPair console)
    {
        return new StyledLogHandler(console.Out, console.Error, true);
    }

    public override IToolLogHandler GetDefaultToolLogHandler(IOutputPair console)
    {
        return new StyledLogHandler(console.Out, console.Error, false, OperatingSystem.IsMacOS());
    }
}
