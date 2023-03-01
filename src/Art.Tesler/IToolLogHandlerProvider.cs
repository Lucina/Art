namespace Art.Tesler;

public interface IToolLogHandlerProvider
{
    // TODO implementors should use their own TW sources
    internal IToolLogHandler GetStreamToolLogHandler(IOutputPair console);

    internal IToolLogHandler GetDefaultToolLogHandler(IOutputPair console);
}
