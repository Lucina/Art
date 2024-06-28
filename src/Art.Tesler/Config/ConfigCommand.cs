using System.CommandLine;
using Art.Tesler.Profiles;
using Art.Tesler.Properties;

namespace Art.Tesler.Config;

public class ConfigCommand : Command
{
    public ConfigCommand(
        IOutputControl toolOutput,
        IWritableScopedRunnerPropertyProvider runnerPropertyProvider,
        IWritableScopedToolPropertyProvider toolPropertyProvider,
        IProfileResolver profileResolver,
        IArtifactToolRegistryStore registryStore)
        : this(
            toolOutput,
            runnerPropertyProvider,
            toolPropertyProvider,
            profileResolver,
            registryStore,
            "config",
            "Perform operations on configuration properties.")
    {
    }

    public ConfigCommand(
        IOutputControl toolOutput,
        IWritableScopedRunnerPropertyProvider runnerPropertyProvider,
        IWritableScopedToolPropertyProvider toolPropertyProvider,
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
            "Lists configuration properties."));
        AddCommand(new ConfigCommandGet(
            toolOutput,
            runnerPropertyProvider,
            toolPropertyProvider,
            profileResolver,
            registryStore,
            "get",
            "Gets a configuration property."));
        AddCommand(new ConfigCommandSet(
            toolOutput,
            runnerPropertyProvider,
            toolPropertyProvider,
            profileResolver,
            "set",
            "Sets a configuration property."));
        AddCommand(new ConfigCommandUnset(
            toolOutput,
            runnerPropertyProvider,
            toolPropertyProvider,
            profileResolver,
            "unset",
            "Unsets a configuration property."));
    }
}
