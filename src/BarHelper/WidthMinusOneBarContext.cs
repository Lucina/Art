namespace BarHelper;

public class WidthMinusOneBarContext : BarContext
{
    private bool _disposed;
    protected override int AvailableWidth => base.AvailableWidth - 1;

    public WidthMinusOneBarContext(TextWriter output, Func<bool> redirectedFunc, Func<int> widthFunc, TimeSpan interval) : base(output, redirectedFunc, widthFunc, interval)
    {
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (_disposed)
        {
            return;
        }
        _disposed = true;
    }
}
