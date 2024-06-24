using System.CommandLine;

namespace Art.Tesler.Config;

// TODO need new commands for new config system; tightly coupled to local/global config scope concept
public class ConfigCommand : Command
{
    public ConfigCommand(
        IOutputControl toolOutput,
        IToolPropertyProvider toolPropertyProvider,
        IProfileResolver profileResolver)
        : this(toolOutput, toolPropertyProvider, profileResolver, "config", "Perform operations on tool [default] options.")
    {
    }

    public ConfigCommand(
        IOutputControl toolOutput,
        IToolPropertyProvider toolPropertyProvider,
        IProfileResolver profileResolver,
        string name,
        string? description = null)
        : base(name, description)
    {
        AddCommand(new ConfigCommandList(toolOutput, toolPropertyProvider, profileResolver, "list", "Lists options."));
    }
}
