using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using Art.Common;
using Art.Common.Management;
using Art.Common.Proxies;
using Art.Tesler.Properties;

namespace Art.Tesler;

public class ListCommand : ToolCommandBase
{
    protected TimeProvider TimeProvider;

    protected Option<string> ProfileFileOption;

    protected Option<bool> ListResourceOption;

    protected Option<string> ToolOption;

    protected Option<string> GroupOption;

    protected Option<bool> DetailedOption;

    public ListCommand(
        IToolLogHandlerProvider toolLogHandlerProvider,
        IArtifactToolRegistryStore pluginStore,
        IToolPropertyProvider toolPropertyProvider,
        TimeProvider timeProvider
    )
        : this(toolLogHandlerProvider, pluginStore, toolPropertyProvider, timeProvider, "list", "Execute artifact list tools.")
    {
    }

    public ListCommand(
        IToolLogHandlerProvider toolLogHandlerProvider,
        IArtifactToolRegistryStore pluginStore,
        IToolPropertyProvider toolPropertyProvider,
        TimeProvider timeProvider,
        string name,
        string? description = null)
        : base(toolLogHandlerProvider, pluginStore, toolPropertyProvider, name, description)
    {
        TimeProvider = timeProvider;
        ProfileFileOption = new Option<string>(new[] { "-i", "--input" }, "Profile file") { ArgumentHelpName = "file" };
        AddOption(ProfileFileOption);
        ListResourceOption = new Option<bool>(new[] { "-l", "--list-resource" }, "List associated resources");
        AddOption(ListResourceOption);
        ToolOption = new Option<string>(new[] { "-t", "--tool" }, "Tool to use or filter profiles by") { ArgumentHelpName = "name" };
        AddOption(ToolOption);
        GroupOption = new Option<string>(new[] { "-g", "--group" }, "Group to use or filter profiles by") { ArgumentHelpName = "name" };
        AddOption(GroupOption);
        DetailedOption = new Option<bool>(new[] { "--detailed" }, "Show detailed information on entries");
        AddOption(DetailedOption);
        AddValidator(v =>
        {
            if (v.GetValueForOption(ProfileFileOption) == null && v.GetValueForOption(ToolOption) == null)
            {
                v.ErrorMessage = $"At least one of {ProfileFileOption.Aliases.First()} or {ToolOption.Aliases.First()} must be passed.";
            }
        });
    }

    protected override async Task<int> RunAsync(InvocationContext context)
    {
        string? profileFile = context.ParseResult.HasOption(ProfileFileOption) ? context.ParseResult.GetValueForOption(ProfileFileOption) : null;
        string? tool = context.ParseResult.HasOption(ToolOption) ? context.ParseResult.GetValueForOption(ToolOption) : null;
        string? group = context.ParseResult.HasOption(GroupOption) ? context.ParseResult.GetValueForOption(GroupOption) : null;
        if (profileFile == null) return await ExecAsync(context, new ArtifactToolProfile(tool!, group, null));
        int ec = 0;
        foreach (ArtifactToolProfile profile in ArtifactToolProfileUtil.DeserializeProfilesFromFile(profileFile))
        {
            if (group != null && group != profile.Group || tool != null && tool != profile.Tool) continue;
            ec = Common.AccumulateErrorCode(await ExecAsync(context, profile), ec);
        }
        return ec;
    }

    private async Task<int> ExecAsync(InvocationContext context, ArtifactToolProfile profile)
    {
        using var arm = new InMemoryArtifactRegistrationManager();
        using var adm = new NullArtifactDataManager();
        profile = PrepareProfile(context, profile);
        using var tool = await GetToolAsync(profile, arm, adm, TimeProvider);
        ArtifactToolListOptions options = new();
        ArtifactToolListProxy proxy = new(tool, options, ToolLogHandlerProvider.GetDefaultToolLogHandler());
        bool listResource = context.ParseResult.GetValueForOption(ListResourceOption);
        bool detailed = context.ParseResult.GetValueForOption(DetailedOption);
        await foreach (IArtifactData data in proxy.ListAsync())
        {
            await Common.DisplayAsync(data, listResource, detailed, ToolOutput);
        }
        return 0;
    }
}
