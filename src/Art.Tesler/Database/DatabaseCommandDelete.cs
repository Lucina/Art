﻿using System.CommandLine;
using System.CommandLine.Invocation;
using Art.Common;

namespace Art.Tesler.Database;

public class DatabaseCommandDelete : DatabaseCommandBase
{
    protected Option<bool> ListOption;

    protected Option<bool> AllOption;

    protected Option<bool> DoDeleteOption;

    public DatabaseCommandDelete(
        IOutputControl toolOutput,
        ITeslerRegistrationProvider registrationProvider,
        string name,
        string? description = null)
        : base(toolOutput, registrationProvider, name, description)
    {
        ListOption = new Option<bool>(new[] { "--list" }, "List items");
        AddOption(ListOption);
        AllOption = new Option<bool>(new[] { "--all" }, "Delete all items");
        AddOption(AllOption);
        DoDeleteOption = new Option<bool>(new[] { "--do-delete" }, "Perform actual delete");
        AddOption(DoDeleteOption);
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
        IEnumerable<ArtifactInfo> en;
        if (context.ParseResult.GetValueForOption(AllOption))
        {
            en = await arm.ListArtifactsAsync(cancellationToken).ConfigureAwait(false);
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
            en = (await arm.ListArtifactsOptionalsAsync(tool, group, cancellationToken: cancellationToken).ConfigureAwait(false)).WithFilters(tool, toolLike, group, groupLike, id, idLike, nameLike);
        }
        int v = 0;
        bool list = context.ParseResult.GetValueForOption(ListOption);
        bool doDelete = context.ParseResult.GetValueForOption(DoDeleteOption);
        bool listResource = context.ParseResult.GetValueForOption(ListResourceOption);
        bool detailed = context.ParseResult.GetValueForOption(DetailedOption);
        foreach (ArtifactInfo i in en.ToList())
        {
            if (list) await Common.DisplayAsync(i, listResource, arm, detailed, ToolOutput).ConfigureAwait(false);
            if (doDelete) await arm.RemoveArtifactAsync(i.Key, cancellationToken).ConfigureAwait(false);
            v++;
        }
        ToolOutput.Out.WriteLine(doDelete ? $"Deleted {v} records." : $"{v} records would be affected.");
        return 0;
    }
}
