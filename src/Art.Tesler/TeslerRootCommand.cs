using System.CommandLine;
using Art.Common;
using Art.Tesler.Config;
using Art.Tesler.Cookie;
using Art.Tesler.Database;
using Art.Tesler.Profiles;
using Art.Tesler.Properties;

namespace Art.Tesler;

public class TeslerRootCommand : RootCommand
{
    protected IArtifactToolRegistryStore PluginStore;

    public TeslerRootCommand(IArtifactToolRegistryStore pluginStore)
    {
        PluginStore = pluginStore;
    }

    public static TeslerRootCommand Create(
        IToolLogHandlerProvider toolLogHandlerProvider,
        IArtifactToolRegistryStore pluginStore,
        IWritableScopedRunnerPropertyProvider runnerPropertyProvider,
        IWritableScopedToolPropertyProvider toolPropertyProvider,
        ITeslerDataProvider dataProvider,
        ITeslerRegistrationProvider registrationProvider,
        IProfileResolver profileResolver)
    {
        return new TeslerRootCommand(pluginStore)
        {
            new ArcCommand(toolLogHandlerProvider, pluginStore, toolPropertyProvider, dataProvider, registrationProvider, profileResolver),
            new DumpCommand(toolLogHandlerProvider, pluginStore, toolPropertyProvider, dataProvider, registrationProvider),
            new FindCommand(toolLogHandlerProvider, pluginStore, toolPropertyProvider),
            new ListCommand(toolLogHandlerProvider, pluginStore, toolPropertyProvider),
            new StreamCommand(toolLogHandlerProvider, pluginStore, toolPropertyProvider, profileResolver),
            new RehashCommand(toolLogHandlerProvider, dataProvider, registrationProvider),
            new ToolsCommand(toolLogHandlerProvider, pluginStore),
            new ValidateCommand(toolLogHandlerProvider, pluginStore, toolPropertyProvider, dataProvider, registrationProvider),
            new DatabaseCommand(toolLogHandlerProvider, registrationProvider),
            new CookieCommand(toolLogHandlerProvider),
            new ConfigCommand(toolLogHandlerProvider, runnerPropertyProvider, toolPropertyProvider, profileResolver, pluginStore)
        };
    }

    public static TeslerRootCommand CreateSinglePlugin<TTool>(
        IToolLogHandlerProvider toolLogHandlerProvider,
        IWritableScopedRunnerPropertyProvider runnerPropertyProvider,
        IWritableScopedToolPropertyProvider toolPropertyProvider,
        ITeslerDataProvider dataProvider,
        ITeslerRegistrationProvider registrationProvider,
        IProfileResolver profileResolver)
        where TTool : IArtifactToolFactory
    {
        var registry = new ArtifactToolRegistry();
        registry.Add<TTool>();
        var pluginStore = new StaticArtifactToolRegistryStore(registry);
        return new TeslerRootCommand(pluginStore)
        {
            new ArcCommand(toolLogHandlerProvider, pluginStore, toolPropertyProvider, dataProvider, registrationProvider, profileResolver),
            new DumpCommand(toolLogHandlerProvider, pluginStore, toolPropertyProvider, dataProvider, registrationProvider),
            new FindCommand(toolLogHandlerProvider, pluginStore, toolPropertyProvider),
            new ListCommand(toolLogHandlerProvider, pluginStore, toolPropertyProvider),
            new StreamCommand(toolLogHandlerProvider, pluginStore, toolPropertyProvider, profileResolver),
            new RehashCommand(toolLogHandlerProvider, dataProvider, registrationProvider),
            new ToolsCommand(toolLogHandlerProvider, pluginStore),
            new ValidateCommand(toolLogHandlerProvider, pluginStore, toolPropertyProvider, dataProvider, registrationProvider),
            new DatabaseCommand(toolLogHandlerProvider, registrationProvider),
            new CookieCommand(toolLogHandlerProvider),
            new ConfigCommand(toolLogHandlerProvider, runnerPropertyProvider, toolPropertyProvider, profileResolver, pluginStore)
        };
    }

    public static TeslerRootCommand CreateSingleSelectablePlugin<TTool>(
        IToolLogHandlerProvider toolLogHandlerProvider,
        IWritableScopedRunnerPropertyProvider runnerPropertyProvider,
        IWritableScopedToolPropertyProvider toolPropertyProvider,
        ITeslerDataProvider dataProvider,
        ITeslerRegistrationProvider registrationProvider,
        IProfileResolver profileResolver)
        where TTool : IArtifactToolFactory, IArtifactToolSelector<string>
    {
        var registry = new ArtifactToolRegistry();
        registry.AddSelectable<TTool>();
        var pluginStore = new StaticArtifactToolRegistryStore(registry);
        return new TeslerRootCommand(pluginStore)
        {
            new ArcCommand(toolLogHandlerProvider, pluginStore, toolPropertyProvider, dataProvider, registrationProvider, profileResolver),
            new DumpCommand(toolLogHandlerProvider, pluginStore, toolPropertyProvider, dataProvider, registrationProvider),
            new FindCommand(toolLogHandlerProvider, pluginStore, toolPropertyProvider),
            new ListCommand(toolLogHandlerProvider, pluginStore, toolPropertyProvider),
            new StreamCommand(toolLogHandlerProvider, pluginStore, toolPropertyProvider, profileResolver),
            new RehashCommand(toolLogHandlerProvider, dataProvider, registrationProvider),
            new ToolsCommand(toolLogHandlerProvider, pluginStore),
            new ValidateCommand(toolLogHandlerProvider, pluginStore, toolPropertyProvider, dataProvider, registrationProvider),
            new DatabaseCommand(toolLogHandlerProvider, registrationProvider),
            new CookieCommand(toolLogHandlerProvider),
            new ConfigCommand(toolLogHandlerProvider, runnerPropertyProvider, toolPropertyProvider, profileResolver, pluginStore)
        };
    }

    public static TeslerRootCommand Create(
        IToolLogHandlerProvider toolLogHandlerProvider,
        IArtifactToolRegistry artifactToolRegistry,
        IWritableScopedRunnerPropertyProvider runnerPropertyProvider,
        IWritableScopedToolPropertyProvider toolPropertyProvider,
        ITeslerDataProvider dataProvider,
        ITeslerRegistrationProvider registrationProvider,
        IProfileResolver profileResolver)
    {
        return Create(
            toolLogHandlerProvider,
            new StaticArtifactToolRegistryStore(artifactToolRegistry),
            runnerPropertyProvider,
            toolPropertyProvider,
            dataProvider,
            registrationProvider,
            profileResolver);
    }
}
