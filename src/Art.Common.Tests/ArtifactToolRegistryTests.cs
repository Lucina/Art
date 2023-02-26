using System;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Art.Common.Tests;

public class ArtifactToolRegistryTests
{
    private static readonly ArtifactToolID s_dummyToolId = new("Art.Common.Tests", "Art.Common.Tests.ArtifactToolRegistryTestsDummyTool");
    private static readonly ArtifactToolID s_customToolId = new("CustomAssembly", "CustomType");

    private ArtifactToolRegistry _registry = null!;

    [SetUp]
    public void SetUp()
    {
        _registry = new ArtifactToolRegistry();
    }

    [Test]
    public void Add_Generic_AllowsLoad()
    {
        _registry.AddSelectable<ArtifactToolRegistryTestsDummyTool>();
        bool success = _registry.TryLoad(s_dummyToolId, out IArtifactTool? artifactTool);
        Assert.That(success, Is.True);
        Assert.That(artifactTool, Is.InstanceOf<ArtifactToolRegistryTestsDummyTool>());
        artifactTool?.Dispose();
    }

    [Test]
    public void AddSelectable_Generic_AllowsLoad()
    {
        _registry.AddSelectable<ArtifactToolRegistryTestsDummyTool>();
        bool success = _registry.TryLoad(s_dummyToolId, out IArtifactTool? artifactTool);
        Assert.That(success, Is.True);
        Assert.That(artifactTool, Is.InstanceOf<ArtifactToolRegistryTestsDummyTool>());
        artifactTool?.Dispose();
    }

    [Test]
    public void Add_DuplicateButCustomId_AllowsLoad()
    {
        _registry.AddSelectable<ArtifactToolRegistryTestsDummyTool>();
        _registry.Add(new ArtifactToolSelectableRegistryEntry<ArtifactToolRegistryTestsDummyTool>(s_customToolId));
        bool success = _registry.TryLoad(s_dummyToolId, out IArtifactTool? artifactTool);
        Assert.That(success, Is.True);
        Assert.That(artifactTool, Is.InstanceOf<ArtifactToolRegistryTestsDummyTool>());
        artifactTool?.Dispose();
    }

    [Test]
    public void Add_DuplicateGeneric_ThrowsArgumentException()
    {
        _registry.AddSelectable<ArtifactToolRegistryTestsDummyTool>();
        Assert.That(() => _registry.AddSelectable<ArtifactToolRegistryTestsDummyTool>(), Throws.InstanceOf<ArgumentException>());
    }

    [Test]
    public void AddSelectable_DuplicateGeneric_ThrowsArgumentException()
    {
        _registry.AddSelectable<ArtifactToolRegistryTestsDummyTool>();
        Assert.That(() => _registry.AddSelectable<ArtifactToolRegistryTestsDummyTool>(), Throws.InstanceOf<ArgumentException>());
    }

    [Test]
    public void Add_WithSelectableType_DisallowsSelectionOfSelectable()
    {
        _registry.Add<ArtifactToolRegistryTestsDummyTool>();
        bool success = _registry.TryIdentify("ID_1234", out ArtifactToolID artifactToolId, out string? artifactId);
        Assert.That(success, Is.False);
        Assert.That(artifactToolId, Is.EqualTo(default(ArtifactToolID)));
        Assert.That(artifactId, Is.Null);
    }

    [Test]
    public void AddSelectable_WithSelectableType_AllowsSelectionOfSelectable()
    {
        _registry.AddSelectable<ArtifactToolRegistryTestsDummyTool>();
        bool success = _registry.TryIdentify("ID_1234", out ArtifactToolID artifactToolId, out string? artifactId);
        Assert.That(success, Is.True);
        Assert.That(artifactToolId, Is.EqualTo(s_dummyToolId));
        success = _registry.TryLoad(artifactToolId, out IArtifactTool? artifactTool);
        Assert.That(success, Is.True);
        Assert.That(artifactTool, Is.InstanceOf<ArtifactToolRegistryTestsDummyTool>());
        Assert.That(artifactId, Is.EqualTo("1234"));
        artifactTool?.Dispose();
    }

    [Test]
    public void TryLoad_ValidToolId_Succeeds()
    {
        _registry.AddSelectable<ArtifactToolRegistryTestsDummyTool>();
        bool success = _registry.TryLoad(s_dummyToolId, out IArtifactTool? artifactTool);
        Assert.That(success, Is.True);
        Assert.That(artifactTool, Is.InstanceOf<ArtifactToolRegistryTestsDummyTool>());
        artifactTool?.Dispose();
    }

    [Test]
    public void TryLoad_CustomToolId_Succeeds()
    {
        _registry.Add(new ArtifactToolSelectableRegistryEntry<ArtifactToolRegistryTestsDummyTool>(s_customToolId));
        bool success = _registry.TryLoad(s_customToolId, out IArtifactTool? artifactTool);
        Assert.That(success, Is.True);
        Assert.That(artifactTool, Is.InstanceOf<ArtifactToolRegistryTestsDummyTool>());
        artifactTool?.Dispose();
    }

    [Test]
    public void TryLoad_InvalidToolId_Fails()
    {
        _registry.AddSelectable<ArtifactToolRegistryTestsDummyTool>();
        bool success = _registry.TryLoad(new ArtifactToolID("InvalidAssembly", "InvalidType"), out IArtifactTool? artifactTool);
        Assert.That(success, Is.False);
        Assert.That(artifactTool, Is.Null);
    }

    [Test]
    public void TryIdentify_ValidKey_Succeeds()
    {
        _registry.AddSelectable<ArtifactToolRegistryTestsDummyTool>();
        bool success = _registry.TryIdentify("ID_1234", out ArtifactToolID artifactToolId, out string? artifactId);
        Assert.That(success, Is.True);
        Assert.That(artifactToolId, Is.EqualTo(s_dummyToolId));
        Assert.That(artifactId, Is.EqualTo("1234"));
    }

    [Test]
    public void TryIdentify_CustomToolId_ValidKey_Succeeds()
    {
        _registry.Add(new ArtifactToolSelectableRegistryEntry<ArtifactToolRegistryTestsDummyTool>(s_customToolId));
        bool success = _registry.TryIdentify("ID_1234", out ArtifactToolID artifactToolId, out string? artifactId);
        Assert.That(success, Is.True);
        Assert.That(artifactToolId, Is.EqualTo(s_customToolId));
        Assert.That(artifactId, Is.EqualTo("1234"));
    }

    [Test]
    public void TryIdentify_InvalidKey_Fails()
    {
        _registry.AddSelectable<ArtifactToolRegistryTestsDummyTool>();
        bool success = _registry.TryIdentify("INVALID_1234", out ArtifactToolID artifactToolId, out string? artifactId);
        Assert.That(success, Is.False);
        Assert.That(artifactToolId, Is.EqualTo(default(ArtifactToolID)));
        Assert.That(artifactId, Is.Null);
    }

    [Test]
    public void TryIdentifyAndLoad_ValidKey_Succeeds()
    {
        _registry.AddSelectable<ArtifactToolRegistryTestsDummyTool>();
        bool success = _registry.TryIdentify("ID_1234",out ArtifactToolID artifactToolId, out string? artifactId);
        Assert.That(success, Is.True);
        Assert.That(artifactToolId, Is.EqualTo(s_dummyToolId));
        success = _registry.TryLoad(artifactToolId, out IArtifactTool? artifactTool);
        Assert.That(success, Is.True);
        Assert.That(artifactTool, Is.InstanceOf<ArtifactToolRegistryTestsDummyTool>());
        Assert.That(artifactId, Is.EqualTo("1234"));
        artifactTool?.Dispose();
    }

    [Test]
    public void TryIdentifyAndLoad_CustomToolId_ValidKey_Succeeds()
    {
        _registry.Add(new ArtifactToolSelectableRegistryEntry<ArtifactToolRegistryTestsDummyTool>(s_customToolId));
        bool success = _registry.TryIdentify("ID_1234", out ArtifactToolID artifactToolId, out string? artifactId);
        Assert.That(success, Is.True);
        Assert.That(artifactToolId, Is.EqualTo(s_customToolId));
        success = _registry.TryLoad(artifactToolId, out IArtifactTool? artifactTool);
        Assert.That(success, Is.True);
        Assert.That(artifactTool, Is.InstanceOf<ArtifactToolRegistryTestsDummyTool>());
        Assert.That(artifactId, Is.EqualTo("1234"));
        artifactTool?.Dispose();
    }

    [Test]
    public void TryIdentifyAndLoad_InvalidKey_Fails()
    {
        _registry.AddSelectable<ArtifactToolRegistryTestsDummyTool>();
        bool success = _registry.TryIdentify("INVALID_1234", out ArtifactToolID artifactToolId, out string? artifactId);
        Assert.That(success, Is.False);
        Assert.That(artifactToolId, Is.EqualTo(default(ArtifactToolID)));
        Assert.That(artifactId, Is.Null);
    }
}

internal partial class ArtifactToolRegistryTestsDummyTool : ArtifactTool, IArtifactToolSelfFactory<ArtifactToolRegistryTestsDummyTool>, IArtifactToolRegexSelector<ArtifactToolRegistryTestsDummyTool>
{
    [GeneratedRegex(@"^ID_(?<ID_GROUP>\d+)$")]
    public static partial Regex GetArtifactToolSelectorRegex();

    public static string GetArtifactToolSelectorRegexIdGroupName() => "ID_GROUP";
}
