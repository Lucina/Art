using System.Text.Json;
using Art.Common.Management;
using Art.Common.Proxies;
using Art.TestsBase;
using Microsoft.Extensions.Time.Testing;
using NUnit.Framework;

namespace Art.Common.Tests;

public class ArtifactToolDumpProxyTests
{
    [Test]
    public async Task FindOnlyTool_WithoutArtifactList_Throws()
    {
        var profile = new ArtifactToolProfile("tool", null, null);
        var tool = new ProgrammableArtifactFindTool((v, k) =>
        {
            int i = int.Parse(k);
            if (1 <= i && i <= 3)
            {
                return v.CreateData($"{i}");
            }
            return null;
        });
        await tool.InitializeAsync(profile: profile);
        var proxy = new ArtifactToolDumpProxy(tool, new ArtifactToolDumpOptions(), null);
        Assert.That(async () => await proxy.DumpAsync(), Throws.InstanceOf<NotSupportedException>());
    }

    [Test]
    public async Task FindOnlyTool_WithArtifactList_Success()
    {
        var options = new Dictionary<string, JsonElement> { { "artifactList", JsonSerializer.SerializeToElement(new[] { "1", "2", "3" }) } };
        var profile = new ArtifactToolProfile("tool", null, options);
        var arm = new InMemoryArtifactRegistrationManager();
        var config = new ArtifactToolConfig(arm, new NullArtifactDataManager(), new FakeTimeProvider());
        var tool = new ProgrammableArtifactFindTool((v, k) =>
        {
            int i = int.Parse(k);
            if (1 <= i && i <= 3)
            {
                return v.CreateData($"{i}");
            }
            return null;
        });
        await tool.InitializeAsync(config: config, profile: profile);
        var proxy = new ArtifactToolDumpProxy(tool, new ArtifactToolDumpOptions(), null);
        await proxy.DumpAsync();
        Assert.That((await arm.ListArtifactsAsync()).Select(v => int.Parse(v.Key.Id)), Is.EquivalentTo(new[] { 1, 2, 3 }));
    }

    [Test]
    public async Task DumpOnlyTool_WithoutArtifactList_Success()
    {
        var options = new Dictionary<string, JsonElement> { { "artifactList", JsonSerializer.SerializeToElement(new[] { "1", "2", "3" }) } };
        var profile = new ArtifactToolProfile("tool", null, options);
        var arm = new InMemoryArtifactRegistrationManager();
        var config = new ArtifactToolConfig(arm, new NullArtifactDataManager(), new FakeTimeProvider());
        var tool = new AsyncProgrammableArtifactDumpTool(async v =>
        {
            for (int i = 1; i <= 3; i++)
            {
                await v.DumpArtifactAsync(v.CreateData($"{i}"));
            }
        });
        await tool.InitializeAsync(config: config, profile: profile);
        var proxy = new ArtifactToolDumpProxy(tool, new ArtifactToolDumpOptions(), null);
        await proxy.DumpAsync();
        Assert.That((await arm.ListArtifactsAsync()).Select(v => int.Parse(v.Key.Id)), Is.EquivalentTo(new[] { 1, 2, 3 }));
    }

    [Test]
    public async Task DumpOnlyTool_WithArtifactList_DoesNotFilter()
    {
        var options = new Dictionary<string, JsonElement> { { "artifactList", JsonSerializer.SerializeToElement(new[] { "1", "2" }) } };
        var profile = new ArtifactToolProfile("tool", null, options);
        var arm = new InMemoryArtifactRegistrationManager();
        var config = new ArtifactToolConfig(arm, new NullArtifactDataManager(), new FakeTimeProvider());
        var tool = new AsyncProgrammableArtifactDumpTool(async v =>
        {
            for (int i = 1; i <= 3; i++)
            {
                await v.DumpArtifactAsync(v.CreateData($"{i}"));
            }
        });
        await tool.InitializeAsync(config: config, profile: profile);
        var proxy = new ArtifactToolDumpProxy(tool, new ArtifactToolDumpOptions(), null);
        await proxy.DumpAsync();
        Assert.That((await arm.ListArtifactsAsync()).Select(v => int.Parse(v.Key.Id)), Is.EquivalentTo(new[] { 1, 2, 3 }));
    }

    // TODO prioritisation tests
}
