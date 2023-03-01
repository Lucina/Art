namespace Art.Tesler;

public class TextWriterOutputPair : IOutputPair
{
    public TextWriter Out { get; }

    public TextWriter Error { get; }

    public TextWriterOutputPair(TextWriter outWriter, TextWriter errorWriter)
    {
        Out = outWriter;
        Error = errorWriter;
    }
}
