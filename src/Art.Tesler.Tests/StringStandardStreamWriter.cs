using System.CommandLine.IO;
using System.IO;

namespace Art.Tesler.Tests;

public class StringStandardStreamWriter : IStandardStreamWriter
{
    public readonly StringWriter StringWriter;

    public StringStandardStreamWriter()
    {
        StringWriter = new StringWriter();
    }

    public void Write(string? value) => StringWriter.Write(value);
}
