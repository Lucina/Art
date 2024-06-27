using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics.CodeAnalysis;
using Art.Tesler.Properties;

namespace Art.Tesler.Config;

public abstract class ConfigCommandGetSetBase : CommandBase
{
    protected const int ErrorProfileLoad = 2;
    protected const int ErrorProfileIndexUnspecified = 3;
    protected const int ErrorProfileIndexInvalid = 4;
    protected Option<bool> LocalOption;
    protected Option<bool> GlobalOption;
    protected Option<bool> ProfileOption;
    protected Option<string> InputOption;
    protected Option<string> ToolOption;
    protected Argument<string> KeyArgument;
    protected Option<int?> ProfileIndexOption;

    protected ConfigCommandGetSetBase(
        IOutputControl toolOutput,
        string name,
        string? description = null)
    : base(toolOutput, name, description)
    {
        ToolOption = new Option<string>(new[] { "-t", "--tool" }, "Tool for which to get/set configuration property")
        {
            ArgumentHelpName = "tool-string"
        };
        AddOption(ToolOption);
        InputOption = new Option<string>(new[] { "-i", "--input" }, "Profile for which to get/set configuration property")
        {
            ArgumentHelpName = "profile-path"
        };
        AddOption(InputOption);
        LocalOption = new Option<bool>(new[] { "-l", "--local" }, "Use local option scope");
        AddOption(LocalOption);
        GlobalOption = new Option<bool>(new[] { "-g", "--global" }, "Use global option scope");
        AddOption(GlobalOption);
        ProfileOption = new Option<bool>(new[] { "-p", "--profile" }, "Use profile option scope");
        AddOption(ProfileOption);
        KeyArgument = new Argument<string>("key", "Configuration property key") { HelpName = "key", Arity = ArgumentArity.ExactlyOne };
        AddArgument(KeyArgument);
        ProfileIndexOption = new Option<int?>(new[] { "--profile-index" }, "Profile index");
        AddOption(ProfileIndexOption);
        AddValidator(result =>
        {
            HashSet<Option> optionSet = new();
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

            optionSet.Clear();

            if (result.GetValueForOption(LocalOption))
            {
                optionSet.Add(LocalOption);
            }
            if (result.GetValueForOption(GlobalOption))
            {
                optionSet.Add(GlobalOption);
            }
            if (result.GetValueForOption(ProfileOption))
            {
                optionSet.Add(ProfileOption);
            }

            if (optionSet.Count > 1)
            {
                result.ErrorMessage = $"Only one option from {CommandHelper.GetOptionAliasList(new Option[] { LocalOption, GlobalOption, ProfileOption })} may be specified";
                return;
            }

            if (result.GetValueForOption(ProfileOption) && result.GetValueForOption(InputOption) == null)
            {
                result.ErrorMessage = $"{CommandHelper.GetOptionAlias(ProfileOption)} may not be used without {CommandHelper.GetOptionAlias(InputOption)}";
                return;
            }

            if (result.GetValueForOption(ProfileIndexOption) != null && result.GetValueForOption(InputOption) == null)
            {
                result.ErrorMessage = $"{CommandHelper.GetOptionAlias(ProfileIndexOption)} may not be used without {CommandHelper.GetOptionAlias(InputOption)}";
                return;
            }

            if (result.GetValueForOption(InputOption) != null)
            {
                if (result.GetValueForOption(LocalOption) && result.GetValueForOption(GlobalOption))
                {
                    result.ErrorMessage = $"{CommandHelper.GetOptionAliasList(new Option[] { LocalOption, GlobalOption })} may not be specified when {CommandHelper.GetOptionAlias(InputOption)} is specified";
                    return;
                }
            }
        });
    }

    protected bool TryGetProfilesWithIndex(
        IProfileResolver profileResolver,
        InvocationContext context,
        [NotNullWhen(true)] out List<ArtifactToolProfile>? profileList,
        out int selectedIndex,
        out int errorCode)
    {
        string profileString = context.ParseResult.GetValueForOption(InputOption)!;
        if (!profileResolver.TryGetProfiles(profileString, out var profiles, ProfileResolutionFlags.Files))
        {
            PrintErrorMessage($"Unable to identify profile file {profileString}", ToolOutput);
            profileList = null;
            selectedIndex = default;
            errorCode = ErrorProfileLoad;
            return false;
        }

        profileList = profiles.ToList();
        if (context.ParseResult.GetValueForOption(ProfileIndexOption) is { } profileIndexResult)
        {
            selectedIndex = profileIndexResult;
        }
        else if (profileList.Count != 1)
        {
            PrintErrorMessage($"There are {profileList.Count} profiles in profile file {profileString} - select one with {CommandHelper.GetOptionAlias(ProfileIndexOption)}", ToolOutput);
            selectedIndex = default;
            errorCode = ErrorProfileIndexUnspecified;
            return false;
        }
        else
        {
            selectedIndex = 0;
        }
        if ((uint)selectedIndex >= profileList.Count)
        {
            PrintErrorMessage($"{CommandHelper.GetOptionAlias(ProfileIndexOption)} {selectedIndex} is out of range for {profileList.Count} profiles in profile file {profileString}", ToolOutput);
            errorCode = ErrorProfileIndexInvalid;
            return false;
        }
        errorCode = 0;
        return true;
    }

    protected ConfigScope GetConfigScope(InvocationContext context)
    {
        if (context.ParseResult.GetValueForOption(ProfileOption))
        {
            return ConfigScope.Profile;
        }
        if (context.ParseResult.GetValueForOption(GlobalOption))
        {
            return ConfigScope.Global;
        }
        if (context.ParseResult.GetValueForOption(LocalOption))
        {
            return ConfigScope.Local;
        }
        return ConfigScope.Local;
    }
}
