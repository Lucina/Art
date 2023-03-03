using NUnit.Framework;

namespace Art.Common.Tests;

public class ArtifactToolArtifactDataTests
{
    private const string ProfileGroup = "ProfileGroup";
    private const string CustomGroup = "CustomGroup";
    private const string FallbackGroup = "FallbackGroup";

    private CustomGroupArtifactTool _tool = null!;

    [SetUp]
    public void SetUp()
    {
        _tool = new CustomGroupArtifactTool();
    }

    [Test]
    public async Task CreateData_CustomGroup_NoProfileGroup_CustomGroupApplied()
    {
        await _tool.InitializeAsync(profile: CreateProfile(null));
        var data = _tool.CreateData("id", group: CustomGroup);
        Assert.That(data.Info.Key.Group, Is.EqualTo(CustomGroup).And.Not.EqualTo(ProfileGroup).And.Not.EqualTo(FallbackGroup));
    }

    [Test]
    public async Task CreateData_CustomGroup_ProfileGroup_ProfileGroupApplied()
    {
        await _tool.InitializeAsync(profile: CreateProfile(ProfileGroup));
        var data = _tool.CreateData("id", group: CustomGroup);
        Assert.That(data.Info.Key.Group, Is.EqualTo(ProfileGroup).And.Not.EqualTo(CustomGroup).And.Not.EqualTo(FallbackGroup));
    }

    [Test]
    public async Task CreateData_NoCustomGroup_ProfileGroup_ProfileGroupApplied()
    {
        await _tool.InitializeAsync(profile: CreateProfile(ProfileGroup));
        var data = _tool.CreateData("id");
        Assert.That(data.Info.Key.Group, Is.EqualTo(ProfileGroup).And.Not.EqualTo(CustomGroup).And.Not.EqualTo(FallbackGroup));
    }

    [Test]
    public async Task CreateData_NoCustomGroup_NoProfileGroup_FallbackGroupApplied()
    {
        await _tool.InitializeAsync(profile: CreateProfile(null));
        var data = _tool.CreateData("id");
        Assert.That(data.Info.Key.Group, Is.EqualTo(FallbackGroup).And.Not.EqualTo(ProfileGroup).And.Not.EqualTo(CustomGroup));
    }

    private ArtifactToolProfile CreateProfile(string? group)
    {
        return new ArtifactToolProfile(ArtifactToolIDUtil.CreateToolString(_tool.GetType()), group, null);
    }

    private class CustomGroupArtifactTool : ArtifactTool
    {
        public override string GroupFallback => FallbackGroup;
    }
}
