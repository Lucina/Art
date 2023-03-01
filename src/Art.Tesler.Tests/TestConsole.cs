using System.CommandLine;
using System.CommandLine.IO;
using System.IO;

namespace Art.Tesler.Tests;

public class TestConsole : IConsole
{
    private readonly int _windowWidth;
    private readonly bool _outputRedirected;
    private readonly bool _errorRedirected;
    private readonly bool _inputRedirected;

    public TestConsole(TextWriter outWriter, TextWriter errorWriter, int windowWidth, bool outputRedirected, bool errorRedirected, bool inputRedirected)
    {
        _windowWidth = windowWidth;
        _outputRedirected = outputRedirected;
        _errorRedirected = errorRedirected;
        _inputRedirected = inputRedirected;
        Out = new TextWriterStandardStreamWriter(outWriter);
        Error = new TextWriterStandardStreamWriter(errorWriter);
    }

    /// <inheritdoc />
    public IStandardStreamWriter Error { get; }

    /// <inheritdoc />
    public bool IsErrorRedirected => _errorRedirected;

    /// <inheritdoc />
    public IStandardStreamWriter Out { get; }

    /// <inheritdoc />
    public bool IsOutputRedirected => _outputRedirected;

    /// <inheritdoc />
    public bool IsInputRedirected => _inputRedirected;

    internal int GetWindowWidth() => _windowWidth;
}
