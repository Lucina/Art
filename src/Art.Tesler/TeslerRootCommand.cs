using System.CommandLine;
using Art.Common;
using Art.Tesler.Cookie;
using Art.Tesler.Database;

namespace Art.Tesler;

public class TeslerRootCommand : RootCommand
{
    public static TeslerRootCommand Create<TPluginStore>(
        TPluginStore pluginStore,
        IDefaultPropertyProvider defaultPropertyProvider,
        ITeslerDataProvider dataProvider,
        ITeslerRegistrationProvider registrationProvider)
        where TPluginStore : IArtifactToolRegistryStore
    {
        return new TeslerRootCommand<TPluginStore>(pluginStore)
        {
            new ArcCommand<TPluginStore>(pluginStore, defaultPropertyProvider, dataProvider, registrationProvider),
            new DumpCommand<TPluginStore>(pluginStore, defaultPropertyProvider, dataProvider, registrationProvider),
            new FindCommand<TPluginStore>(pluginStore, defaultPropertyProvider),
            new ListCommand<TPluginStore>(pluginStore, defaultPropertyProvider),
            new StreamCommand<TPluginStore>(pluginStore, defaultPropertyProvider),
            new RehashCommand(dataProvider, registrationProvider),
            new ToolsCommand<TPluginStore>(pluginStore),
            new ValidateCommand<TPluginStore>(pluginStore, defaultPropertyProvider, dataProvider, registrationProvider),
            new DatabaseCommand(registrationProvider),
            new CookieCommand()
        };
    }

    public static TeslerRootCommand CreateSinglePlugin<TTool>(
        IDefaultPropertyProvider defaultPropertyProvider,
        ITeslerDataProvider dataProvider,
        ITeslerRegistrationProvider registrationProvider)
        where TTool : IArtifactToolFactory
    {
        var registry = new ArtifactToolRegistry();
        registry.Add<TTool>();
        var pluginStore = new StaticArtifactToolRegistryStore(registry);
        return new TeslerRootCommand<StaticArtifactToolRegistryStore>(pluginStore)
        {
            new ArcCommand<StaticArtifactToolRegistryStore>(pluginStore, defaultPropertyProvider, dataProvider, registrationProvider),
            new DumpCommand<StaticArtifactToolRegistryStore>(pluginStore, defaultPropertyProvider, dataProvider, registrationProvider),
            new FindCommand<StaticArtifactToolRegistryStore>(pluginStore, defaultPropertyProvider),
            new ListCommand<StaticArtifactToolRegistryStore>(pluginStore, defaultPropertyProvider),
            new StreamCommand<StaticArtifactToolRegistryStore>(pluginStore, defaultPropertyProvider),
            new RehashCommand(dataProvider, registrationProvider),
            new ToolsCommand<StaticArtifactToolRegistryStore>(pluginStore),
            new ValidateCommand<StaticArtifactToolRegistryStore>(pluginStore, defaultPropertyProvider, dataProvider, registrationProvider),
            new DatabaseCommand(registrationProvider),
            new CookieCommand()
        };
    }

    public static TeslerRootCommand CreateSingleSelectablePlugin<TTool>(
        IDefaultPropertyProvider defaultPropertyProvider,
        ITeslerDataProvider dataProvider,
        ITeslerRegistrationProvider registrationProvider)
        where TTool : IArtifactToolFactory, IArtifactToolSelector<string>
    {
        var registry = new ArtifactToolRegistry();
        registry.AddSelectable<TTool>();
        var pluginStore = new StaticArtifactToolRegistryStore(registry);
        return new TeslerRootCommand<StaticArtifactToolRegistryStore>(pluginStore)
        {
            new ArcCommand<StaticArtifactToolRegistryStore>(pluginStore, defaultPropertyProvider, dataProvider, registrationProvider),
            new DumpCommand<StaticArtifactToolRegistryStore>(pluginStore, defaultPropertyProvider, dataProvider, registrationProvider),
            new FindCommand<StaticArtifactToolRegistryStore>(pluginStore, defaultPropertyProvider),
            new ListCommand<StaticArtifactToolRegistryStore>(pluginStore, defaultPropertyProvider),
            new StreamCommand<StaticArtifactToolRegistryStore>(pluginStore, defaultPropertyProvider),
            new RehashCommand(dataProvider, registrationProvider),
            new ToolsCommand<StaticArtifactToolRegistryStore>(pluginStore),
            new ValidateCommand<StaticArtifactToolRegistryStore>(pluginStore, defaultPropertyProvider, dataProvider, registrationProvider),
            new DatabaseCommand(registrationProvider),
            new CookieCommand()
        };
    }

    public static TeslerRootCommand Create(
        IArtifactToolRegistry artifactToolRegistry,
        IDefaultPropertyProvider defaultPropertyProvider,
        ITeslerDataProvider dataProvider,
        ITeslerRegistrationProvider registrationProvider)
    {
        return Create(new StaticArtifactToolRegistryStore(artifactToolRegistry), defaultPropertyProvider, dataProvider, registrationProvider);
    }
}

public class TeslerRootCommand<TPluginStore> : TeslerRootCommand where TPluginStore : IArtifactToolRegistryStore
{
    protected TPluginStore PluginStore;

    public TeslerRootCommand(TPluginStore pluginStore)
    {
        PluginStore = pluginStore;
    }
}
