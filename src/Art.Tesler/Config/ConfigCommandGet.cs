using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using Art.Common;
using Art.Tesler.Properties;

namespace Art.Tesler.Config;

public class ConfigCommandGet : ConfigCommandGetSetBase
{
    private readonly IScopedRunnerPropertyProvider _runnerPropertyProvider;
    private readonly IScopedToolPropertyProvider _toolPropertyProvider;
    private readonly IProfileResolver _profileResolver;
    private readonly IArtifactToolRegistryStore _registryStore;
    protected Option<bool> EffectiveOption;

    public ConfigCommandGet(
        IOutputControl toolOutput,
        IScopedRunnerPropertyProvider runnerPropertyProvider,
        IScopedToolPropertyProvider toolPropertyProvider,
        IProfileResolver profileResolver,
        IArtifactToolRegistryStore registryStore,
        string name,
        string? description = null)
        : base(toolOutput, name, description)
    {
        _runnerPropertyProvider = runnerPropertyProvider;
        _toolPropertyProvider = toolPropertyProvider;
        _profileResolver = profileResolver;
        _registryStore = registryStore;
        EffectiveOption = new Option<bool>(new[] { "-e", "--effective" }, "Gets effective value at the specified scope");
        AddOption(EffectiveOption);
    }

    protected override Task<int> RunAsync(InvocationContext context)
    {
        ConfigScopeFlags configScopeFlags = GetConfigScopeFlags(context);
        string key = context.ParseResult.GetValueForArgument(KeyArgument);
        if (context.ParseResult.HasOption(ToolOption))
        {
            string toolString = context.ParseResult.GetValueForOption(ToolOption)!;
            if (!ArtifactToolIDUtil.TryParseID(toolString, out var toolID))
            {
                PrintErrorMessage($"Unable to parse tool string \"{toolString}\"", ToolOutput);
                return Task.FromResult(1);
            }
            // TODO implement
            throw new NotImplementedException();
        }
        else if (context.ParseResult.HasOption(ProfileOption))
        {
            // TODO implement
            throw new NotImplementedException();
        }
        else
        {
            if (_runnerPropertyProvider.TryGetProperty(key, configScopeFlags, out var result))
            {
                ToolOutput.Out.WriteLine(ConfigPropertyUtility.FormatPropertyForDisplay(result));
            }
        }
        return Task.FromResult(0);
    }

    private ConfigScopeFlags GetConfigScopeFlags(InvocationContext context)
    {
        ConfigScope configScope = GetConfigScope(context);
        bool effective = context.ParseResult.GetValueForOption(EffectiveOption);
        return configScope switch
        {
            ConfigScope.Global => effective ? ConfigScopeFlags.Local : ConfigScopeFlags.Global,
            ConfigScope.Local => effective ? ConfigScopeFlags.Local | ConfigScopeFlags.Global : ConfigScopeFlags.Local,
            ConfigScope.Profile => effective ? ConfigScopeFlags.Local | ConfigScopeFlags.Global | ConfigScopeFlags.Profile : ConfigScopeFlags.Profile,
            _ => throw new InvalidOperationException($"Invalid scope value {configScope}")
        };
    }
}
