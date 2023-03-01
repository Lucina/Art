namespace Art.Tesler;

public interface IToolLogHandlerProvider : IOutputPair
{
    internal IToolLogHandler GetStreamToolLogHandler();

    internal IToolLogHandler GetDefaultToolLogHandler();

    internal Stream GetOutStream();
}
