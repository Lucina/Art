using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using Art.Common;

namespace Art.Tesler;

public abstract class ToolCommandBase : CommandBase
{
    protected IArtifactToolRegistryStore PluginStore;

    protected IDefaultPropertyProvider DefaultPropertyProvider;

    protected IToolLogHandlerProvider ToolLogHandlerProvider;

    protected Option<string> UserAgentOption;

    protected Option<string> CookieFileOption;

    protected Option<List<string>> PropertiesOption;

    protected ToolCommandBase(
        IOutputPair toolOutput,
        IArtifactToolRegistryStore pluginStore,
        IDefaultPropertyProvider defaultPropertyProvider,
        IToolLogHandlerProvider toolLogHandlerProvider,
        string name,
        string? description = null) : base(toolOutput, name, description)
    {
        PluginStore = pluginStore;
        ToolOutput = toolOutput;
        DefaultPropertyProvider = defaultPropertyProvider;
        ToolLogHandlerProvider = toolLogHandlerProvider;
        UserAgentOption = new Option<string>(new[] { "--user-agent" }, "Custom user agent string") { ArgumentHelpName = "user-agent" };
        AddOption(UserAgentOption);
        CookieFileOption = new Option<string>(new[] { "--cookie-file" }, "Cookie file") { ArgumentHelpName = "file" };
        AddOption(CookieFileOption);
        PropertiesOption = new Option<List<string>>(new[] { "-p", "--property" }, "Add a property") { ArgumentHelpName = "key:value", Arity = ArgumentArity.ZeroOrMore };
        AddOption(PropertiesOption);
    }

    protected async Task<IArtifactTool> GetSearchingToolAsync(InvocationContext context, ArtifactToolProfile artifactToolProfile, IArtifactRegistrationManager artifactRegistrationManager, IArtifactDataManager artifactDataManager, CancellationToken cancellationToken = default)
    {
        return await GetToolAsync(context, artifactToolProfile, artifactRegistrationManager, artifactDataManager, cancellationToken);
    }

    protected async Task<IArtifactTool> GetToolAsync(InvocationContext context, ArtifactToolProfile artifactToolProfile, IArtifactRegistrationManager arm, IArtifactDataManager adm, CancellationToken cancellationToken = default)
    {
        var plugin = PluginStore.LoadRegistry(ArtifactToolProfileUtil.GetID(artifactToolProfile.Tool));
        string? cookieFile = context.ParseResult.HasOption(CookieFileOption) ? context.ParseResult.GetValueForOption(CookieFileOption) : null;
        string? userAgent = context.ParseResult.HasOption(UserAgentOption) ? context.ParseResult.GetValueForOption(UserAgentOption) : null;
        IEnumerable<string> properties = context.ParseResult.HasOption(PropertiesOption) ? context.ParseResult.GetValueForOption(PropertiesOption)! : Array.Empty<string>();
        artifactToolProfile = artifactToolProfile.GetWithConsoleOptions(DefaultPropertyProvider, properties, cookieFile, userAgent, ToolOutput);
        IArtifactTool t = await ArtifactTool.PrepareToolAsync(plugin, artifactToolProfile, arm, adm, cancellationToken);
        return t;
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
