using System.CommandLine;

namespace Art.Tesler.Config;

public class ConfigCommand : Command
{
    public ConfigCommand(
        IOutputPair toolOutput,
        IDefaultPropertyProvider defaultPropertyProvider,
        IProfileResolver profileResolver)
        : this(toolOutput, defaultPropertyProvider, profileResolver, "config", "Perform operations on tool [default] options.")
    {
    }

    public ConfigCommand(
        IOutputPair toolOutput,
        IDefaultPropertyProvider defaultPropertyProvider,
        IProfileResolver profileResolver,
        string name,
        string? description = null)
        : base(name, description)
    {
        AddCommand(new ConfigCommandList(toolOutput, defaultPropertyProvider, profileResolver, "list", "Lists options."));
    }
}
