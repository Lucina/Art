using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using Art.Common;

namespace Art.Tesler;

public abstract class ToolCommandBase : CommandBase
{
    protected IToolLogHandlerProvider ToolLogHandlerProvider;

    protected IArtifactToolRegistryStore PluginStore;

    protected IDefaultPropertyProvider DefaultPropertyProvider;

    protected Option<string> UserAgentOption;

    protected Option<string> CookieFileOption;

    protected Option<List<string>> PropertiesOption;

    protected Option<bool> NoDefaultPropertiesOption;

    protected ToolCommandBase(
        IToolLogHandlerProvider toolLogHandlerProvider,
        IArtifactToolRegistryStore pluginStore,
        IDefaultPropertyProvider defaultPropertyProvider,
        string name,
        string? description = null) : base(toolLogHandlerProvider, name, description)
    {
        ToolLogHandlerProvider = toolLogHandlerProvider;
        PluginStore = pluginStore;
        DefaultPropertyProvider = defaultPropertyProvider;
        UserAgentOption = new Option<string>(new[] { "--user-agent" }, "Custom user agent string") { ArgumentHelpName = "user-agent" };
        AddOption(UserAgentOption);
        CookieFileOption = new Option<string>(new[] { "--cookie-file" }, "Cookie file") { ArgumentHelpName = "file" };
        AddOption(CookieFileOption);
        PropertiesOption = new Option<List<string>>(new[] { "-p", "--property" }, "Add a property") { ArgumentHelpName = "key:value", Arity = ArgumentArity.ZeroOrMore };
        AddOption(PropertiesOption);
        NoDefaultPropertiesOption = new Option<bool>(new[] { "--no-default-properties" }, "Don't apply default properties");
        AddOption(NoDefaultPropertiesOption);
    }

    protected ArtifactToolProfile PrepareProfile(InvocationContext context, ArtifactToolProfile artifactToolProfile)
    {
        string? cookieFile = context.ParseResult.HasOption(CookieFileOption) ? context.ParseResult.GetValueForOption(CookieFileOption) : null;
        string? userAgent = context.ParseResult.HasOption(UserAgentOption) ? context.ParseResult.GetValueForOption(UserAgentOption) : null;
        IEnumerable<string> properties = context.ParseResult.HasOption(PropertiesOption) ? context.ParseResult.GetValueForOption(PropertiesOption)! : Array.Empty<string>();
        var defaultPropertyProvider = GetOptionalDefaultPropertyProvider(context);
        return artifactToolProfile.GetWithConsoleOptions(defaultPropertyProvider, properties, cookieFile, userAgent, ToolOutput);
    }

    protected IEnumerable<ArtifactToolProfile> PrepareProfiles(InvocationContext context, IEnumerable<ArtifactToolProfile> artifactToolProfiles)
    {
        string? cookieFile = context.ParseResult.HasOption(CookieFileOption) ? context.ParseResult.GetValueForOption(CookieFileOption) : null;
        string? userAgent = context.ParseResult.HasOption(UserAgentOption) ? context.ParseResult.GetValueForOption(UserAgentOption) : null;
        IEnumerable<string> properties = context.ParseResult.HasOption(PropertiesOption) ? context.ParseResult.GetValueForOption(PropertiesOption)! : Array.Empty<string>();
        var defaultPropertyProvider = GetOptionalDefaultPropertyProvider(context);
        return artifactToolProfiles.Select(p => p.GetWithConsoleOptions(defaultPropertyProvider, properties, cookieFile, userAgent, ToolOutput));
    }

    protected async Task<IArtifactTool> GetToolAsync(ArtifactToolProfile artifactToolProfile, IArtifactRegistrationManager arm, IArtifactDataManager adm, CancellationToken cancellationToken = default)
    {
        var plugin = PluginStore.LoadRegistry(ArtifactToolProfileUtil.GetID(artifactToolProfile.Tool));
        return await ArtifactTool.PrepareToolAsync(plugin, artifactToolProfile, arm, adm, cancellationToken).ConfigureAwait(false);
    }

    protected IDefaultPropertyProvider? GetOptionalDefaultPropertyProvider(InvocationContext context)
    {
        if (context.ParseResult.GetValueForOption(NoDefaultPropertiesOption))
        {
            return null;
        }
        return DefaultPropertyProvider;
    }

    protected static void ResolveAndAddProfiles(IProfileResolver profileResolver, List<ArtifactToolProfile> profiles, string profileFile)
    {
        if (!profileResolver.TryGetProfiles(profileFile, out var profilesResult))
        {
            throw new ArtUserException($"Could not resolve profile for input \"{profileFile}\"");
        }
        profiles.AddRange(profilesResult);
    }
}
