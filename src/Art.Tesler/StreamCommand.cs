using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using Art.Common;
using Art.Common.Management;
using Art.Common.Proxies;

namespace Art.Tesler;

public class StreamCommand : ToolCommandBase
{
    protected IProfileResolver ProfileResolver;

    protected Argument<string> ProfileFileArg;

    public StreamCommand(
        IToolLogHandlerProvider toolLogHandlerProvider,
        IArtifactToolRegistryStore pluginStore,
        IDefaultPropertyProvider defaultPropertyProvider,
        IProfileResolver profileResolver)
        : this(toolLogHandlerProvider, pluginStore, defaultPropertyProvider, profileResolver, "stream", "Stream primary resource to standard output.")
    {
    }

    public StreamCommand(
        IToolLogHandlerProvider toolLogHandlerProvider,
        IArtifactToolRegistryStore pluginStore,
        IDefaultPropertyProvider defaultPropertyProvider,
        IProfileResolver profileResolver,
        string name,
        string? description = null)
        : base(toolLogHandlerProvider, pluginStore, defaultPropertyProvider, name, description)
    {
        ProfileResolver = profileResolver;
        ProfileFileArg = new Argument<string>("profile", "Profile file") { HelpName = "profile", Arity = ArgumentArity.ExactlyOne };
        AddArgument(ProfileFileArg);
    }

    protected override async Task<int> RunAsync(InvocationContext context)
    {
        IToolLogHandler l = ToolLogHandlerProvider.GetStreamToolLogHandler();
        List<ArtifactToolProfile> profiles = new();
        ResolveAndAddProfiles(ProfileResolver, profiles, context.ParseResult.GetValueForArgument(ProfileFileArg));
        if (profiles.Count == 0)
        {
            throw new ArtUserException("No profiles were loaded from specified inputs, this command requires exactly one");
        }
        if (profiles.Count != 1)
        {
            throw new ArtUserException("Multiple profiles were loaded from specified inputs, this command requires exactly one");
        }
        var profile = profiles[0];
        string? cookieFile = context.ParseResult.HasOption(CookieFileOption) ? context.ParseResult.GetValueForOption(CookieFileOption) : null;
        string? userAgent = context.ParseResult.HasOption(UserAgentOption) ? context.ParseResult.GetValueForOption(UserAgentOption) : null;
        IEnumerable<string> properties = context.ParseResult.HasOption(PropertiesOption) ? context.ParseResult.GetValueForOption(PropertiesOption)! : Array.Empty<string>();
        var defaultPropertyProvider = GetOptionalDefaultPropertyProvider(context);
        profile = profile.GetWithConsoleOptions(defaultPropertyProvider, properties, cookieFile, userAgent, ToolOutput);
        var plugin = PluginStore.LoadRegistry(ArtifactToolProfileUtil.GetID(profile.Tool));
        using var arm = new InMemoryArtifactRegistrationManager();
        using var adm = new InMemoryArtifactDataManager();
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
            l.Log($"Primary resource {primaryResource} does not support exporting, this command requires this functionality", null, LogLevel.Error);
        }
        await using var output = ToolLogHandlerProvider.GetOutStream();
        await primaryResource.ExportStreamAsync(output).ConfigureAwait(false);
        return 0;
    }
}
