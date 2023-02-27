using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using Art.Common;
using Art.Common.Management;
using Art.Common.Proxies;

namespace Art.Tesler;

internal class StreamCommand<TPluginStore> : ToolCommandBase<TPluginStore> where TPluginStore : IArtifactToolRegistryStore
{
    protected Argument<string> ProfileFileArg;

    private List<IArtifactToolSelectableRegistry<string>>? _selectableRegistries;

    public StreamCommand(TPluginStore pluginStore, IDefaultPropertyProvider defaultPropertyProvider) : this(pluginStore, defaultPropertyProvider, "arc", "Execute archival artifact tools.")
    {
    }

    public StreamCommand(TPluginStore pluginStore, IDefaultPropertyProvider defaultPropertyProvider, string name, string? description = null) : base(pluginStore, defaultPropertyProvider, name, description)
    {
        ProfileFileArg = new Argument<string>("profile", "Profile file") { HelpName = "profile", Arity = ArgumentArity.ExactlyOne };
        AddArgument(ProfileFileArg);
    }

    protected override async Task<int> RunAsync(InvocationContext context)
    {
        IToolLogHandler l = Common.GetStreamToolLogHandler();
            var profile = LoadProfile( context.ParseResult.GetValueForArgument(ProfileFileArg));
        string? cookieFile = context.ParseResult.HasOption(CookieFileOption) ? context.ParseResult.GetValueForOption(CookieFileOption) : null;
        string? userAgent = context.ParseResult.HasOption(UserAgentOption) ? context.ParseResult.GetValueForOption(UserAgentOption) : null;
        IEnumerable<string> properties = context.ParseResult.HasOption(PropertiesOption) ? context.ParseResult.GetValueForOption(PropertiesOption)! : Array.Empty<string>();
        profile = profile.GetWithConsoleOptions(DefaultPropertyProvider, properties, cookieFile, userAgent);
        var plugin = PluginStore.LoadRegistry(ArtifactToolProfileUtil.GetID(profile.Tool));
        var arm = NullArtifactRegistrationManager.Instance;
        var adm = NullArtifactDataManager.Instance;
        using IArtifactTool tool = await ArtifactTool.PrepareToolAsync(plugin, profile, arm, adm).ConfigureAwait(false);
        var listProxy = new ArtifactToolListProxy(tool, ArtifactToolListOptions.Default, l);
        var res = await listProxy.ListAsync().ToListAsync().ConfigureAwait(false);
        if (res.Count == 0)
        {
            l.Log($"No artifacts found for profile {profile}, this command requires exactly one", null, LogLevel.Error);
            return 77;
        }
        if (res.Count > 1)
        {
            l.Log($"Multiple artifacts found for profile {profile}, this command requires exactly one", null, LogLevel.Error);
            return 78;
        }
        var artifact = res[0];
        if (artifact.PrimaryResource is not { } primaryResource)
        {
            l.Log($"No primary resource available for artifact {artifact.Info.Key}, this command requires one", null, LogLevel.Error);
            return 79;
        }
        if (!primaryResource.CanExportStream)
        {
            l.Log($"Primary resource {primaryResource} does not support exporting, this command requires this functionality",null, LogLevel.Error);
        }
        await using var output = Console.OpenStandardOutput();
        await primaryResource.ExportStreamAsync(output).ConfigureAwait(false);
        return 0;
    }

    private ArtifactToolProfile LoadProfile(string profileFile)
    {
        if (File.Exists(profileFile))
        {
            return ArtifactToolProfileUtil.DeserializeProfilesFromFile(profileFile)[0];
        }
        if (_selectableRegistries == null)
        {
            _selectableRegistries = new List<IArtifactToolSelectableRegistry<string>>();
            foreach (var registry in PluginStore.LoadAllRegistries())
            {
                if (registry is IArtifactToolSelectableRegistry<string> selectableRegistry)
                {
                    _selectableRegistries.Add(selectableRegistry);
                }
            }
        }
        if (!PurificationUtil.TryIdentify(_selectableRegistries, profileFile, out var profile))
        {
            throw new FileNotFoundException(null, profileFile);
        }
        return profile;
    }
}
