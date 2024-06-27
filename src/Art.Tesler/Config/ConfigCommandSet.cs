using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Text.Json;
using Art.Common;
using Art.Tesler.Properties;

namespace Art.Tesler.Config;

public class ConfigCommandSet : ConfigCommandGetSetBase
{
    protected Argument<string> ValueArgument;
    private readonly IWritableScopedRunnerPropertyProvider _runnerPropertyProvider;
    private readonly IWritableScopedToolPropertyProvider _toolPropertyProvider;
    private readonly IProfileResolver _profileResolver;
    private readonly IArtifactToolRegistryStore _registryStore;

    public ConfigCommandSet(
        IOutputControl toolOutput,
        IWritableScopedRunnerPropertyProvider runnerPropertyProvider,
        IWritableScopedToolPropertyProvider toolPropertyProvider,
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
        ValueArgument = new Argument<string>("value", "Configuration property value") { HelpName = "value", Arity = ArgumentArity.ExactlyOne };
        AddArgument(ValueArgument);
    }

    protected override Task<int> RunAsync(InvocationContext context)
    {
        ConfigScope configScope = GetConfigScope(context);
        string key = context.ParseResult.GetValueForArgument(KeyArgument);
        JsonElement value = Common.ParsePropToJsonElement(context.ParseResult.GetValueForArgument(ValueArgument));
        ConfigProperty property = new ConfigProperty(configScope, key, value);
        bool anyFailure = false;
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
        else if (context.ParseResult.HasOption(InputOption))
        {
            // TODO implement
            throw new NotImplementedException();
        }
        else
        {
            if (!_runnerPropertyProvider.TrySetProperty(property))
            {
                PrintFailureToSet(property);
                return Task.FromResult(1);
            }
            anyFailure |= !TrySetRunnerProperty(property);
        }
        return Task.FromResult(anyFailure ? 1 : 0);
    }

    private bool TrySetToolProperty( ConfigProperty configProperty)
    {
        // TODO implement
        throw new NotImplementedException();
    }

    private bool TrySetRunnerProperty(ConfigProperty configProperty)
    {
        if (!_runnerPropertyProvider.TrySetProperty(configProperty))
        {
            PrintFailureToSet(configProperty);
            return false;
        }
        return true;

    }

    private void PrintFailureToSet(ConfigProperty property)
    {
        PrintErrorMessage($"Failed to set property {ConfigPropertyUtility.FormatPropertyKeyForDisplay(property.ConfigScope, property.Key)}", ToolOutput);
    }
}
