using System.Collections.Immutable;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Text;
using System.Text.Json;
using Art.Common;

namespace Art.Tesler.Config;

public class ConfigCommandList : CommandBase
{
    private readonly IToolPropertyProvider _toolPropertyProvider;
    private readonly IProfileResolver _profileResolver;

    protected Option<string> DefaultsOption;
    protected Option<string> ProfileOption;
    protected Option<bool> SimpleOption;
    protected Option<bool> IncludeDefaultsOption;

    public ConfigCommandList(
        IOutputControl toolOutput,
        IToolPropertyProvider toolPropertyProvider,
        IProfileResolver profileResolver,
        string name,
        string? description = null)
        : base(toolOutput, name, description)
    {
        _toolPropertyProvider = toolPropertyProvider;
        _profileResolver = profileResolver;
        DefaultsOption = new Option<string>(new[] { "-d", "--defaults" }, "Tool to get defaults for")
        {
            ArgumentHelpName = "tool-string"
        };
        AddOption(DefaultsOption);
        ProfileOption = new Option<string>(new[] { "-p", "--profile" }, "Profile to get options for")
        {
            ArgumentHelpName = "profile-path"
        };
        AddOption(ProfileOption);
        SimpleOption = new Option<bool>(new[] { "-s", "--simple" }, "Print simple output");
        AddOption(SimpleOption);
        IncludeDefaultsOption =
            new Option<bool>(new[] { "-i", "--include-defaults" }, "Include default options for profile");
        AddOption(IncludeDefaultsOption);
        AddValidator(result =>
        {
            var optionSet = new HashSet<Option>();
            if (result.GetValueForOption(DefaultsOption) != null)
            {
                optionSet.Add(DefaultsOption);
            }

            if (result.GetValueForOption(ProfileOption) != null)
            {
                optionSet.Add(ProfileOption);
            }

            var all = new Option[] { DefaultsOption, ProfileOption };
            switch (optionSet.Count)
            {
                case 0:
                    result.ErrorMessage = $"One filter from {GetOptionAliasList(all)} must be specified";
                    return;
                case > 1:
                    result.ErrorMessage = $"Exactly one filter from {GetOptionAliasList(all)} must be specified";
                    return;
            }

            if (result.GetValueForOption(ProfileOption) == null && result.GetValueForOption(IncludeDefaultsOption))
            {
                result.ErrorMessage =
                    $"Cannot use {IncludeDefaultsOption.Aliases.FirstOrDefault()} with {optionSet.FirstOrDefault()}";
            }
        });
    }

    protected override Task<int> RunAsync(InvocationContext context)
    {
        bool simple = context.ParseResult.HasOption(SimpleOption);
        bool includeDefaults = context.ParseResult.HasOption(IncludeDefaultsOption);
        if (context.ParseResult.HasOption(DefaultsOption))
        {
            string toolString = context.ParseResult.GetValueForOption(DefaultsOption)!;
            if (!ArtifactToolIDUtil.TryParseID(toolString, out var toolID))
            {
                PrintErrorMessage($"Unable to parse tool string \"{toolString}\"", ToolOutput);
                return Task.FromResult(1);
            }

            var properties = new Dictionary<string, JsonElement>();
            TeslerPropertyUtility.ApplyProperties(_toolPropertyProvider, properties, toolID);
            WriteOutput($"Default properties for {toolID.GetToolString()}", properties, simple);
            return Task.FromResult(0);
        }

        if (context.ParseResult.HasOption(ProfileOption))
        {
            string profileString = context.ParseResult.GetValueForOption(ProfileOption)!;
            if (!_profileResolver.TryGetProfiles(profileString, out var profiles, ProfileResolutionFlags.Files))
            {
                PrintErrorMessage($"Unable to identify profile file {profileString}", ToolOutput);
                return Task.FromResult(2);
            }

            var profileList = profiles.ToList();
            if (profileList.Count == 1)
            {
                if (includeDefaults)
                {
                    var profile = profileList[0];
                    var properties = new Dictionary<string, JsonElement>();
                    TeslerPropertyUtility.ApplyProperties(_toolPropertyProvider, properties, profile.GetID());
                    if (profile.Options is { } options)
                    {
                        foreach (var pair in options)
                        {
                            properties[pair.Key] = pair.Value;
                        }
                    }

                    WriteOutput($"Properties for {profileString}", properties, simple);
                }
                else
                {
                    WriteOutput($"Properties for {profileString}",
                        profileList[0].Options ?? ImmutableDictionary<string, JsonElement>.Empty, simple);
                }
            }
            else
            {
                for (int i = 0; i < profileList.Count; i++)
                {
                    if (includeDefaults)
                    {
                        var profile = profileList[i];
                        var properties = new Dictionary<string, JsonElement>();
                        TeslerPropertyUtility.ApplyProperties(_toolPropertyProvider, properties, profile.GetID());
                        if (profile.Options is { } options)
                        {
                            foreach (var pair in options)
                            {
                                properties[pair.Key] = pair.Value;
                            }
                        }

                        WriteOutput($"Properties for {profileString}:{i}", properties, simple);
                    }
                    else
                    {
                        WriteOutput($"Properties for {profileString}:{i}",
                            profileList[i].Options ?? ImmutableDictionary<string, JsonElement>.Empty, simple);
                    }
                }
            }

            return Task.FromResult(0);
        }

        return Task.FromResult(0);
    }

    protected void WriteOutput(string title, IEnumerable<KeyValuePair<string, JsonElement>> properties, bool simple)
    {
        if (simple)
        {
            foreach ((string? key, JsonElement value) in properties)
            {
                ToolOutput.Out.Write(key);
                ToolOutput.Out.Write("=");
                ToolOutput.Out.WriteLine(value.ToString());
            }
        }
        else
        {
            ToolOutput.Out.Write(title);
            ToolOutput.Out.WriteLine(":");
            foreach ((string? key, JsonElement value) in properties)
            {
                ToolOutput.Out.WriteLine($"- {key}: {value.ToString()}");
            }
        }
    }

    private static string GetOptionAliasList(IEnumerable<Option> options, string separator = ", ")
    {
        return new StringBuilder()
            .AppendJoin(separator, options.Select(v => v.Aliases.FirstOrDefault() ?? v.Name))
            .ToString();
    }
}
