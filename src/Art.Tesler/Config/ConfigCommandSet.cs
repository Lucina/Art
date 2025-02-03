﻿using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Text.Json;
using Art.Common;
using Art.Tesler.Profiles;
using Art.Tesler.Properties;

namespace Art.Tesler.Config;

public class ConfigCommandSet : ConfigCommandGetSetBase
{
    protected Argument<string> ValueArgument;
    private readonly IWritableScopedRunnerPropertyProvider _runnerPropertyProvider;
    private readonly IWritableScopedToolPropertyProvider _toolPropertyProvider;
    private readonly IProfileResolver _profileResolver;

    public ConfigCommandSet(
        IOutputControl toolOutput,
        IWritableScopedRunnerPropertyProvider runnerPropertyProvider,
        IWritableScopedToolPropertyProvider toolPropertyProvider,
        IProfileResolver profileResolver,
        string name,
        string? description = null)
        : base(toolOutput, name, description)
    {
        _runnerPropertyProvider = runnerPropertyProvider;
        _toolPropertyProvider = toolPropertyProvider;
        _profileResolver = profileResolver;
        ValueArgument = new Argument<string>("value", "Configuration property value") { HelpName = "value", Arity = ArgumentArity.ExactlyOne };
        AddArgument(ValueArgument);
    }

    protected override Task<int> RunAsync(InvocationContext context, CancellationToken cancellationToken)
    {
        ConfigScope configScope = GetConfigScope(context);
        string key = context.ParseResult.GetValueForArgument(KeyArgument);
        JsonElement value = Common.ParsePropToJsonElement(context.ParseResult.GetValueForArgument(ValueArgument));
        ConfigProperty property = new ConfigProperty(configScope, key, value);
        if (context.ParseResult.HasOption(ToolOption))
        {
            string toolString = context.ParseResult.GetValueForOption(ToolOption)!;
            if (!ArtifactToolIDUtil.TryParseID(toolString, out var toolID))
            {
                PrintErrorMessage($"Unable to parse tool string \"{toolString}\"", ToolOutput);
                return Task.FromResult(1);
            }
            switch (configScope)
            {
                case ConfigScope.Local:
                    if (!TrySetToolPropertyNonProfile(toolID, new ConfigProperty(ConfigScope.Local, key, value)))
                    {
                        return Task.FromResult(1);
                    }
                    break;
                case ConfigScope.Global:
                    if (!TrySetToolPropertyNonProfile(toolID, new ConfigProperty(ConfigScope.Global, key, value)))
                    {
                        return Task.FromResult(1);
                    }
                    break;
                case ConfigScope.Profile:
                default:
                    throw new InvalidOperationException($"Invalid config scope {configScope} for tool");
            }
        }
        else if (context.ParseResult.HasOption(InputOption))
        {
            if (!TryGetProfilesWithIndex(_profileResolver, context, out var profiles, out string profileString, out int selectedIndex, out int errorCode))
            {
                return Task.FromResult(errorCode);
            }
            if (profiles is not IWritableResolvedProfiles writableResolvedProfiles)
            {
                PrintErrorMessage($"Source for profiles in {profileString} is not writable", ToolOutput);
                return Task.FromResult(7);
            }
            var profile = profiles.Values[selectedIndex];
            if (!ArtifactToolIDUtil.TryParseID(profile.Tool, out var toolID))
            {
                PrintErrorMessage($"Unable to parse tool string \"{profile.Tool}\" for profile {selectedIndex} in {profileString}", ToolOutput);
                return Task.FromResult(1);
            }
            switch (configScope)
            {
                case ConfigScope.Local:
                    if (!TrySetToolPropertyNonProfile(toolID, new ConfigProperty(ConfigScope.Local, key, value)))
                    {
                        return Task.FromResult(1);
                    }
                    break;
                case ConfigScope.Global:
                    if (!TrySetToolPropertyNonProfile(toolID, new ConfigProperty(ConfigScope.Global, key, value)))
                    {
                        return Task.FromResult(1);
                    }
                    break;
                case ConfigScope.Profile:
                    List<ArtifactToolProfile> copy = new(profiles.Values)
                    {
                        [selectedIndex] = profile with { Options = TeslerPropertyUtility.GetOptionsMapWithAddedPair(profile.Options, key, value) }
                    };
                    writableResolvedProfiles.WriteProfiles(copy);
                    break;
                default:
                    throw new InvalidOperationException($"Invalid config scope {configScope} for profile {selectedIndex} in {profileString}");
            }
        }
        else
        {
            if (!TrySetRunnerProperty(property))
            {
                return Task.FromResult(1);
            }
        }
        return Task.FromResult(0);
    }

    private bool TrySetToolPropertyNonProfile(ArtifactToolID artifactToolId, ConfigProperty configProperty)
    {
        if (!_toolPropertyProvider.TrySetProperty(artifactToolId, configProperty))
        {
            PrintFailureToSet(configProperty);
            return false;
        }
        return true;
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
