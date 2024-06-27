using System.CommandLine;
using Art.Tesler.Properties;

namespace Art.Tesler.Config;

public class ConfigCommand : Command
{
    public ConfigCommand(
        IOutputControl toolOutput,
        IRunnerPropertyProvider runnerPropertyProvider,
        IToolPropertyProvider toolPropertyProvider,
        IProfileResolver profileResolver,
        IArtifactToolRegistryStore registryStore)
        : this(
            toolOutput,
            runnerPropertyProvider,
            toolPropertyProvider,
            profileResolver,
            registryStore,
            "config",
            "Perform operations on tool [default] options.")
    {
    }

    public ConfigCommand(
        IOutputControl toolOutput,
        IRunnerPropertyProvider runnerPropertyProvider,
        IToolPropertyProvider toolPropertyProvider,
        IProfileResolver profileResolver,
        IArtifactToolRegistryStore registryStore,
        string name,
        string? description = null)
        : base(name, description)
    {
        AddCommand(new ConfigCommandList(
            toolOutput,
            runnerPropertyProvider,
            toolPropertyProvider,
            profileResolver,
            registryStore,
            "list",
            "Lists options."));
    }
}
