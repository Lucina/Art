using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Art.Common.Logging;
using BarHelper;

namespace Art.Tesler;

public class ConsoleStyledToolLogHandlerProvider : ToolLogHandlerProviderBase
{
    private readonly Func<bool> _errorRedirectedFunc;
    private readonly Func<int> _widthFunc;

    public ConsoleStyledToolLogHandlerProvider(TextWriter outWriter, TextWriter errorWriter, Func<bool> errorRedirectedFunc, Func<int> widthFunc, Func<Stream> outStreamAccessFunc)
        : base(outWriter, errorWriter, outStreamAccessFunc)
    {
        _errorRedirectedFunc = errorRedirectedFunc;
        _widthFunc = widthFunc;
    }

    public override IToolLogHandler GetStreamToolLogHandler()
    {
        return new ConsoleStyledLogHandler(Out, Error, true, _errorRedirectedFunc, _widthFunc, true);
    }

    public override IToolLogHandler GetDefaultToolLogHandler()
    {
        return new ConsoleStyledLogHandler(Out, Error, false, _errorRedirectedFunc, _widthFunc, false, OperatingSystem.IsMacOS());
    }
}

public class ConsoleStyledLogHandler : StyledLogHandler
{
    private readonly bool _forceFallback;
    private readonly Func<bool> _errorRedirectedFunc;
    private readonly Func<int> _widthFunc;
    private static readonly Guid s_downloadOperation = Guid.ParseExact("c6d42b18f0ae452385f180aa74e9ef29", "N");
    private static readonly Guid s_operationWaitingForResult = Guid.ParseExact("4fd5c851a88c430c8f8da54dbcf70ab2", "N");

    public ConsoleStyledLogHandler(TextWriter outWriter, TextWriter errorWriter, bool forceFallback, Func<bool> errorRedirectedFunc, Func<int> widthFunc, bool alwaysPrintToErrorStream, bool enableFancy = false) : base(outWriter, errorWriter, alwaysPrintToErrorStream, enableFancy)
    {
        _forceFallback = forceFallback;
        _errorRedirectedFunc = errorRedirectedFunc;
        _widthFunc = widthFunc;
    }

    public override bool TryGetOperationProgressContext(string operationName, Guid operationGuid, [NotNullWhen(true)] out IOperationProgressContext? operationProgressContext)
    {
        if (operationGuid.Equals(s_downloadOperation))
        {
            operationProgressContext = new DownloadUpdateContext(operationName, Error, _forceFallback, _errorRedirectedFunc, _widthFunc);
            return true;
        }
        if (operationGuid.Equals(s_operationWaitingForResult))
        {
            operationProgressContext = new WaitUpdateContext(operationName, Error, _forceFallback, _errorRedirectedFunc, _widthFunc);
            return true;
        }
        return base.TryGetOperationProgressContext(operationName, operationGuid, out operationProgressContext);
    }
}

internal class WaitUpdateContext : IOperationProgressContext
{
    private readonly BarContext _context;
    private readonly EllipsisSuffixContentFiller _filler;

    public WaitUpdateContext(string name, TextWriter output, bool forceFallback, Func<bool> errorRedirectedFunc, Func<int> widthFunc)
    {
        _context = BarContext.Create(output, forceFallback, errorRedirectedFunc, widthFunc);
        _filler = new EllipsisSuffixContentFiller(name, 0);
        _context.Write(_filler);
    }

    public void Report(float value)
    {
        _context.Update(_filler);
    }

    public void Dispose()
    {
        _context.Clear();
        _context.Dispose();
    }
}

internal class DownloadUpdateContext : IOperationProgressContext
{
    private readonly BarContext _context;
    private readonly Stopwatch _stopwatch;
    private TimedDownloadPrefabContentFiller _filler;

    public DownloadUpdateContext(string name, TextWriter output, bool forceFallback, Func<bool> errorRedirectedFunc, Func<int> widthFunc)
    {
        _context = BarContext.Create(output, forceFallback, errorRedirectedFunc, widthFunc);
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
        _context.Clear();
        //_context.Write(_filler);
        //_context.End();
        _context.Dispose();
    }
}
