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
        IOutputPair toolOutput,
        IArtifactToolRegistryStore pluginStore,
        IDefaultPropertyProvider defaultPropertyProvider,
        IToolLogHandlerProvider toolLogHandlerProvider,
        ITeslerDataProvider dataProvider,
        ITeslerRegistrationProvider registrationProvider,
        IProfileResolver profileResolver)
    {
        return new TeslerRootCommand(pluginStore)
        {
            new ArcCommand(toolOutput, pluginStore, defaultPropertyProvider, toolLogHandlerProvider, dataProvider, registrationProvider, profileResolver),
            new DumpCommand(toolOutput, pluginStore, defaultPropertyProvider, toolLogHandlerProvider, dataProvider, registrationProvider),
            new FindCommand(toolOutput, pluginStore, defaultPropertyProvider, toolLogHandlerProvider),
            new ListCommand(toolOutput, pluginStore, defaultPropertyProvider, toolLogHandlerProvider),
            new StreamCommand(toolOutput, pluginStore, defaultPropertyProvider, toolLogHandlerProvider, profileResolver),
            new RehashCommand(toolOutput, dataProvider, registrationProvider),
            new ToolsCommand(toolOutput, pluginStore),
            new ValidateCommand(toolOutput, pluginStore, defaultPropertyProvider, toolLogHandlerProvider, dataProvider, registrationProvider),
            new DatabaseCommand(toolOutput, registrationProvider),
            new CookieCommand(toolOutput)
        };
    }

    public static TeslerRootCommand CreateSinglePlugin<TTool>(
        IOutputPair toolOutput,
        IDefaultPropertyProvider defaultPropertyProvider,
        IToolLogHandlerProvider toolLogHandlerProvider,
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
            new ArcCommand(toolOutput, pluginStore, defaultPropertyProvider, toolLogHandlerProvider, dataProvider, registrationProvider, profileResolver),
            new DumpCommand(toolOutput, pluginStore, defaultPropertyProvider, toolLogHandlerProvider, dataProvider, registrationProvider),
            new FindCommand(toolOutput, pluginStore, defaultPropertyProvider, toolLogHandlerProvider),
            new ListCommand(toolOutput, pluginStore, defaultPropertyProvider, toolLogHandlerProvider),
            new StreamCommand(toolOutput, pluginStore, defaultPropertyProvider, toolLogHandlerProvider, profileResolver),
            new RehashCommand(toolOutput, dataProvider, registrationProvider),
            new ToolsCommand(toolOutput, pluginStore),
            new ValidateCommand(toolOutput, pluginStore, defaultPropertyProvider, toolLogHandlerProvider, dataProvider, registrationProvider),
            new DatabaseCommand(toolOutput, registrationProvider),
            new CookieCommand(toolOutput)
        };
    }

    public static TeslerRootCommand CreateSingleSelectablePlugin<TTool>(
        IOutputPair toolOutput,
        IDefaultPropertyProvider defaultPropertyProvider,
        IToolLogHandlerProvider toolLogHandlerProvider,
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
            new ArcCommand(toolOutput, pluginStore, defaultPropertyProvider, toolLogHandlerProvider, dataProvider, registrationProvider, profileResolver),
            new DumpCommand(toolOutput, pluginStore, defaultPropertyProvider, toolLogHandlerProvider, dataProvider, registrationProvider),
            new FindCommand(toolOutput, pluginStore, defaultPropertyProvider, toolLogHandlerProvider),
            new ListCommand(toolOutput, pluginStore, defaultPropertyProvider, toolLogHandlerProvider),
            new StreamCommand(toolOutput, pluginStore, defaultPropertyProvider, toolLogHandlerProvider, profileResolver),
            new RehashCommand(toolOutput, dataProvider, registrationProvider),
            new ToolsCommand(toolOutput, pluginStore),
            new ValidateCommand(toolOutput, pluginStore, defaultPropertyProvider, toolLogHandlerProvider, dataProvider, registrationProvider),
            new DatabaseCommand(toolOutput, registrationProvider),
            new CookieCommand(toolOutput)
        };
    }

    public static TeslerRootCommand Create(
        IOutputPair toolOutput,
        IArtifactToolRegistry artifactToolRegistry,
        IDefaultPropertyProvider defaultPropertyProvider,
        IToolLogHandlerProvider toolLogHandlerProvider,
        ITeslerDataProvider dataProvider,
        ITeslerRegistrationProvider registrationProvider,
        IProfileResolver profileResolver)
    {
        return Create(toolOutput, new StaticArtifactToolRegistryStore(artifactToolRegistry), defaultPropertyProvider, toolLogHandlerProvider, dataProvider, registrationProvider, profileResolver);
    }
}
