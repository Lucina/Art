using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using Art.Tesler.Properties;

namespace Art.Tesler.Config;

public abstract class ConfigCommandGetSetBase : CommandBase
{
    protected Option<bool> LocalOption;
    protected Option<bool> GlobalOption;
    protected Option<bool> ProfileOption;
    protected Option<string> InputOption;
    protected Option<string> ToolOption;
    protected Argument<string> KeyArgument;

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
