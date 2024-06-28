using Art.Common.Logging;

namespace Art.Tesler;

public class StyledToolLogHandlerProvider : ToolLogHandlerProviderBase
{
    public StyledToolLogHandlerProvider(
        TextWriter outWriter,
        TextWriter warnWriter,
        TextWriter errorWriter,
        Func<Stream> outStreamAccessFunc)
        : base(outWriter, warnWriter, errorWriter, outStreamAccessFunc)
    {
    }

    public override IToolLogHandler GetStreamToolLogHandler()
    {
        return new StyledLogHandler(Out, Warn, Error, true);
    }

    public override IToolLogHandler GetDefaultToolLogHandler()
    {
        return new StyledLogHandler(Out, Warn, Error, false, OperatingSystem.IsMacOS());
    }
}
