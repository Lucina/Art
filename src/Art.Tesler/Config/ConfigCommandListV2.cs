using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Text;
using Art.Common;
using Art.Tesler.Properties;

namespace Art.Tesler.Config;

public class ConfigCommandListV2 : CommandBase
{
    private readonly IRunnerPropertyProvider _runnerPropertyProvider;
    private readonly IToolPropertyProvider _toolPropertyProvider;
    private readonly IProfileResolver _profileResolver;
    private readonly IArtifactToolRegistryStore _registryStore;

    protected Option<bool> LocalOption;
    protected Option<bool> GlobalOption;
    protected Option<bool> AllOption;
    protected Option<bool> SpecificOption;
    protected Option<string> ProfileOption;
    protected Option<string> ToolOption;

    public ConfigCommandListV2(
        IOutputControl toolOutput,
        IRunnerPropertyProvider runnerPropertyProvider,
        IToolPropertyProvider toolPropertyProvider,
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
        ToolOption = new Option<string>(new[] { "-t", "--tool" }, "Tool to get options for")
        {
            ArgumentHelpName = "tool-string"
        };
        AddOption(ToolOption);
        ProfileOption = new Option<string>(new[] { "-p", "--profile" }, "Profile to get options for")
        {
            ArgumentHelpName = "profile-path"
        };
        AddOption(ProfileOption);
        LocalOption = new Option<bool>(new[] { "-l", "--local" }, "Include local option scope");
        AddOption(LocalOption);
        GlobalOption = new Option<bool>(new[] { "-g", "--global" }, "Include global option scope");
        AddOption(GlobalOption);
        AllOption = new Option<bool>(new[] { "-a", "--all" }, "Include all option scopes");
        AddOption(AllOption);
        SpecificOption = new Option<bool>(new[] { "-s", "--specific" }, "(Tools only) Don't retrieve properties for base types");
        AddOption(SpecificOption);
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
                result.ErrorMessage = $"Only one option from {GetOptionAliasList(new Option[] { ToolOption, ProfileOption })} may be specified";
                return;
            }

            if (result.GetValueForOption(AllOption)
             || result.GetValueForOption(LocalOption)
             || result.GetValueForOption(GlobalOption))
            {
                if (result.GetValueForOption(SpecificOption))
                {
                    result.ErrorMessage = $"{GetOptionAlias(SpecificOption)} may not be specified when an option among {GetOptionAliasList(new Option[] { ToolOption, ProfileOption })} is specified";
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
                        // TODO implement
                        break;
                    }
                case EffectiveListingSettings:
                    {
                        // TODO implement
                        break;
                    }
            }
            return Task.FromResult(0);
        }
        else if (context.ParseResult.HasOption(ProfileOption))
        {
            switch (listingSettings)
            {
                case ScopedListingSettings scopedListingSettings:
                    {
                        // TODO implement
                        break;
                    }
                case EffectiveListingSettings:
                    {
                        // TODO implement
                        break;
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
                        // TODO implement
                        break;
                    }
                case EffectiveListingSettings:
                    {
                        // TODO implement
                        break;
                    }
            }
            return Task.FromResult(0);
        }
    }

    private record ListingSettings;
    private record ScopedListingSettings(ConfigScopeFlags ConfigScopeFlags, bool Specific) : ListingSettings;
    private record EffectiveListingSettings : ListingSettings;

    private ListingSettings GetListingSettings(InvocationContext context)
    {
        ConfigScopeFlags? activeFlags = null;
        if (context.ParseResult.HasOption(AllOption))
        {
            activeFlags = (activeFlags ?? ConfigScopeFlags.None) | ConfigScopeFlags.All;
        }
        if (context.ParseResult.HasOption(LocalOption))
        {
            activeFlags = (activeFlags ?? ConfigScopeFlags.None) | ConfigScopeFlags.Local;
        }
        if (context.ParseResult.HasOption(GlobalOption))
        {
            activeFlags = (activeFlags ?? ConfigScopeFlags.None) | ConfigScopeFlags.Global;
        }
        if (activeFlags is { } flags)
        {
            return new ScopedListingSettings(flags, context.ParseResult.HasOption(SpecificOption));
        }
        return new EffectiveListingSettings();
    }

    private static string GetOptionAlias(Option option)
    {
        return option.Aliases.FirstOrDefault() ?? option.Name;
    }

    private static string GetOptionAliasList(IEnumerable<Option> options, string separator = ", ")
    {
        return new StringBuilder()
            .AppendJoin(separator, options.Select(v => v.Aliases.FirstOrDefault() ?? v.Name))
            .ToString();
    }
}
