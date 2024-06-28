using System.CommandLine;

namespace Art.Tesler.Cookie;

public class CookieCommand : Command
{
    public CookieCommand(
        IOutputControl toolOutput)
        : this(toolOutput, "cookie", "Perform operations on browser cookies.")
    {
    }

    public CookieCommand(
        IOutputControl toolOutput,
        string name,
        string? description = null)
        : base(name, description)
    {
        AddCommand(new CookieCommandExtract(toolOutput, "extract", "Extracts cookies."));
    }
}
