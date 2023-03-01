using Art.Common.Logging;

namespace Art.Tesler;

public class StyledToolLogHandlerProvider : ToolLogHandlerProviderBase
{
    public StyledToolLogHandlerProvider(TextWriter outWriter, TextWriter errorWriter) : base(outWriter, errorWriter)
    {
    }

    public override IToolLogHandler GetStreamToolLogHandler()
    {
        return new StyledLogHandler(Out, Error, true);
    }

    public override IToolLogHandler GetDefaultToolLogHandler()
    {
        return new StyledLogHandler(Out, Error, false, OperatingSystem.IsMacOS());
    }
}
