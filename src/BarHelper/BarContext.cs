using System.Diagnostics;
using System.Text;

namespace BarHelper;

public abstract class BarContext : IDisposable
{
    public static readonly TimeSpan DefaultInterval = TimeSpan.FromSeconds(0.05f);

    protected virtual int AvailableWidth => _redirectedFunc() ? 40 : _widthFunc();
    private readonly TextWriter _output;
    private readonly Func<bool> _redirectedFunc;
    private readonly Func<int> _widthFunc;
    private readonly TimeSpan _interval;
    private readonly Stopwatch _stopwatch;
    private readonly StringBuilder _stringBuilder;
    private bool _active;

    protected BarContext(TextWriter output, Func<bool> redirectedFunc, Func<int> widthFunc, TimeSpan interval)
    {
        _interval = interval;
        _output = output;
        _redirectedFunc = redirectedFunc;
        _widthFunc = widthFunc;
        _stopwatch = new Stopwatch();
        _stopwatch.Start();
        _stringBuilder = new StringBuilder();
    }

    public void Write<T>(T contentFiller) where T : IContentFiller
    {
        _active = true;
        DrawInternal(contentFiller);
    }

    public void Update<T>(T contentFiller) where T : IContentFiller
    {
        _active = true;
        if (_stopwatch.Elapsed < _interval)
        {
            return;
        }
        DrawInternal(contentFiller);
    }

    public void Clear()
    {
        _active = true;
        DrawInternal(new BlankContentFiller());
        _output.Write('\r');
    }

    public void End()
    {
        if (!_active)
        {
            return;
        }
        _active = false;
        EndLine();
    }

    private void DrawInternal<T>(T contentFiller) where T : IContentFiller
    {
        _stopwatch.Restart();
        _stringBuilder.Clear();
        int widthRemaining = AvailableWidth;
        _stringBuilder.Append('\r');
        if (widthRemaining > 0)
        {
            contentFiller.Fill(_stringBuilder, widthRemaining);
        }
        DrawLine(_stringBuilder);
        _stringBuilder.Clear();
    }

    internal struct BlankContentFiller : IContentFiller
    {
        public void Fill(StringBuilder stringBuilder, int width)
        {
            StringFillUtil.PadRemaining(stringBuilder, width);
        }
    }

    protected virtual void DrawLine(StringBuilder stringBuilder)
    {
        _output.Write(stringBuilder.ToString());
    }

    protected virtual void EndLine()
    {
        _output.WriteLine();
    }

    public static BarContext Create(TextWriter output, bool forceFallback, Func<bool> redirectedFunc, Func<int> widthFunc, TimeSpan interval = default)
    {
        if (interval == default)
        {
            interval = DefaultInterval;
        }
        if (!forceFallback && OperatingSystem.IsWindowsVersionAtLeast(5, 1, 2600))
        {
            return new WindowsBarContext(output, redirectedFunc, widthFunc, interval);
        }
        return new WidthMinusOneBarContext(output, redirectedFunc, widthFunc, interval);
    }

    protected virtual void Dispose(bool disposing)
    {
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
