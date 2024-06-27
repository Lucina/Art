using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Text;
using System.Text.Json;
using Art.Common;
using Art.Tesler.Properties;

namespace Art.Tesler.Config;

public class ConfigCommandList : CommandBase
{
    private readonly IScopedRunnerPropertyProvider _runnerPropertyProvider;
    private readonly IScopedToolPropertyProvider _toolPropertyProvider;
    private readonly IProfileResolver _profileResolver;
    private readonly IArtifactToolRegistryStore _registryStore;

    protected Option<bool> LocalOption;
    protected Option<bool> GlobalOption;
    protected Option<bool> AllOption;
    protected Option<bool> SpecificOption;
    protected Option<string> ProfileOption;
    protected Option<string> ToolOption;
    protected Option<bool> EffectiveOption;

    public ConfigCommandList(
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
        ToolOption = new Option<string>(new[] { "-t", "--tool" }, "Tool for which to get configuration properties")
        {
            ArgumentHelpName = "tool-string"
        };
        AddOption(ToolOption);
        ProfileOption = new Option<string>(new[] { "-p", "--profile" }, "Profile for which to get configuration properties")
        {
            ArgumentHelpName = "profile-path"
        };
        AddOption(ProfileOption);
        LocalOption = new Option<bool>(new[] { "-l", "--local" }, "Get properties in local option scope");
        AddOption(LocalOption);
        GlobalOption = new Option<bool>(new[] { "-g", "--global" }, "Get properties in global option scope");
        AddOption(GlobalOption);
        AllOption = new Option<bool>(new[] { "-a", "--all" }, "Get properties in all option scopes");
        AddOption(AllOption);
        // TODO rework specificity, would be nice to select profile-only for profile and exclude-base for tools
        SpecificOption = new Option<bool>(new[] { "-s", "--specific" }, "(Tools only) Don't retrieve properties for base types");
        AddOption(SpecificOption);
        EffectiveOption = new Option<bool>(new[] { "-e", "--effective" }, "Gets effective config values (default)");
        AddOption(EffectiveOption);
        AddValidator(result =>
        {
            var optionSet = new HashSet<Option>();
            if (result.GetValueForOption(ToolOption) != null)
            {
                optionSet.Add(ToolOption);
            }

            if (result.GetValueForOption(ProfileOption) != null)
            {
                optionSet.Add(ProfileOption);
            }

            if (optionSet.Count > 0)
            {
                result.ErrorMessage = $"Only one option from {CommandHelper.GetOptionAliasList(new Option[] { ToolOption, ProfileOption })} may be specified";
                return;
            }

            bool anyScopeSpecifiers = result.GetValueForOption(AllOption) || result.GetValueForOption(LocalOption) || result.GetValueForOption(GlobalOption);

            if (result.GetValueForOption(EffectiveOption))
            {
                if (anyScopeSpecifiers)
                {
                    result.ErrorMessage = $"{CommandHelper.GetOptionAlias(EffectiveOption)} may not be used with options {CommandHelper.GetOptionAliasList(new Option[] { AllOption, LocalOption, GlobalOption })}";
                    return;
                }
            }

            if (result.GetValueForOption(SpecificOption))
            {
                if (result.GetValueForOption(ToolOption) == null)
                {
                    result.ErrorMessage = $"{CommandHelper.GetOptionAlias(SpecificOption)} must be used with {CommandHelper.GetOptionAlias(ToolOption)}";
                    return;
                }

                if (!anyScopeSpecifiers)
                {
                    result.ErrorMessage = $"{CommandHelper.GetOptionAlias(SpecificOption)} may not be specified without an option among {CommandHelper.GetOptionAliasList(new Option[] { AllOption, LocalOption, GlobalOption })}";
                    return;
                }
            }
        });
    }

    protected override Task<int> RunAsync(InvocationContext context)
    {
        ListingSettings listingSettings = GetListingSettings(context);
        if (context.ParseResult.HasOption(ToolOption))
        {
            string toolString = context.ParseResult.GetValueForOption(ToolOption)!;
            if (!ArtifactToolIDUtil.TryParseID(toolString, out var toolID))
            {
                PrintErrorMessage($"Unable to parse tool string \"{toolString}\"", ToolOutput);
                return Task.FromResult(1);
            }
            switch (listingSettings)
            {
                case ScopedListingSettings scopedListingSettings:
                    {
                        if (scopedListingSettings.Specific)
                        {
                            foreach (var v in _toolPropertyProvider.GetProperties(toolID, scopedListingSettings.ConfigScopeFlags))
                            {
                                ToolOutput.Out.WriteLine(ConfigPropertyUtility.FormatPropertyForDisplay(toolID, v));
                            }
                        }
                        else
                        {
                            foreach (var v in TeslerPropertyUtility.GetPropertiesDeep(_registryStore, _toolPropertyProvider, ToolOutput, toolID, scopedListingSettings.ConfigScopeFlags))
                            {
                                ToolOutput.Out.WriteLine(ConfigPropertyUtility.FormatPropertyForDisplay(toolID, v));
                            }
                        }
                        break;
                    }
                case EffectiveListingSettings:
                    {
                        var map = new Dictionary<string, ConfigProperty>();
                        foreach (var v in TeslerPropertyUtility.GetPropertiesDeep(_registryStore, _toolPropertyProvider, ToolOutput, toolID, ConfigScopeFlags.All))
                        {
                            map[v.Key] = v;
                        }
                        foreach (var v in map.Values)
                        {
                            ToolOutput.Out.WriteLine(ConfigPropertyUtility.FormatPropertyForDisplay(toolID, v));
                        }
                        break;
                    }
                default:
                    throw new InvalidOperationException($"Invalid listing setting type {listingSettings?.GetType()}");
            }
            return Task.FromResult(0);
        }
        else if (context.ParseResult.HasOption(ProfileOption))
        {
            string profileString = context.ParseResult.GetValueForOption(ProfileOption)!;
            if (!_profileResolver.TryGetProfiles(profileString, out var profiles, ProfileResolutionFlags.Files))
            {
                PrintErrorMessage($"Unable to identify profile file {profileString}", ToolOutput);
                return Task.FromResult(2);
            }

            var profileList = profiles.ToList();
            for (int i = 0; i < profileList.Count; i++)
            {
                var profile = profileList[i];
                string profileGroup = profile.Group ?? "<unspecified>";
                if (!ArtifactToolIDUtil.TryParseID(profile.Tool, out var toolID))
                {
                    PrintErrorMessage($"Unable to parse tool string \"{profile.Tool}\" in profile index {i}", ToolOutput);
                    return Task.FromResult(1);
                }
                switch (listingSettings)
                {
                    case ScopedListingSettings scopedListingSettings:
                        {
                            if (scopedListingSettings.Specific)
                            {
                                foreach (var v in _toolPropertyProvider.GetProperties(toolID, scopedListingSettings.ConfigScopeFlags))
                                {
                                    ToolOutput.Out.WriteLine(ConfigPropertyUtility.FormatPropertyForDisplay(i, profileGroup, toolID, v));
                                }
                            }
                            else
                            {
                                foreach (var v in TeslerPropertyUtility.GetPropertiesDeep(_registryStore, _toolPropertyProvider, ToolOutput, toolID, scopedListingSettings.ConfigScopeFlags))
                                {
                                    ToolOutput.Out.WriteLine(ConfigPropertyUtility.FormatPropertyForDisplay(i, profileGroup, toolID, v));
                                }
                            }
                            break;
                        }
                    case EffectiveListingSettings:
                        {
                            var map = new Dictionary<string, ConfigProperty>();
                            foreach (var v in TeslerPropertyUtility.GetPropertiesDeep(_registryStore, _toolPropertyProvider, ToolOutput, toolID, ConfigScopeFlags.All))
                            {
                                map[v.Key] = v;
                            }
                            foreach (var v in map.Values)
                            {
                                ToolOutput.Out.WriteLine(ConfigPropertyUtility.FormatPropertyForDisplay(i, profileGroup, toolID, v));
                            }
                            break;
                        }
                    default:
                        throw new InvalidOperationException($"Invalid listing setting type {listingSettings?.GetType()}");
                }
            }
            return Task.FromResult(0);
        }
        else
        {
            switch (listingSettings)
            {
                case ScopedListingSettings scopedListingSettings:
                    {
                        foreach (var v in _runnerPropertyProvider.GetProperties(scopedListingSettings.ConfigScopeFlags))
                        {
                            ToolOutput.Out.WriteLine(ConfigPropertyUtility.FormatPropertyForDisplay(v));
                        }
                        break;
                    }
                case EffectiveListingSettings:
                    {
                        Dictionary<string, ConfigProperty> map = new();
                        foreach (var v in _runnerPropertyProvider.GetProperties(ConfigScopeFlags.All))
                        {
                            map[v.Key] = v;
                        }
                        foreach (var v in map.Values)
                        {
                            ToolOutput.Out.WriteLine(ConfigPropertyUtility.FormatPropertyForDisplay(v));
                        }
                        break;
                    }
                default:
                    throw new InvalidOperationException($"Invalid listing setting type {listingSettings?.GetType()}");
            }
            return Task.FromResult(0);
        }
    }

    private record ListingSettings;
    private record ScopedListingSettings(ConfigScopeFlags ConfigScopeFlags, bool Specific) : ListingSettings;
    private record EffectiveListingSettings : ListingSettings;

    private ListingSettings GetListingSettings(InvocationContext context)
    {
        if (context.ParseResult.GetValueForOption(EffectiveOption))
        {
            return new EffectiveListingSettings();
        }
        ConfigScopeFlags? activeFlags = null;
        if (context.ParseResult.GetValueForOption(AllOption))
        {
            activeFlags = (activeFlags ?? ConfigScopeFlags.None) | ConfigScopeFlags.All;
        }
        if (context.ParseResult.GetValueForOption(LocalOption))
        {
            activeFlags = (activeFlags ?? ConfigScopeFlags.None) | ConfigScopeFlags.Local;
        }
        if (context.ParseResult.GetValueForOption(GlobalOption))
        {
            activeFlags = (activeFlags ?? ConfigScopeFlags.None) | ConfigScopeFlags.Global;
        }
        if (activeFlags is { } flags)
        {
            return new ScopedListingSettings(flags, context.ParseResult.GetValueForOption(SpecificOption));
        }
        return new EffectiveListingSettings();
    }
}
