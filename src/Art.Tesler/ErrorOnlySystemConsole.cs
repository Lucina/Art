using System.CommandLine;
using System.CommandLine.IO;

namespace Art.Tesler;

/// <summary>
/// Provides access to the standard streams via <see cref="System.Console"/>, but restricts output to the error stream.
/// </summary>
public class ErrorOnlySystemConsole : IConsole
{
    /// <summary>
    /// Initializes a new instance of <see cref="SystemConsole"/>.
    /// </summary>
    public ErrorOnlySystemConsole()
    {
        Error = StandardErrorStreamWriter.Instance;
        Out = StandardErrorStreamWriter.Instance;
    }

    /// <inheritdoc />
    public IStandardStreamWriter Error { get; }

    /// <inheritdoc />
    public bool IsErrorRedirected => Console.IsErrorRedirected;

    /// <inheritdoc />
    public IStandardStreamWriter Out { get; }

    /// <inheritdoc />
    public bool IsOutputRedirected => Console.IsErrorRedirected;

    /// <inheritdoc />
    public bool IsInputRedirected => Console.IsInputRedirected;

    internal int GetWindowWidth() => IsOutputRedirected ? int.MaxValue : Console.WindowWidth;

    private struct StandardErrorStreamWriter : IStandardStreamWriter
    {
        public static readonly StandardErrorStreamWriter Instance = new();

        public void Write(string? value) => Console.Error.Write(value);
    }
}
