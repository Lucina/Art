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
    protected Option<bool> ExactScopeOption;
    protected Option<bool> SimpleOption;

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
        ExactScopeOption = new Option<bool>(new[] { "-e", "--exact-scope" }, "Only check at the exact scope");
        AddOption(ExactScopeOption);
        SimpleOption = new Option<bool>(new[] { "-s", "--simple" }, "Use simple output format (key=value)");
        AddOption(SimpleOption);
    }

    protected override Task<int> RunAsync(InvocationContext context)
    {
        PropertyFormatter propertyFormatter = context.ParseResult.GetValueForOption(SimpleOption)
            ? SimplePropertyFormatter.Instance
            : DefaultPropertyFormatter.Instance;
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
            if (TeslerPropertyUtility.TryGetPropertyDeep(_registryStore, _toolPropertyProvider, ToolOutput, toolID, key, configScopeFlags, out var result))
            {
                ToolOutput.Out.WriteLine(propertyFormatter.FormatPropertyValue(result.Value));
            }
        }
        else if (context.ParseResult.HasOption(InputOption))
        {
            if (!TryGetProfilesWithIndex(_profileResolver, context, out var profiles, out int selectedIndex, out int errorCode))
            {
                return Task.FromResult(errorCode);
            }
            var profile = profiles[selectedIndex];
            if (!ArtifactToolIDUtil.TryParseID(profile.Tool, out var toolID))
            {
                PrintErrorMessage($"Unable to parse tool string \"{profile.Tool}\"", ToolOutput);
                return Task.FromResult(1);
            }
            if ((configScopeFlags & ConfigScopeFlags.Profile) != 0 && profile.Options is { } options && options.TryGetValue(key, out var profileValueResult))
            {
                ToolOutput.Out.WriteLine(propertyFormatter.FormatPropertyValue(profileValueResult));
            }
            else if (TeslerPropertyUtility.TryGetPropertyDeep(_registryStore, _toolPropertyProvider, ToolOutput, toolID, key, configScopeFlags, out var result))
            {
                ToolOutput.Out.WriteLine(propertyFormatter.FormatPropertyValue(result.Value));
            }
        }
        else
        {
            if (_runnerPropertyProvider.TryGetProperty(key, configScopeFlags, out var result))
            {
                ToolOutput.Out.WriteLine(propertyFormatter.FormatPropertyValue(result.Value));
            }
        }
        return Task.FromResult(0);
    }

    private ConfigScopeFlags GetConfigScopeFlags(InvocationContext context)
    {
        ConfigScope configScope = GetConfigScope(context);
        bool exactScope = context.ParseResult.GetValueForOption(ExactScopeOption);
        return configScope switch
        {
            ConfigScope.Global => exactScope ? ConfigScopeFlags.Global : ConfigScopeFlags.Global,
            ConfigScope.Local => exactScope ? ConfigScopeFlags.Local : ConfigScopeFlags.Local | ConfigScopeFlags.Global,
            ConfigScope.Profile => exactScope ? ConfigScopeFlags.Profile : ConfigScopeFlags.Local | ConfigScopeFlags.Global | ConfigScopeFlags.Profile,
            _ => throw new InvalidOperationException($"Invalid scope value {configScope}")
        };
    }
}
