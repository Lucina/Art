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
    protected Option<bool> ProfileOption;
    protected Option<bool> AllOption;
    protected Option<string> InputOption;
    protected Option<string> ToolOption;
    protected Option<bool> EffectiveOption;
    protected Option<bool> IgnoreBaseTypesOption;
    protected Option<bool> SimpleOption;

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
        InputOption = new Option<string>(new[] { "-i", "--input" }, "Profile for which to get configuration properties")
        {
            ArgumentHelpName = "profile-path"
        };
        AddOption(InputOption);
        LocalOption = new Option<bool>(new[] { "-l", "--local" }, "Get properties in local option scope");
        AddOption(LocalOption);
        GlobalOption = new Option<bool>(new[] { "-g", "--global" }, "Get properties in global option scope");
        AddOption(GlobalOption);
        ProfileOption = new Option<bool>(new[] { "-p", "--profile" }, "Get properties in profile option scope");
        AddOption(ProfileOption);
        AllOption = new Option<bool>(new[] { "-a", "--all" }, "Get properties in all option scopes");
        AddOption(AllOption);
        EffectiveOption = new Option<bool>(new[] { "-e", "--effective" }, "Gets effective config values (default)");
        AddOption(EffectiveOption);
        IgnoreBaseTypesOption = new Option<bool>(new[] { "--ignore-base-types" }, "(Tools and profiles) Ignores base types");
        AddOption(IgnoreBaseTypesOption);
        SimpleOption = new Option<bool>(new[] { "-s", "--simple" }, "Use simple output format (key=value)");
        AddOption(SimpleOption);
        AddValidator(result =>
        {
            var optionSet = new HashSet<Option>();
            if (result.GetValueForOption(ToolOption) != null)
            {
                optionSet.Add(ToolOption);
            }

            if (result.GetValueForOption(InputOption) != null)
            {
                optionSet.Add(InputOption);
            }

            if (optionSet.Count > 1)
            {
                result.ErrorMessage = $"Only one option from {CommandHelper.GetOptionAliasList(new Option[] { ToolOption, InputOption })} may be specified";
                return;
            }

            bool anyScopeSpecifiers = result.GetValueForOption(AllOption) || result.GetValueForOption(LocalOption) || result.GetValueForOption(GlobalOption) || result.GetValueForOption(ProfileOption);

            if (result.GetValueForOption(EffectiveOption))
            {
                if (anyScopeSpecifiers || result.GetValueForOption(IgnoreBaseTypesOption))
                {
                    result.ErrorMessage = $"{CommandHelper.GetOptionAlias(EffectiveOption)} may not be used with options {CommandHelper.GetOptionAliasList(new Option[] { AllOption, LocalOption, GlobalOption, IgnoreBaseTypesOption })}";
                    return;
                }
            }

            if (result.GetValueForOption(ProfileOption))
            {
                if (result.GetValueForOption(InputOption) == null)
                {
                    result.ErrorMessage = $"{CommandHelper.GetOptionAlias(ProfileOption)} must be used with {CommandHelper.GetOptionAlias(InputOption)}";
                    return;
                }
            }

            if (result.GetValueForOption(IgnoreBaseTypesOption))
            {
                if (result.GetValueForOption(ToolOption) == null && result.GetValueForOption(InputOption) == null)
                {
                    result.ErrorMessage = $"{CommandHelper.GetOptionAlias(IgnoreBaseTypesOption)} must be used with {CommandHelper.GetOptionAlias(ToolOption)} or {CommandHelper.GetOptionAlias(InputOption)}";
                    return;
                }
            }
        });
    }

    protected override Task<int> RunAsync(InvocationContext context)
    {
        PropertyFormatter propertyFormatter = context.ParseResult.GetValueForOption(SimpleOption)
            ? SimplePropertyFormatter.Instance
            : DefaultPropertyFormatter.Instance;
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
                        if (scopedListingSettings.IgnoreBaseTypes)
                        {
                            foreach (var v in _toolPropertyProvider.GetProperties(toolID, scopedListingSettings.ConfigScopeFlags))
                            {
                                ToolOutput.Out.WriteLine(propertyFormatter.FormatProperty(toolID, v));
                            }
                        }
                        else
                        {
                            foreach (var v in TeslerPropertyUtility.GetPropertiesDeep(_registryStore, _toolPropertyProvider, ToolOutput, toolID, scopedListingSettings.ConfigScopeFlags))
                            {
                                ToolOutput.Out.WriteLine(propertyFormatter.FormatProperty(toolID, v));
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
                            ToolOutput.Out.WriteLine(propertyFormatter.FormatProperty(toolID, v));
                        }
                        break;
                    }
                default:
                    throw new InvalidOperationException($"Invalid listing setting type {listingSettings?.GetType()}");
            }
            return Task.FromResult(0);
        }
        else if (context.ParseResult.HasOption(InputOption))
        {
            string profileString = context.ParseResult.GetValueForOption(InputOption)!;
            if (!_profileResolver.TryGetProfiles(profileString, out var profiles, ProfileResolutionFlags.Files))
            {
                PrintErrorMessage($"Unable to identify profile file {profileString}", ToolOutput);
                return Task.FromResult(2);
            }

            var profileList = profiles.ToList();
            for (int i = 0; i < profileList.Count; i++)
            {
                var profile = profileList[i];
                if (!ArtifactToolIDUtil.TryParseID(profile.Tool, out var toolID))
                {
                    PrintErrorMessage($"Unable to parse tool string \"{profile.Tool}\" in profile index {i}", ToolOutput);
                    return Task.FromResult(1);
                }
                string profileGroup = ConfigCommandUtility.GetGroupName(profile);
                switch (listingSettings)
                {
                    case ScopedListingSettings scopedListingSettings:
                        {
                            if (scopedListingSettings.IgnoreBaseTypes)
                            {
                                foreach (var v in _toolPropertyProvider.GetProperties(toolID, scopedListingSettings.ConfigScopeFlags))
                                {
                                    ToolOutput.Out.WriteLine(propertyFormatter.FormatProperty(i, profileGroup, toolID, v));
                                }
                            }
                            else
                            {
                                foreach (var v in TeslerPropertyUtility.GetPropertiesDeep(_registryStore, _toolPropertyProvider, ToolOutput, toolID, scopedListingSettings.ConfigScopeFlags))
                                {
                                    ToolOutput.Out.WriteLine(propertyFormatter.FormatProperty(i, profileGroup, toolID, v));
                                }
                            }
                            if ((scopedListingSettings.ConfigScopeFlags & ConfigScopeFlags.Profile) != 0 && profile.Options != null)
                            {
                                foreach (var v in profile.Options)
                                {
                                    ToolOutput.Out.WriteLine(propertyFormatter.FormatProperty(i, profileGroup, toolID, new ConfigProperty(ConfigScope.Profile, v.Key, v.Value)));
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
                            if (profile.Options != null)
                            {
                                foreach (var v in profile.Options)
                                {
                                    map[v.Key] = new ConfigProperty(ConfigScope.Profile, v.Key, v.Value);
                                }
                            }
                            foreach (var v in map.Values)
                            {
                                ToolOutput.Out.WriteLine(propertyFormatter.FormatProperty(i, profileGroup, toolID, v));
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
                            ToolOutput.Out.WriteLine(propertyFormatter.FormatProperty(v));
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
                            ToolOutput.Out.WriteLine(propertyFormatter.FormatProperty(v));
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
    private record ScopedListingSettings(ConfigScopeFlags ConfigScopeFlags, bool IgnoreBaseTypes) : ListingSettings;
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
        if (context.ParseResult.GetValueForOption(ProfileOption))
        {
            activeFlags = (activeFlags ?? ConfigScopeFlags.None) | ConfigScopeFlags.Profile;
        }
        if (activeFlags is { } flags)
        {
            return new ScopedListingSettings(flags, context.ParseResult.GetValueForOption(IgnoreBaseTypesOption));
        }
        return new EffectiveListingSettings();
    }
}
