namespace Art.Tesler;

public interface IToolLogHandlerProvider : IOutputControl
{
    internal IToolLogHandler GetStreamToolLogHandler();

    internal IToolLogHandler GetDefaultToolLogHandler();

    internal Stream GetOutStream();
}
