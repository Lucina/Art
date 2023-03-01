using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using Art.Common;
using Art.Common.Management;
using Art.Common.Proxies;

namespace Art.Tesler;

public class DumpCommand : ToolCommandBase
{
    protected ITeslerDataProvider DataProvider;

    protected ITeslerRegistrationProvider RegistrationProvider;

    protected Option<string> HashOption;

    protected Option<bool> NoDatabaseOption;

    protected Option<string> ProfileFileOption;

    protected Option<string> ToolOption;

    protected Option<string> GroupOption;

    public DumpCommand(IArtifactToolRegistryStore pluginStore,
        IDefaultPropertyProvider defaultPropertyProvider,
        ITeslerDataProvider dataProvider,
        ITeslerRegistrationProvider registrationProvider)
        : this(pluginStore, defaultPropertyProvider, dataProvider, registrationProvider, "dump", "Execute artifact dump tools.")
    {
    }

    public DumpCommand(IArtifactToolRegistryStore pluginStore,
        IDefaultPropertyProvider defaultPropertyProvider,
        ITeslerDataProvider dataProvider,
        ITeslerRegistrationProvider registrationProvider,
        string name,
        string? description = null)
        : base(pluginStore, defaultPropertyProvider, name, description)
    {
        DataProvider = dataProvider;
        DataProvider.Initialize(this);
        RegistrationProvider = registrationProvider;
        RegistrationProvider.Initialize(this);
        HashOption = new Option<string>(new[] { "-h", "--hash" }, $"Checksum algorithm ({Common.ChecksumAlgorithms})");
        HashOption.SetDefaultValue(Common.DefaultChecksumAlgorithm);
        AddOption(HashOption);
        NoDatabaseOption = new Option<bool>(new[] { "--no-database" }, "Don't use database to track resources");
        AddOption(NoDatabaseOption);
        ProfileFileOption = new Option<string>(new[] { "-i", "--input" }, "Profile file") { ArgumentHelpName = "file" };
        AddOption(ProfileFileOption);
        ToolOption = new Option<string>(new[] { "-t", "--tool" }, "Tool to use or filter profiles by") { ArgumentHelpName = "name" };
        AddOption(ToolOption);
        GroupOption = new Option<string>(new[] { "-g", "--group" }, "Group to use or filter profiles by") { ArgumentHelpName = "name" };
        AddOption(GroupOption);
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
        using var adm = DataProvider.CreateArtifactDataManager(context);
        if (context.ParseResult.GetValueForOption(NoDatabaseOption))
        {
            InMemoryArtifactRegistrationManager arm = new();
            return await RunAsync(context, adm, arm);
        }
        else
        {
            using var arm = RegistrationProvider.CreateArtifactRegistrationManager(context);
            return await RunAsync(context, adm, arm);
        }
    }

    private async Task<int> RunAsync(InvocationContext context, IArtifactDataManager adm, IArtifactRegistrationManager arm)
    {
        string? hash = context.ParseResult.HasOption(HashOption) ? context.ParseResult.GetValueForOption(HashOption) : null;
        hash = string.Equals(hash, "none", StringComparison.InvariantCultureIgnoreCase) ? null : hash;
        if (hash != null && !ChecksumSource.DefaultSources.ContainsKey(hash))
        {
            PrintErrorMessage(Common.GetInvalidHashMessage(hash));
            return 2;
        }
        string? profileFile = context.ParseResult.HasOption(ProfileFileOption) ? context.ParseResult.GetValueForOption(ProfileFileOption) : null;
        string? tool = context.ParseResult.HasOption(ToolOption) ? context.ParseResult.GetValueForOption(ToolOption) : null;
        string? group = context.ParseResult.HasOption(GroupOption) ? context.ParseResult.GetValueForOption(GroupOption) : null;
        if (profileFile == null) return await ExecAsync(context, new ArtifactToolProfile(tool!, group, null), arm, adm, hash);
        int ec = 0;
        foreach (ArtifactToolProfile profile in ArtifactToolProfileUtil.DeserializeProfilesFromFile(profileFile))
        {
            if (group != null && group != profile.Group || tool != null && tool != profile.Tool) continue;
            ec = Common.AccumulateErrorCode(await ExecAsync(context, profile, arm, adm, hash), ec);
        }
        return ec;
    }

    private async Task<int> ExecAsync(InvocationContext context, ArtifactToolProfile profile, IArtifactRegistrationManager arm, IArtifactDataManager adm, string? hash)
    {
        ArtifactToolDumpOptions options = new(ChecksumId: hash);
        using var tool = await GetToolAsync(context, profile, arm, adm);
        ArtifactToolDumpProxy dProxy = new(tool, options, Common.GetDefaultToolLogHandler(context.Console));
        await dProxy.DumpAsync();
        return 0;
    }
}
