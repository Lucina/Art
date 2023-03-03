using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Art.Common;

/// <summary>
/// Registry entry that wraps another entry with explicit regex to apply during selection.
/// </summary>
/// <param name="Entry">Entry to augment.</param>
/// <param name="Regex">Regex to use for selection.</param>
/// <param name="Group">Named group to use for regex match, or null to match first group.</param>
public record ArtifactToolRegexSelectableRegistryEntry(ArtifactToolRegistryEntry Entry, Regex Regex, string? Group = null) : ArtifactToolSelectableRegistryEntry(Entry.Id)
{
    /// <inheritdoc />
    public override bool TryIdentify(string key, [NotNullWhen(true)] out ArtifactToolID? artifactToolId, [NotNullWhen(true)] out string? artifactId)
    {
        if (Regex.Match(key) is not { Success: true } match)
        {
            artifactToolId = null;
            artifactId = null;
            return false;
        }
        if (Group == null)
        {
            if (match.Groups.Count == 0)
            {
                throw new InvalidOperationException("The provided regular expression has 0 groups, but one is expected because no named group was supplied.");
            }
            if (match.Groups[0] is not { Success: true } firstGroup)
            {
                artifactToolId = null;
                artifactId = null;
                return false;
            }
            artifactToolId = Entry.Id;
            artifactId = firstGroup.Value;
            return true;
        }
        if (match.Groups[Group] is not { Success: true } namedGroup)
        {
            artifactToolId = null;
            artifactId = null;
            return false;
        }
        artifactToolId = Entry.Id;
        artifactId = namedGroup.Value;
        return true;
    }

    /// <inheritdoc />
    public override IArtifactTool CreateArtifactTool() => Entry.CreateArtifactTool();

    /// <inheritdoc />
    public override Type GetArtifactToolType() => Entry.GetArtifactToolType();
}
