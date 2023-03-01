using System.CommandLine.IO;
using System.IO;

namespace Art.Tesler.Tests;

public class TextWriterStandardStreamWriter : IStandardStreamWriter
{
    private readonly TextWriter _textWriter;

    public TextWriterStandardStreamWriter(TextWriter textWriter)
    {
        _textWriter = textWriter;
    }

    public void Write(string? value) => _textWriter.Write(value);
}
