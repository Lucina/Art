using System.CommandLine;
using Art.Common;
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
        IDefaultPropertyProvider defaultPropertyProvider,
        ITeslerDataProvider dataProvider,
        ITeslerRegistrationProvider registrationProvider,
        IProfileResolver profileResolver)
    {
        return new TeslerRootCommand(pluginStore)
        {
            new ArcCommand(toolLogHandlerProvider, pluginStore, defaultPropertyProvider, dataProvider, registrationProvider, profileResolver),
            new DumpCommand(toolLogHandlerProvider, pluginStore, defaultPropertyProvider, dataProvider, registrationProvider),
            new FindCommand(toolLogHandlerProvider, pluginStore, defaultPropertyProvider),
            new ListCommand(toolLogHandlerProvider, pluginStore, defaultPropertyProvider),
            new StreamCommand(toolLogHandlerProvider, pluginStore, defaultPropertyProvider, profileResolver),
            new RehashCommand(toolLogHandlerProvider, dataProvider, registrationProvider),
            new ToolsCommand(toolLogHandlerProvider, pluginStore),
            new ValidateCommand(toolLogHandlerProvider, pluginStore, defaultPropertyProvider, dataProvider, registrationProvider),
            new DatabaseCommand(toolLogHandlerProvider, registrationProvider),
            new CookieCommand(toolLogHandlerProvider)
        };
    }

    public static TeslerRootCommand CreateSinglePlugin<TTool>(
        IToolLogHandlerProvider toolLogHandlerProvider,
        IDefaultPropertyProvider defaultPropertyProvider,
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
            new ArcCommand(toolLogHandlerProvider, pluginStore, defaultPropertyProvider, dataProvider, registrationProvider, profileResolver),
            new DumpCommand(toolLogHandlerProvider, pluginStore, defaultPropertyProvider, dataProvider, registrationProvider),
            new FindCommand(toolLogHandlerProvider, pluginStore, defaultPropertyProvider),
            new ListCommand(toolLogHandlerProvider, pluginStore, defaultPropertyProvider),
            new StreamCommand(toolLogHandlerProvider, pluginStore, defaultPropertyProvider, profileResolver),
            new RehashCommand(toolLogHandlerProvider, dataProvider, registrationProvider),
            new ToolsCommand(toolLogHandlerProvider, pluginStore),
            new ValidateCommand(toolLogHandlerProvider, pluginStore, defaultPropertyProvider, dataProvider, registrationProvider),
            new DatabaseCommand(toolLogHandlerProvider, registrationProvider),
            new CookieCommand(toolLogHandlerProvider)
        };
    }

    public static TeslerRootCommand CreateSingleSelectablePlugin<TTool>(
        IToolLogHandlerProvider toolLogHandlerProvider,
        IDefaultPropertyProvider defaultPropertyProvider,
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
            new ArcCommand(toolLogHandlerProvider, pluginStore, defaultPropertyProvider, dataProvider, registrationProvider, profileResolver),
            new DumpCommand(toolLogHandlerProvider, pluginStore, defaultPropertyProvider, dataProvider, registrationProvider),
            new FindCommand(toolLogHandlerProvider, pluginStore, defaultPropertyProvider),
            new ListCommand(toolLogHandlerProvider, pluginStore, defaultPropertyProvider),
            new StreamCommand(toolLogHandlerProvider, pluginStore, defaultPropertyProvider, profileResolver),
            new RehashCommand(toolLogHandlerProvider, dataProvider, registrationProvider),
            new ToolsCommand(toolLogHandlerProvider, pluginStore),
            new ValidateCommand(toolLogHandlerProvider, pluginStore, defaultPropertyProvider, dataProvider, registrationProvider),
            new DatabaseCommand(toolLogHandlerProvider, registrationProvider),
            new CookieCommand(toolLogHandlerProvider)
        };
    }

    public static TeslerRootCommand Create(
        IToolLogHandlerProvider toolLogHandlerProvider,
        IArtifactToolRegistry artifactToolRegistry,
        IDefaultPropertyProvider defaultPropertyProvider,
        ITeslerDataProvider dataProvider,
        ITeslerRegistrationProvider registrationProvider,
        IProfileResolver profileResolver)
    {
        return Create(toolLogHandlerProvider, new StaticArtifactToolRegistryStore(artifactToolRegistry), defaultPropertyProvider, dataProvider, registrationProvider, profileResolver);
    }
}
