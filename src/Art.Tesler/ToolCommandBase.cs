using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using Art.Common;
using Art.Common.Management;

namespace Art.Tesler;

public abstract class ToolCommandBase<TPluginStore> : CommandBase where TPluginStore : IArtifactToolRegistryStore
{
    protected TPluginStore PluginStore;

    protected IDefaultPropertyProvider DefaultPropertyProvider;

    protected Option<string> UserAgentOption;

    protected Option<string> CookieFileOption;

    protected Option<List<string>> PropertiesOption;

    protected ToolCommandBase(TPluginStore pluginStore, IDefaultPropertyProvider defaultPropertyProvider, string name, string? description = null) : base(name, description)
    {
        PluginStore = pluginStore;
        DefaultPropertyProvider = defaultPropertyProvider;
        UserAgentOption = new Option<string>(new[] { "--user-agent" }, "Custom user agent string") { ArgumentHelpName = "user-agent" };
        AddOption(UserAgentOption);
        CookieFileOption = new Option<string>(new[] { "--cookie-file" }, "Cookie file") { ArgumentHelpName = "file" };
        AddOption(CookieFileOption);
        PropertiesOption = new Option<List<string>>(new[] { "-p", "--property" }, "Add a property") { ArgumentHelpName = "key:value", Arity = ArgumentArity.ZeroOrMore };
        AddOption(PropertiesOption);
    }

    protected async Task<IArtifactTool> GetSearchingToolAsync(InvocationContext context, ArtifactToolProfile artifactToolProfile, CancellationToken cancellationToken = default)
    {
        return await GetToolAsync(context, artifactToolProfile, new InMemoryArtifactRegistrationManager(), new NullArtifactDataManager(), cancellationToken);
    }

    protected async Task<IArtifactTool> GetToolAsync(InvocationContext context, ArtifactToolProfile artifactToolProfile, IArtifactRegistrationManager arm, IArtifactDataManager adm, CancellationToken cancellationToken = default)
    {
        var plugin = PluginStore.LoadRegistry(ArtifactToolProfileUtil.GetID(artifactToolProfile.Tool));
        string? cookieFile = context.ParseResult.HasOption(CookieFileOption) ? context.ParseResult.GetValueForOption(CookieFileOption) : null;
        string? userAgent = context.ParseResult.HasOption(UserAgentOption) ? context.ParseResult.GetValueForOption(UserAgentOption) : null;
        IEnumerable<string> properties = context.ParseResult.HasOption(PropertiesOption) ? context.ParseResult.GetValueForOption(PropertiesOption)! : Array.Empty<string>();
        artifactToolProfile = artifactToolProfile.GetWithConsoleOptions(DefaultPropertyProvider, properties, cookieFile, userAgent);
        IArtifactTool t = await ArtifactTool.PrepareToolAsync(plugin, artifactToolProfile, arm, adm, cancellationToken);
        return t;
    }
}
