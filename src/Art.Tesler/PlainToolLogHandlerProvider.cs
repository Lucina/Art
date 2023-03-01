using Art.Common.Logging;

namespace Art.Tesler;

public class PlainToolLogHandlerProvider : ToolLogHandlerProviderBase
{
    public PlainToolLogHandlerProvider(TextWriter outWriter, TextWriter errorWriter) : base(outWriter, errorWriter)
    {
    }

    public override IToolLogHandler GetStreamToolLogHandler()
    {
        return new PlainLogHandler(Out, Error, true);
    }

    public override IToolLogHandler GetDefaultToolLogHandler()
    {
        return new PlainLogHandler(Out, Error, false);
    }
}
