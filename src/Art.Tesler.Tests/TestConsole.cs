using System.CommandLine;
using System.CommandLine.IO;

namespace Art.Tesler.Tests;

public class TestConsole : IConsole
{
    public StringStandardStreamWriter StringOut;
    public StringStandardStreamWriter StringError;
    private readonly int _windowWidth;
    private readonly bool _outputRedirected;
    private readonly bool _errorRedirected;
    private readonly bool _inputRedirected;

    public TestConsole(int windowWidth, bool outputRedirected, bool errorRedirected, bool inputRedirected)
    {
        _windowWidth = windowWidth;
        _outputRedirected = outputRedirected;
        _errorRedirected = errorRedirected;
        _inputRedirected = inputRedirected;
        Out = StringOut = new StringStandardStreamWriter();
        Error = StringError = new StringStandardStreamWriter();
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
