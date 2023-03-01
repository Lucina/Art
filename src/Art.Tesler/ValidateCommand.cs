using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using Art.Common;

namespace Art.Tesler;

public class ValidateCommand : ToolCommandBase
{
    protected ITeslerDataProvider DataProvider;

    protected ITeslerRegistrationProvider RegistrationProvider;

    protected Option<string> HashOption;

    protected Argument<List<string>> ProfileFilesArg;

    protected Option<bool> RepairOption;

    protected Option<bool> AddChecksumOption;

    protected Option<bool> DetailedOption;

    public ValidateCommand(
        IToolLogHandlerProvider toolLogHandlerProvider,
        IArtifactToolRegistryStore pluginStore,
        IDefaultPropertyProvider defaultPropertyProvider,
        ITeslerDataProvider dataProvider,
        ITeslerRegistrationProvider registrationProvider)
        : this(toolLogHandlerProvider, pluginStore, defaultPropertyProvider, dataProvider, registrationProvider, "validate", "Verify resource integrity.")
    {
    }

    public ValidateCommand(
        IToolLogHandlerProvider toolLogHandlerProvider,
        IArtifactToolRegistryStore pluginStore,
        IDefaultPropertyProvider defaultPropertyProvider,
        ITeslerDataProvider dataProvider,
        ITeslerRegistrationProvider registrationProvider,
        string name,
        string? description = null) : base(toolLogHandlerProvider, pluginStore, defaultPropertyProvider, name, description)
    {
        DataProvider = dataProvider;
        DataProvider.Initialize(this);
        RegistrationProvider = registrationProvider;
        RegistrationProvider.Initialize(this);
        HashOption = new Option<string>(new[] { "-h", "--hash" }, $"Checksum algorithm ({Common.ChecksumAlgorithms})");
        HashOption.SetDefaultValue(Common.DefaultChecksumAlgorithm);
        AddOption(HashOption);
        ProfileFilesArg = new Argument<List<string>>("profile", "Profile file(s) to filter and repair with") { HelpName = "profile", Arity = ArgumentArity.ZeroOrMore };
        AddArgument(ProfileFilesArg);
        RepairOption = new Option<bool>(new[] { "--repair" }, "Re-obtain resources that failed validation (requires appropriate profiles)");
        AddOption(RepairOption);
        AddChecksumOption = new Option<bool>(new[] { "--add-checksum" }, "Add checksum to resources without checksum during validation");
        AddOption(AddChecksumOption);
        DetailedOption = new Option<bool>(new[] { "--detailed" }, "Show detailed information on entries");
        AddOption(DetailedOption);
    }

    protected override async Task<int> RunAsync(InvocationContext context)
    {
        ChecksumSource? checksumSource;
        string? hash = context.ParseResult.HasOption(HashOption) ? context.ParseResult.GetValueForOption(HashOption) : null;
        hash = string.Equals(hash, "none", StringComparison.InvariantCultureIgnoreCase) ? null : hash;
        if (hash == null)
        {
            checksumSource = null;
        }
        else
        {
            if (!ChecksumSource.DefaultSources.TryGetValue(hash, out checksumSource))
            {
                PrintErrorMessage(Common.GetInvalidHashMessage(hash), ToolOutput);
                return 2;
            }
        }
        IToolLogHandler l = ToolLogHandlerProvider.GetDefaultToolLogHandler();
        List<ArtifactToolProfile> profiles = new();
        foreach (string profileFile in context.ParseResult.GetValueForArgument(ProfileFilesArg))
            profiles.AddRange(ArtifactToolProfileUtil.DeserializeProfilesFromFile(profileFile));
        string? cookieFile = context.ParseResult.HasOption(CookieFileOption) ? context.ParseResult.GetValueForOption(CookieFileOption) : null;
        string? userAgent = context.ParseResult.HasOption(UserAgentOption) ? context.ParseResult.GetValueForOption(UserAgentOption) : null;
        IEnumerable<string> properties = context.ParseResult.HasOption(PropertiesOption) ? context.ParseResult.GetValueForOption(PropertiesOption)! : Array.Empty<string>();
        profiles = profiles.Select(p => p.GetWithConsoleOptions(DefaultPropertyProvider, properties, cookieFile, userAgent, ToolOutput)).ToList();
        bool repair = context.ParseResult.GetValueForOption(RepairOption);
        if (profiles.Count == 0)
        {
            if (repair)
            {
                l.Log("Repair was requested, but no profiles were provided", null, LogLevel.Error);
                return 3;
            }
            l.Log("No profiles provided, validating all artifacts and resources", null, LogLevel.Information);
        }
        using var adm = DataProvider.CreateArtifactDataManager(context);
        using var arm = RegistrationProvider.CreateArtifactRegistrationManager(context);
        var validationContext = new ValidationContext(PluginStore, arm, adm, l);
        ValidationProcessResult result;
        ChecksumSource? checksumSourceForAdd = context.ParseResult.GetValueForOption(AddChecksumOption) ? checksumSource : null;
        if (profiles.Count == 0)
        {
            result = await validationContext.ProcessAsync(await arm.ListArtifactsAsync(), checksumSourceForAdd);
        }
        else
        {
            result = await validationContext.ProcessAsync(profiles, checksumSourceForAdd);
        }
        l.Log($"Total: {result.Artifacts} artifacts and {result.Resources} processed.", null, LogLevel.Information);
        if (!validationContext.AnyFailed)
        {
            l.Log("All resources for specified profiles successfully validated.", null, LogLevel.Information);
            return 0;
        }
        int resourceFailCount = validationContext.CountResourceFailures();
        if (!repair)
        {
            l.Log($"{resourceFailCount} resources failed to validate.", null, LogLevel.Warning);
            foreach (var entry in validationContext.GetFailureCountsByKey())
            {
                l.Log($"Artifact {entry.Key}: {entry.Value} failures", null, LogLevel.Warning);
            }
            return 1;
        }
        l.Log($"{resourceFailCount} resources failed to validate and will be reacquired.", null, LogLevel.Information);
        var repairContext = validationContext.CreateRepairContext();
        await repairContext.RepairAsync(profiles, context.ParseResult.GetValueForOption(DetailedOption), checksumSource, ToolOutput);
        return 0;
    }
}
