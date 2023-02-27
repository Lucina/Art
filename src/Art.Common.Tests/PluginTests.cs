using System.Text.RegularExpressions;
using Art.Modular;
using NUnit.Framework;

namespace Art.Common.Tests;

public class PluginTests
{
    private static readonly ArtifactToolID s_dummyToolId = new("Art.Common.Tests", "Art.Common.Tests.PluginTestTool");

    private Plugin _registry = null!;

    [SetUp]
    public void SetUp()
    {
        _registry = new Plugin(typeof(PluginTestTool).Assembly);
    }

    [Test]
    public void Generic_AllowsLoad()
    {
        bool success = _registry.TryLoad(s_dummyToolId, out IArtifactTool? artifactTool);
        Assert.That(success, Is.True);
        Assert.That(artifactTool, Is.InstanceOf<PluginTestTool>());
        artifactTool?.Dispose();
    }

    [Test]
    public void Selectable_Generic_AllowsLoad()
    {
        bool success = _registry.TryLoad(s_dummyToolId, out IArtifactTool? artifactTool);
        Assert.That(success, Is.True);
        Assert.That(artifactTool, Is.InstanceOf<PluginTestTool>());
        artifactTool?.Dispose();
    }

    [Test]
    public void Selectable_WithSelectableType_AllowsSelectionOfSelectable()
    {
        bool success = _registry.TryIdentify("PLUGINID_1234", out ArtifactToolID? artifactToolId, out string? artifactId);
        Assert.That(success, Is.True);
        Assert.That(artifactToolId, Is.EqualTo(s_dummyToolId));
        success = _registry.TryLoad(artifactToolId!, out IArtifactTool? artifactTool);
        Assert.That(success, Is.True);
        Assert.That(artifactTool, Is.InstanceOf<PluginTestTool>());
        Assert.That(artifactId, Is.EqualTo("1234"));
        artifactTool?.Dispose();
    }

    [Test]
    public void TryLoad_ValidToolId_Succeeds()
    {
        bool success = _registry.TryLoad(s_dummyToolId, out IArtifactTool? artifactTool);
        Assert.That(success, Is.True);
        Assert.That(artifactTool, Is.InstanceOf<PluginTestTool>());
        artifactTool?.Dispose();
    }

    [Test]
    public void TryLoad_InvalidToolId_Fails()
    {
        bool success = _registry.TryLoad(new ArtifactToolID("InvalidAssembly", "InvalidType"), out IArtifactTool? artifactTool);
        Assert.That(success, Is.False);
        Assert.That(artifactTool, Is.Null);
    }

    [Test]
    public void TryIdentify_ValidKey_Succeeds()
    {
        bool success = _registry.TryIdentify("PLUGINID_1234", out ArtifactToolID? artifactToolId, out string? artifactId);
        Assert.That(success, Is.True);
        Assert.That(artifactToolId, Is.EqualTo(s_dummyToolId));
        Assert.That(artifactId, Is.EqualTo("1234"));
    }

    [Test]
    public void TryIdentify_InvalidKey_Fails()
    {
        bool success = _registry.TryIdentify("INVALID_1234", out ArtifactToolID? artifactToolId, out string? artifactId);
        Assert.That(success, Is.False);
        Assert.That(artifactToolId, Is.EqualTo(default(ArtifactToolID)));
        Assert.That(artifactId, Is.Null);
    }

    [Test]
    public void TryIdentifyAndLoad_ValidKey_Succeeds()
    {
        bool success = _registry.TryIdentify("PLUGINID_1234", out ArtifactToolID? artifactToolId, out string? artifactId);
        Assert.That(success, Is.True);
        Assert.That(artifactToolId, Is.EqualTo(s_dummyToolId));
        success = _registry.TryLoad(artifactToolId!, out IArtifactTool? artifactTool);
        Assert.That(success, Is.True);
        Assert.That(artifactTool, Is.InstanceOf<PluginTestTool>());
        Assert.That(artifactId, Is.EqualTo("1234"));
        artifactTool?.Dispose();
    }

    [Test]
    public void TryIdentifyAndLoad_InvalidKey_Fails()
    {
        bool success = _registry.TryIdentify("INVALID_1234", out ArtifactToolID? artifactToolId, out string? artifactId);
        Assert.That(success, Is.False);
        Assert.That(artifactToolId, Is.EqualTo(default(ArtifactToolID)));
        Assert.That(artifactId, Is.Null);
    }
}

public partial class PluginTestTool : ArtifactTool, IArtifactToolSelfFactory<PluginTestTool>, IArtifactToolRegexSelector<PluginTestTool>
{
    [GeneratedRegex(@"^PLUGINID_(?<ID_GROUP>\d+)$")]
    public static partial Regex GetArtifactToolSelectorRegex();

    public static string GetArtifactToolSelectorRegexIdGroupName() => "ID_GROUP";
}
