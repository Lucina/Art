using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using Art.Tesler.Properties;

namespace Art.Tesler.Config;

public abstract class ConfigCommandGetSetBase : CommandBase
{
    protected Option<bool> LocalOption;
    protected Option<bool> GlobalOption;
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
        LocalOption = new Option<bool>(new[] { "-l", "--local" }, "Use local option scope (default)");
        AddOption(LocalOption);
        GlobalOption = new Option<bool>(new[] { "-g", "--global" }, "Use global option scope");
        AddOption(GlobalOption);
        KeyArgument = new Argument<string>("key", "Configuration property key") { HelpName = "key", Arity = ArgumentArity.ExactlyOne };
        AddArgument(KeyArgument);
        AddValidator(result =>
        {
            if (result.GetValueForOption(LocalOption) && result.GetValueForOption(GlobalOption))
            {
                result.ErrorMessage = $"Only one option from {CommandHelper.GetOptionAliasList(new Option[] { LocalOption, GlobalOption })} may be specified";
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
