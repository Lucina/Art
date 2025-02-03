using System.CommandLine;
using System.CommandLine.Invocation;
using System.Text;
using System.Text.RegularExpressions;

namespace Art.Tesler;

public class ToolsCommand : CommandBase
{
    protected IArtifactToolRegistryStore PluginStore;

    protected Option<string> SearchOption;

    protected Option<bool> DetailedOption;

    public ToolsCommand(
        IOutputControl toolOutput,
        IArtifactToolRegistryStore pluginStore)
        : this(toolOutput, pluginStore, "tools", "List available tools.")
    {
    }

    public ToolsCommand(
        IOutputControl toolOutput,
        IArtifactToolRegistryStore pluginStore,
        string name,
        string? description = null)
        : base(toolOutput, name, description)
    {
        PluginStore = pluginStore;
        SearchOption = new Option<string>(new[] { "-s", "--search" }, "Search pattern") { ArgumentHelpName = "pattern" };
        AddOption(SearchOption);
        DetailedOption = new Option<bool>(new[] { "--detailed" }, "Show detailed information on entries");
        AddOption(DetailedOption);
    }

    protected override Task<int> RunAsync(InvocationContext context, CancellationToken cancellationToken)
    {
        foreach (var plugin in PluginStore.LoadAllRegistries())
        {
            string? search = context.ParseResult.GetValueForOption(SearchOption);
            Regex? re = search != null ? Common.GetFilterRegex(search, false, false) : null;
            foreach (var desc in plugin.GetToolDescriptions()
                         .Where(v => re?.IsMatch(v.Id.GetToolString()) ?? true))
            {
                Common.PrintFormat(desc.Id.GetToolString(), context.ParseResult.GetValueForOption(DetailedOption), () =>
                {
                    bool canFind = desc.Type.IsAssignableTo(typeof(IArtifactFindTool));
                    bool canList = desc.Type.IsAssignableTo(typeof(IArtifactListTool));
                    bool canDump = canList || desc.Type.IsAssignableTo(typeof(IArtifactDumpTool));
                    bool canSelect = desc.Type.IsAssignableTo(typeof(IArtifactToolSelector<string>));
                    IEnumerable<string> capabilities = Enumerable.Empty<string>();
                    if (canFind) capabilities = capabilities.Append("find");
                    if (canList) capabilities = capabilities.Append("list");
                    if (canDump) capabilities = capabilities.Append("arc");
                    if (canSelect) capabilities = capabilities.Append("select");
                    capabilities = capabilities.DefaultIfEmpty("none");
                    return new StringBuilder("Capabilities: ").AppendJoin(", ", capabilities).ToString();
                }, ToolOutput);
            }
        }
        return Task.FromResult(0);
    }
}
