using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Art.Common.Logging;
using BarHelper;

namespace Art.Tesler;

public class ConsoleStyledToolLogHandlerProvider : ToolLogHandlerProviderBase
{
    private readonly Func<bool> _redirectedFunc;
    private readonly Func<int> _widthFunc;

    public ConsoleStyledToolLogHandlerProvider(TextWriter outWriter, TextWriter errorWriter, Func<bool> redirectedFunc, Func<int> widthFunc, Func<Stream> outStreamAccessFunc)
        : base(outWriter, errorWriter, outStreamAccessFunc)
    {
        _redirectedFunc = redirectedFunc;
        _widthFunc = widthFunc;
    }

    public override IToolLogHandler GetStreamToolLogHandler()
    {
        return new ConsoleStyledLogHandler(Out, Error, _redirectedFunc, _widthFunc, true);
    }

    public override IToolLogHandler GetDefaultToolLogHandler()
    {
        return new ConsoleStyledLogHandler(Out, Error, _redirectedFunc, _widthFunc, false, OperatingSystem.IsMacOS());
    }
}

public class ConsoleStyledLogHandler : StyledLogHandler
{
    private readonly Func<bool> _redirectedFunc;
    private readonly Func<int> _widthFunc;
    private static readonly Guid s_downloadOperation = Guid.ParseExact("c6d42b18f0ae452385f180aa74e9ef29", "N");

    public ConsoleStyledLogHandler(TextWriter outWriter, TextWriter errorWriter, Func<bool> redirectedFunc, Func<int> widthFunc, bool alwaysPrintToErrorStream, bool enableFancy = false) : base(outWriter, errorWriter, alwaysPrintToErrorStream, enableFancy)
    {
        _redirectedFunc = redirectedFunc;
        _widthFunc = widthFunc;
    }

    public override bool TryGetOperationProgressContext(string operationName, Guid operationGuid, [NotNullWhen(true)] out IOperationProgressContext? operationProgressContext)
    {
        if (operationGuid.Equals(s_downloadOperation))
        {
            operationProgressContext = new DownloadUpdateContext(operationName, Error, _redirectedFunc, _widthFunc);
            return true;
        }
        return base.TryGetOperationProgressContext(operationName, operationGuid, out operationProgressContext);
    }
}

internal class DownloadUpdateContext : IOperationProgressContext
{
    private readonly BarContext _context;
    private readonly Stopwatch _stopwatch;
    private TimedDownloadPrefabContentFiller _filler;

    public DownloadUpdateContext(string name, TextWriter output, Func<bool> redirectedFunc, Func<int> widthFunc)
    {
        _context = BarContext.Create(output, redirectedFunc, widthFunc);
        _filler = TimedDownloadPrefabContentFiller.Create(name);
        _context.Write(_filler);
        _stopwatch = new Stopwatch();
        _stopwatch.Start();
    }

    public void Report(float value)
    {
        _filler.SetDuration(_stopwatch.Elapsed);
        _filler.SetProgress(value);
        _context.Update(_filler);
    }

    public void Dispose()
    {
        _context.Write(_filler);
        _context.End();
        _context.Dispose();
    }
}
