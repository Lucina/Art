using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Art.Common.Tests;

public class ArtifactToolRegexSelectorTests
{
    private static readonly ArtifactToolID s_dummyToolId = new("Art.Common.Tests", "Art.Common.Tests.ArtifactToolRegexSelectorTestsDummyTool");

    [Test]
    public void TryIdentify_ValidKey_Succeeds()
    {
        bool success = TryIdentify<ArtifactToolRegexSelectorTestsDummyTool>("ID_1234", out ArtifactToolID? artifactToolId, out string? artifactId);
        Assert.That(success, Is.True);
        Assert.That(artifactToolId, Is.EqualTo(s_dummyToolId));
        Assert.That(artifactId, Is.EqualTo("1234"));
    }

    [Test]
    public void TryIdentify_InvalidKey_Fails()
    {
        bool success = TryIdentify<ArtifactToolRegexSelectorTestsDummyTool>("INVALID_1234", out ArtifactToolID? artifactToolId, out string? artifactId);
        Assert.That(success, Is.False);
        Assert.That(artifactToolId, Is.EqualTo(default(ArtifactToolID)));
        Assert.That(artifactId, Is.Null);
    }

    private static bool TryIdentify<T>(string key, [NotNullWhen(true)] out ArtifactToolID? artifactToolId, [NotNullWhen(true)] out string? artifactId) where T : IArtifactToolSelector<string>
    {
        return T.TryIdentify(key, out artifactToolId, out artifactId);
    }
}

internal partial class ArtifactToolRegexSelectorTestsDummyTool : ArtifactTool, IArtifactToolSelfFactory<ArtifactToolRegexSelectorTestsDummyTool>, IArtifactToolRegexSelector<ArtifactToolRegexSelectorTestsDummyTool>
{
    [GeneratedRegex(@"^ID_(?<ID_GROUP>\d+)$")]
    public static partial Regex GetArtifactToolSelectorRegex();

    public static string GetArtifactToolSelectorRegexIdGroupName() => "ID_GROUP";
}
