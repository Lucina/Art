using System.CommandLine;
using System.CommandLine.IO;

namespace Art.Tesler;

/// <summary>
/// Provides access to a console that always writes to a single <see cref="TextWriter"/>.
/// </summary>
public class ErrorOnlyConsole : IConsole
{
    private readonly Func<bool> _isOutputRedirected;
    private readonly Func<bool> _isInputRedirected;
    private readonly Func<int> _windowWidth;

    /// <summary>
    /// Initializes a new instance of <see cref="ErrorOnlyConsole"/>.
    /// </summary>
    public ErrorOnlyConsole(TextWriter textWriter, Func<bool> isOutputRedirected, Func<bool> isInputRedirected, Func<int> windowWidth)
    {
        Out = Error = new TextWriterStandardStreamWriter(textWriter);
        _isOutputRedirected = isOutputRedirected;
        _isInputRedirected = isInputRedirected;
        _windowWidth = windowWidth;
    }

    /// <inheritdoc />
    public IStandardStreamWriter Error { get; }

    /// <inheritdoc />
    public bool IsErrorRedirected => _isOutputRedirected();

    /// <inheritdoc />
    public IStandardStreamWriter Out { get; }

    /// <inheritdoc />
    public bool IsOutputRedirected => _isOutputRedirected();

    /// <inheritdoc />
    public bool IsInputRedirected => _isInputRedirected();

    internal int GetWindowWidth() => _isOutputRedirected() ? int.MaxValue : _windowWidth();

    private class TextWriterStandardStreamWriter : IStandardStreamWriter
    {
        private readonly TextWriter _textWriter;

        public TextWriterStandardStreamWriter(TextWriter textWriter)
        {
            _textWriter = textWriter;
        }

        public void Write(string? value) => _textWriter.Write(value);
    }
}
