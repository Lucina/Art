using System.CommandLine;
using Art.Common;
using Art.Tesler.Config;
using Art.Tesler.Cookie;
using Art.Tesler.Database;

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
        IToolDefaultPropertyProvider toolDefaultPropertyProvider,
        ITeslerDataProvider dataProvider,
        ITeslerRegistrationProvider registrationProvider,
        IProfileResolver profileResolver)
    {
        return new TeslerRootCommand(pluginStore)
        {
            new ArcCommand(toolLogHandlerProvider, pluginStore, toolDefaultPropertyProvider, dataProvider, registrationProvider, profileResolver),
            new DumpCommand(toolLogHandlerProvider, pluginStore, toolDefaultPropertyProvider, dataProvider, registrationProvider),
            new FindCommand(toolLogHandlerProvider, pluginStore, toolDefaultPropertyProvider),
            new ListCommand(toolLogHandlerProvider, pluginStore, toolDefaultPropertyProvider),
            new StreamCommand(toolLogHandlerProvider, pluginStore, toolDefaultPropertyProvider, profileResolver),
            new RehashCommand(toolLogHandlerProvider, dataProvider, registrationProvider),
            new ToolsCommand(toolLogHandlerProvider, pluginStore),
            new ValidateCommand(toolLogHandlerProvider, pluginStore, toolDefaultPropertyProvider, dataProvider, registrationProvider),
            new DatabaseCommand(toolLogHandlerProvider, registrationProvider),
            new CookieCommand(toolLogHandlerProvider),
            new ConfigCommand(toolLogHandlerProvider, toolDefaultPropertyProvider, profileResolver)
        };
    }

    public static TeslerRootCommand CreateSinglePlugin<TTool>(
        IToolLogHandlerProvider toolLogHandlerProvider,
        IToolDefaultPropertyProvider toolDefaultPropertyProvider,
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
            new ArcCommand(toolLogHandlerProvider, pluginStore, toolDefaultPropertyProvider, dataProvider, registrationProvider, profileResolver),
            new DumpCommand(toolLogHandlerProvider, pluginStore, toolDefaultPropertyProvider, dataProvider, registrationProvider),
            new FindCommand(toolLogHandlerProvider, pluginStore, toolDefaultPropertyProvider),
            new ListCommand(toolLogHandlerProvider, pluginStore, toolDefaultPropertyProvider),
            new StreamCommand(toolLogHandlerProvider, pluginStore, toolDefaultPropertyProvider, profileResolver),
            new RehashCommand(toolLogHandlerProvider, dataProvider, registrationProvider),
            new ToolsCommand(toolLogHandlerProvider, pluginStore),
            new ValidateCommand(toolLogHandlerProvider, pluginStore, toolDefaultPropertyProvider, dataProvider, registrationProvider),
            new DatabaseCommand(toolLogHandlerProvider, registrationProvider),
            new CookieCommand(toolLogHandlerProvider),
            new ConfigCommand(toolLogHandlerProvider, toolDefaultPropertyProvider, profileResolver)
        };
    }

    public static TeslerRootCommand CreateSingleSelectablePlugin<TTool>(
        IToolLogHandlerProvider toolLogHandlerProvider,
        IToolDefaultPropertyProvider toolDefaultPropertyProvider,
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
            new ArcCommand(toolLogHandlerProvider, pluginStore, toolDefaultPropertyProvider, dataProvider, registrationProvider, profileResolver),
            new DumpCommand(toolLogHandlerProvider, pluginStore, toolDefaultPropertyProvider, dataProvider, registrationProvider),
            new FindCommand(toolLogHandlerProvider, pluginStore, toolDefaultPropertyProvider),
            new ListCommand(toolLogHandlerProvider, pluginStore, toolDefaultPropertyProvider),
            new StreamCommand(toolLogHandlerProvider, pluginStore, toolDefaultPropertyProvider, profileResolver),
            new RehashCommand(toolLogHandlerProvider, dataProvider, registrationProvider),
            new ToolsCommand(toolLogHandlerProvider, pluginStore),
            new ValidateCommand(toolLogHandlerProvider, pluginStore, toolDefaultPropertyProvider, dataProvider, registrationProvider),
            new DatabaseCommand(toolLogHandlerProvider, registrationProvider),
            new CookieCommand(toolLogHandlerProvider),
            new ConfigCommand(toolLogHandlerProvider, toolDefaultPropertyProvider, profileResolver)
        };
    }

    public static TeslerRootCommand Create(
        IToolLogHandlerProvider toolLogHandlerProvider,
        IArtifactToolRegistry artifactToolRegistry,
        IToolDefaultPropertyProvider toolDefaultPropertyProvider,
        ITeslerDataProvider dataProvider,
        ITeslerRegistrationProvider registrationProvider,
        IProfileResolver profileResolver)
    {
        return Create(toolLogHandlerProvider, new StaticArtifactToolRegistryStore(artifactToolRegistry), toolDefaultPropertyProvider, dataProvider, registrationProvider, profileResolver);
    }
}
