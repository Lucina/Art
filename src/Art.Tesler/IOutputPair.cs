namespace Art.Tesler;

public interface IOutputPair
{
    TextWriter Out { get; }

    TextWriter Error { get; }
}
