using System;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Art.Common.Tests;

public class AggregateArtifactToolRegistryTests
{
    private static readonly ArtifactToolID s_dummyToolId = new("Art.Common.Tests", "Art.Common.Tests.AggregateArtifactToolRegistryTestsDummyTool");

    private AggregateArtifactToolRegistry _registry = null!;
    private ArtifactToolRegistry _subRegistry0 = null!;
    private ArtifactToolRegistry _subRegistry1 = null!;

    [SetUp]
    public void SetUp()
    {
        _registry = new AggregateArtifactToolRegistry();
    }

    [Test]
    public void Add_Single_Succeeds()
    {
        _subRegistry0 = new ArtifactToolRegistry();
        _registry.Add(_subRegistry0);
        Assert.Pass();
    }

    [Test]
    public void Add_Multiple_Succeeds()
    {
        _subRegistry0 = new ArtifactToolRegistry();
        _registry.Add(_subRegistry0);
        _subRegistry1 = new ArtifactToolRegistry();
        _registry.Add(_subRegistry1);
        Assert.Pass();
    }

    [Test]
    public void Add_Duplicate_ThrowsArgumentException()
    {
        _subRegistry0 = new ArtifactToolRegistry();
        _registry.Add(_subRegistry0);
        Assert.That(() => _registry.Add(_subRegistry0), Throws.InstanceOf<ArgumentException>());
    }

    [Test]
    public void TryLoad_MultipleRegistries_LastHasApplicable_Success()
    {
        _subRegistry0 = new ArtifactToolRegistry();
        _subRegistry1 = new ArtifactToolRegistry();
        _subRegistry1.AddSelectable<AggregateArtifactToolRegistryTestsDummyTool>();
        _registry.Add(_subRegistry0);
        _registry.Add(_subRegistry1);
        bool success = _registry.TryLoad(s_dummyToolId, out IArtifactTool? tool);
        Assert.That(success, Is.True);
        Assert.That(tool, Is.InstanceOf<AggregateArtifactToolRegistryTestsDummyTool>());
        tool?.Dispose();
    }

    [Test]
    public void TryLoad_MultipleRegistries_OtherThanLastHasApplicable_Success()
    {
        _subRegistry0 = new ArtifactToolRegistry();
        _subRegistry0.AddSelectable<AggregateArtifactToolRegistryTestsDummyTool>();
        _subRegistry1 = new ArtifactToolRegistry();
        _registry.Add(_subRegistry0);
        _registry.Add(_subRegistry1);
        bool success = _registry.TryLoad(s_dummyToolId, out IArtifactTool? tool);
        Assert.That(success, Is.True);
        Assert.That(tool, Is.InstanceOf<AggregateArtifactToolRegistryTestsDummyTool>());
        tool?.Dispose();
    }

    [Test]
    public void TryLoad_DuplicatesInRegistries_LastUsedFirst()
    {
        ArtifactToolID sharedId = new("SharedAssembly", "SharedType");
        ArtifactToolID sharedId2 = new("SharedAssembly", "SharedType2");
        _subRegistry0 = new ArtifactToolRegistry();
        _subRegistry0.Add(new ArtifactToolSelectableRegistryEntry<AggregateArtifactToolRegistryTestsDummyTool>(sharedId));
        _subRegistry0.Add(new ArtifactToolSelectableRegistryEntry<AggregateArtifactToolRegistryTestsDummyTool2>(sharedId2));
        _subRegistry1 = new ArtifactToolRegistry();
        _subRegistry1.Add(new ArtifactToolSelectableRegistryEntry<AggregateArtifactToolRegistryTestsDummyTool2>(sharedId));
        _subRegistry1.Add(new ArtifactToolSelectableRegistryEntry<AggregateArtifactToolRegistryTestsDummyTool>(sharedId2));
        _registry.Add(_subRegistry0);
        _registry.Add(_subRegistry1);
        bool success = _registry.TryLoad(sharedId, out IArtifactTool? tool);
        Assert.That(success, Is.True);
        Assert.That(tool, Is.InstanceOf<AggregateArtifactToolRegistryTestsDummyTool2>());
        tool?.Dispose();
        bool success2 = _registry.TryLoad(sharedId2, out IArtifactTool? tool2);
        Assert.That(success2, Is.True);
        Assert.That(tool2, Is.InstanceOf<AggregateArtifactToolRegistryTestsDummyTool>());
        tool2?.Dispose();
    }

    [Test]
    public void TryLoad_ValidToolId_Succeeds()
    {
        _subRegistry0 = new ArtifactToolRegistry();
        _registry.Add(_subRegistry0);
        _subRegistry0.AddSelectable<AggregateArtifactToolRegistryTestsDummyTool>();
        bool success = _registry.TryLoad(s_dummyToolId, out IArtifactTool? artifactTool);
        Assert.That(success, Is.True);
        Assert.That(artifactTool, Is.InstanceOf<AggregateArtifactToolRegistryTestsDummyTool>());
        artifactTool?.Dispose();
    }

    [Test]
    public void TryLoad_InvalidToolId_Fails()
    {
        _subRegistry0 = new ArtifactToolRegistry();
        _registry.Add(_subRegistry0);
        _subRegistry0.AddSelectable<AggregateArtifactToolRegistryTestsDummyTool>();
        bool success = _registry.TryLoad(new ArtifactToolID("InvalidAssembly", "InvalidType"), out IArtifactTool? artifactTool);
        Assert.That(success, Is.False);
        Assert.That(artifactTool, Is.Null);
    }

    [Test]
    public void TryIdentify_ValidKey_Succeeds()
    {
        _subRegistry0 = new ArtifactToolRegistry();
        _subRegistry0.AddSelectable<AggregateArtifactToolRegistryTestsDummyTool>();
        _registry.Add(_subRegistry0);
        bool success = _registry.TryIdentify("ID_1234", out ArtifactToolID? artifactToolId, out string? artifactId);
        Assert.That(success, Is.True);
        Assert.That(artifactToolId, Is.EqualTo(s_dummyToolId));
        Assert.That(artifactId, Is.EqualTo("1234"));
    }

    [Test]
    public void TryIdentify_InvalidKey_Fails()
    {
        _subRegistry0 = new ArtifactToolRegistry();
        _registry.Add(_subRegistry0);
        _subRegistry0.AddSelectable<AggregateArtifactToolRegistryTestsDummyTool>();
        bool success = _registry.TryIdentify("INVALID_1234", out ArtifactToolID? artifactToolId, out string? artifactId);
        Assert.That(success, Is.False);
        Assert.That(artifactToolId, Is.EqualTo(default(ArtifactToolID)));
        Assert.That(artifactId, Is.Null);
    }

    [Test]
    public void TryIdentifyAndLoad_ValidKey_Succeeds()
    {
        _subRegistry0 = new ArtifactToolRegistry();
        _registry.Add(_subRegistry0);
        _subRegistry0.AddSelectable<AggregateArtifactToolRegistryTestsDummyTool>();
        bool success = _registry.TryIdentify("ID_1234", out ArtifactToolID? artifactToolId, out string? artifactId);
        Assert.That(success, Is.True);
        Assert.That(artifactToolId, Is.EqualTo(s_dummyToolId));
        success = _registry.TryLoad(artifactToolId!, out IArtifactTool? artifactTool);
        Assert.That(success, Is.True);
        Assert.That(artifactTool, Is.InstanceOf<AggregateArtifactToolRegistryTestsDummyTool>());
        Assert.That(artifactId, Is.EqualTo("1234"));
        artifactTool?.Dispose();
    }

    [Test]
    public void TryIdentifyAndLoad_InvalidKey_Fails()
    {
        _subRegistry0 = new ArtifactToolRegistry();
        _registry.Add(_subRegistry0);
        _subRegistry0.AddSelectable<AggregateArtifactToolRegistryTestsDummyTool>();
        bool success = _registry.TryIdentify("INVALID_1234", out ArtifactToolID? artifactToolId, out string? artifactId);
        Assert.That(success, Is.False);
        Assert.That(artifactToolId, Is.EqualTo(default(ArtifactToolID)));
        Assert.That(artifactId, Is.Null);
    }
}

internal partial class AggregateArtifactToolRegistryTestsDummyTool : ArtifactTool, IArtifactToolSelfFactory<AggregateArtifactToolRegistryTestsDummyTool>, IArtifactToolRegexSelector<AggregateArtifactToolRegistryTestsDummyTool>
{
    [GeneratedRegex(@"^ID_(?<ID_GROUP>\d+)$")]
    public static partial Regex GetArtifactToolSelectorRegex();

    public static string GetArtifactToolSelectorRegexIdGroupName() => "ID_GROUP";
}

internal partial class AggregateArtifactToolRegistryTestsDummyTool2 : ArtifactTool, IArtifactToolSelfFactory<AggregateArtifactToolRegistryTestsDummyTool2>, IArtifactToolRegexSelector<AggregateArtifactToolRegistryTestsDummyTool2>
{
    [GeneratedRegex(@"^ID2_(?<ID_GROUP>\d+)$")]
    public static partial Regex GetArtifactToolSelectorRegex();

    public static string GetArtifactToolSelectorRegexIdGroupName() => "ID_GROUP";
}

internal partial class AggregateArtifactToolRegistryTestsDummyTool3 : ArtifactTool, IArtifactToolSelfFactory<AggregateArtifactToolRegistryTestsDummyTool3>, IArtifactToolRegexSelector<AggregateArtifactToolRegistryTestsDummyTool3>
{
    [GeneratedRegex(@"^ID3_(?<ID_GROUP>\d+)$")]
    public static partial Regex GetArtifactToolSelectorRegex();

    public static string GetArtifactToolSelectorRegexIdGroupName() => "ID_GROUP";
}
