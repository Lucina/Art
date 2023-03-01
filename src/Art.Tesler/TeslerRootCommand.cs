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
        IArtifactToolRegistryStore pluginStore,
        IDefaultPropertyProvider defaultPropertyProvider,
        ITeslerDataProvider dataProvider,
        ITeslerRegistrationProvider registrationProvider)
    {
        return new TeslerRootCommand(pluginStore)
        {
            new ArcCommand(pluginStore, defaultPropertyProvider, dataProvider, registrationProvider),
            new DumpCommand(pluginStore, defaultPropertyProvider, dataProvider, registrationProvider),
            new FindCommand(pluginStore, defaultPropertyProvider),
            new ListCommand(pluginStore, defaultPropertyProvider),
            new StreamCommand(pluginStore, defaultPropertyProvider),
            new RehashCommand(dataProvider, registrationProvider),
            new ToolsCommand(pluginStore),
            new ValidateCommand(pluginStore, defaultPropertyProvider, dataProvider, registrationProvider),
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
        return new TeslerRootCommand(pluginStore)
        {
            new ArcCommand(pluginStore, defaultPropertyProvider, dataProvider, registrationProvider),
            new DumpCommand(pluginStore, defaultPropertyProvider, dataProvider, registrationProvider),
            new FindCommand(pluginStore, defaultPropertyProvider),
            new ListCommand(pluginStore, defaultPropertyProvider),
            new StreamCommand(pluginStore, defaultPropertyProvider),
            new RehashCommand(dataProvider, registrationProvider),
            new ToolsCommand(pluginStore),
            new ValidateCommand(pluginStore, defaultPropertyProvider, dataProvider, registrationProvider),
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
        return new TeslerRootCommand(pluginStore)
        {
            new ArcCommand(pluginStore, defaultPropertyProvider, dataProvider, registrationProvider),
            new DumpCommand(pluginStore, defaultPropertyProvider, dataProvider, registrationProvider),
            new FindCommand(pluginStore, defaultPropertyProvider),
            new ListCommand(pluginStore, defaultPropertyProvider),
            new StreamCommand(pluginStore, defaultPropertyProvider),
            new RehashCommand(dataProvider, registrationProvider),
            new ToolsCommand(pluginStore),
            new ValidateCommand(pluginStore, defaultPropertyProvider, dataProvider, registrationProvider),
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
