using System.CommandLine;

namespace Art.Tesler.Config;

// TODO need new commands for new config system; tightly coupled to local/global config scope concept
public class ConfigCommand : Command
{
    public ConfigCommand(
        IOutputPair toolOutput,
        IToolDefaultPropertyProvider toolDefaultPropertyProvider,
        IProfileResolver profileResolver)
        : this(toolOutput, toolDefaultPropertyProvider, profileResolver, "config", "Perform operations on tool [default] options.")
    {
    }

    public ConfigCommand(
        IOutputPair toolOutput,
        IToolDefaultPropertyProvider toolDefaultPropertyProvider,
        IProfileResolver profileResolver,
        string name,
        string? description = null)
        : base(name, description)
    {
        AddCommand(new ConfigCommandList(toolOutput, toolDefaultPropertyProvider, profileResolver, "list", "Lists options."));
    }
}
