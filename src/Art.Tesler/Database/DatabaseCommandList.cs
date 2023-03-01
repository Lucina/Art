﻿using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Text.Json;
using Art.Common;

namespace Art.Tesler.Database;

public class DatabaseCommandList : DatabaseCommandBase
{
    protected Option<string> AugmentOption;

    protected Option<string> OutputOption;

    public DatabaseCommandList(ITeslerRegistrationProvider registrationProvider, string name, string? description = null) : base(registrationProvider, name, description)
    {
        AugmentOption = new Option<string>(new[] { "--augment" }, "Use base profile file for profile generated by --output") { ArgumentHelpName = "file" };
        AddOption(AugmentOption);
        OutputOption = new Option<string>(new[] { "-o", "--output" }, "Generate profile file") { ArgumentHelpName = "file" };
        AddOption(OutputOption);
    }

    protected override async Task<int> RunAsync(InvocationContext context)
    {
        string? augment = context.ParseResult.HasOption(AugmentOption) ? context.ParseResult.GetValueForOption(AugmentOption) : null;
        string? output = context.ParseResult.HasOption(OutputOption) ? context.ParseResult.GetValueForOption(OutputOption) : null;
        List<ArtifactKey>? selection = output != null ? new List<ArtifactKey>() : null;
        using var arm = RegistrationProvider.CreateArtifactRegistrationManager(context);
        string? tool = context.ParseResult.HasOption(ToolOption) ? context.ParseResult.GetValueForOption(ToolOption) : null;
        string? group = context.ParseResult.HasOption(GroupOption) ? context.ParseResult.GetValueForOption(GroupOption) : null;
        string? toolLike = context.ParseResult.HasOption(ToolLikeOption) ? context.ParseResult.GetValueForOption(ToolLikeOption) : null;
        string? groupLike = context.ParseResult.HasOption(GroupLikeOption) ? context.ParseResult.GetValueForOption(GroupLikeOption) : null;
        string? id = context.ParseResult.HasOption(IdOption) ? context.ParseResult.GetValueForOption(IdOption) : null;
        string? idLike = context.ParseResult.HasOption(IdLikeOption) ? context.ParseResult.GetValueForOption(IdLikeOption) : null;
        string? nameLike = context.ParseResult.HasOption(NameLikeOption) ? context.ParseResult.GetValueForOption(NameLikeOption) : null;
        IEnumerable<ArtifactInfo> en = (await arm.ListArtifactsOptionalsAsync(tool, group)).WithFilters(tool, toolLike, group, groupLike, id, idLike, nameLike);
        bool listResource = context.ParseResult.GetValueForOption(ListResourceOption);
        bool detailed = context.ParseResult.GetValueForOption(DetailedOption);
        foreach (ArtifactInfo i in en)
        {
            await Common.DisplayAsync(i, listResource, arm, detailed, context.Console);
            selection?.Add(i.Key);
        }
        if (output != null && selection != null)
        {
            Dictionary<ToolAndGroup, List<string>> dict = selection.GroupBy(x => new ToolAndGroup(x.Tool, x.Group)).ToDictionary(x => x.Key, x => x.Select(v => v.Id).ToList());
            List<ArtifactToolProfile> profiles = new();
            if (augment != null)
            {
                await using FileStream afs = File.OpenRead(augment);
                var augmentProfiles = ArtifactToolProfileUtil.DeserializeProfiles(afs);
                Dictionary<ToolAndGroup, ArtifactToolProfile> adict = augmentProfiles.ToDictionary(x => new ToolAndGroup(x.Tool, x.Group), x => x);
                foreach ((ToolAndGroup profile, List<string> value) in dict)
                    adict[profile] = adict.TryGetValue(profile, out ArtifactToolProfile? aprofile)
                        ? AugmentProfile(aprofile, value)
                        : CreateNewProfile(profile, value);
                profiles.AddRange(adict.Values);
            }
            else
            {
                foreach ((ToolAndGroup profile, List<string> value) in dict) profiles.Add(CreateNewProfile(profile, value));
            }
            await using FileStream fs = File.Create(output);
            await JsonSerializer.SerializeAsync(fs, profiles, SourceGenerationContext.s_context.ListArtifactToolProfile);
        }
        return 0;
    }

    private readonly record struct ToolAndGroup(string Tool, string? Group);

    private static ArtifactToolProfile AugmentProfile(ArtifactToolProfile profile, List<string> ids)
        => CreateNewProfile(new ToolAndGroup(profile.Tool, profile.Group), profile.Options != null ? new Dictionary<string, JsonElement>(profile.Options) : new Dictionary<string, JsonElement>(), ids);

    private static ArtifactToolProfile CreateNewProfile(ToolAndGroup profile, List<string> ids)
        => CreateNewProfile(profile, new Dictionary<string, JsonElement>(), ids);

    private static ArtifactToolProfile CreateNewProfile(ToolAndGroup profile, Dictionary<string, JsonElement> dict, List<string> ids)
    {
        dict["artifactList"] = JsonSerializer.SerializeToElement(ids, SourceGenerationContext.s_context.ListString);
        return new ArtifactToolProfile(profile.Tool, profile.Group, dict);
    }
}
