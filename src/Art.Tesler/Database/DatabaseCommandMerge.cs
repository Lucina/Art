using System.CommandLine;
using System.CommandLine.Invocation;
using Art.Common;

namespace Art.Tesler.Database;

public class DatabaseCommandMerge : DatabaseCommandBase
{
    public enum MergeFilter
    {
        /// <summary>
        /// Only merge artifacts that are new or are full when previous was non-full.
        /// </summary>
        New,

        /// <summary>
        /// Only merge artifacts considered updated.
        /// </summary>
        Updated,

        /// <summary>
        /// Allow merging all selected artifacts.
        /// </summary>
        ForceAll
    }
    protected ITeslerRegistrationProvider InputRegistrationProvider;

    protected Option<bool> ListOption;

    protected Option<bool> AllOption;

    protected Option<bool> DoMergeOption;

    protected Option<MergeFilter> MergeFilterOption;

    public DatabaseCommandMerge(
        IOutputControl toolOutput,
        ITeslerRegistrationProvider registrationProvider,
        ITeslerRegistrationProvider inputRegistrationProvider,
        string name,
        string? description = null)
        : base(toolOutput, registrationProvider, name, description)
    {
        InputRegistrationProvider = inputRegistrationProvider;
        InputRegistrationProvider.Initialize(this);
        ListOption = new Option<bool>(new[] { "--list" }, "List items");
        AddOption(ListOption);
        AllOption = new Option<bool>(new[] { "--all" }, "Delete all items");
        AddOption(AllOption);
        DoMergeOption = new Option<bool>(new[] { "--do-merge" }, "Perform actual merge");
        AddOption(DoMergeOption);
        MergeFilterOption = new Option<MergeFilter>(new[] { "--merge-filter" }, "Filter artifacts to commit");
        MergeFilterOption.SetDefaultValue(MergeFilter.Updated);
        AddOption(MergeFilterOption);
        AddValidator(result =>
        {
            bool anyFilters = false;
            anyFilters |= result.GetValueForOption(ToolOption) != null;
            anyFilters |= result.GetValueForOption(GroupOption) != null;
            anyFilters |= result.GetValueForOption(ToolLikeOption) != null;
            anyFilters |= result.GetValueForOption(GroupLikeOption) != null;
            anyFilters |= result.GetValueForOption(IdOption) != null;
            anyFilters |= result.GetValueForOption(IdLikeOption) != null;
            anyFilters |= result.GetValueForOption(NameLikeOption) != null;
            if (result.GetValueForOption(AllOption))
            {
                if (anyFilters)
                {
                    result.ErrorMessage = "Cannot specify --all when filters have been specified.";
                }
            }
            else if (!anyFilters)
            {
                result.ErrorMessage = "At least one filter or --all must be specified.";
            }
        });
    }

    protected override async Task<int> RunAsync(InvocationContext context, CancellationToken cancellationToken)
    {
        using var arm = RegistrationProvider.CreateArtifactRegistrationManager(context);
        using var inputArm = InputRegistrationProvider.CreateArtifactRegistrationManager(context);
        IEnumerable<ArtifactInfo> en;
        if (context.ParseResult.GetValueForOption(AllOption))
        {
            en = await inputArm.ListArtifactsAsync(cancellationToken).ConfigureAwait(false);
        }
        else
        {
            string? tool = context.ParseResult.GetValueForOption(ToolOption);
            string? group = context.ParseResult.GetValueForOption(GroupOption);
            string? toolLike = context.ParseResult.GetValueForOption(ToolLikeOption);
            string? groupLike = context.ParseResult.GetValueForOption(GroupLikeOption);
            string? id = context.ParseResult.GetValueForOption(IdOption);
            string? idLike = context.ParseResult.GetValueForOption(IdLikeOption);
            string? nameLike = context.ParseResult.GetValueForOption(NameLikeOption);
            en = (await inputArm.ListArtifactsOptionalsAsync(tool, group, cancellationToken: cancellationToken).ConfigureAwait(false)).WithFilters(tool, toolLike, group, groupLike, id, idLike, nameLike);
        }
        MergeFilter mergeFilter = context.ParseResult.GetValueForOption(MergeFilterOption);
        int v = 0;
        bool list = context.ParseResult.GetValueForOption(ListOption);
        bool doMerge = context.ParseResult.GetValueForOption(DoMergeOption);
        bool listResource = context.ParseResult.GetValueForOption(ListResourceOption);
        bool detailed = context.ParseResult.GetValueForOption(DetailedOption);
        foreach (ArtifactInfo i in en.ToList())
        {
            ArtifactInfo? existing = await arm.TryGetArtifactAsync(i.Key, cancellationToken).ConfigureAwait(false);
            switch (mergeFilter)
            {
                case MergeFilter.Updated:
                    if ((ItemStateFlagsUtility.GetItemStateFlags(existing, i) & ItemStateFlags.NewerIdentityMask) == 0)
                    {
                        continue;
                    }
                    break;
                case MergeFilter.New:
                    if ((ItemStateFlagsUtility.GetItemStateFlags(existing, i) & ItemStateFlags.New) == 0)
                    {
                        continue;
                    }
                    break;
                case MergeFilter.ForceAll:
                    break;
                default:
                    throw new ArgumentException($"Invalid merge filter value {mergeFilter}");
            }
            if (list)
            {
                await Common.DisplayAsync(i, listResource, inputArm, detailed, ToolOutput).ConfigureAwait(false);
                if (existing != null)
                {
                    await Common.DisplayAsync(existing, listResource, arm, detailed, ToolOutput).ConfigureAwait(false);
                }
            }
            if (doMerge)
            {
                if (existing != null)
                {
                    await arm.RemoveArtifactAsync(i.Key, cancellationToken).ConfigureAwait(false);
                }
                await arm.AddArtifactAsync(i, cancellationToken).ConfigureAwait(false);
                foreach (var resource in await inputArm.ListResourcesAsync(i.Key, cancellationToken).ConfigureAwait(false))
                {
                    await arm.AddResourceAsync(resource, cancellationToken).ConfigureAwait(false);
                }
            }
            v++;
        }
        ToolOutput.Out.WriteLine(doMerge ? $"Merged {v} records." : $"{v} records would be affected.");
        return 0;
    }
}
