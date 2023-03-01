using System.CommandLine;

namespace Art.Tesler;

public interface IToolLogHandlerProvider
{
    internal IToolLogHandler GetStreamToolLogHandler(IConsole console);

    internal IToolLogHandler GetDefaultToolLogHandler(IConsole console);
}
