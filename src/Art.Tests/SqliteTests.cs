using System;
using System.IO;
using System.Threading.Tasks;
using Art.EF.Sqlite;
using NUnit.Framework;

namespace Art.Tests;

public class SqliteTests
{
    private string _tempFile = null!;
    private SqliteArtifactRegistrationManager _r = null!;

    [SetUp]
    public void Setup()
    {
        _tempFile = Path.GetTempFileName();
        _r = new SqliteArtifactRegistrationManager(_tempFile);
    }

    [TearDown]
    public void Teardown()
    {
        try
        {
            _r.Dispose();
        }
        finally
        {
            File.Delete(_tempFile);
        }
    }

    [Test]
    public async Task TestDatabaseWorks()
    {
        // 1
        ArtifactKey k1 = new("abec", "group1", "kraft1");
        ArtifactInfo i1 = new(k1, DateTimeOffset.FromUnixTimeSeconds(1636785227L));
        ArtifactResourceKey i1_k1 = new(k1, "file1");
        ArtifactResourceInfo i1_r1 = new(i1_k1, "v0");
        ArtifactResourceKey i1_k2 = new(k1, "file2", "somepath");
        ArtifactResourceInfo i1_r2 = new(i1_k2);
        // 2
        ArtifactKey k2 = new("abec", "group1", "kraft2");
        ArtifactInfo i2 = new(k2, DateTimeOffset.FromUnixTimeSeconds(1636785227L));
        ArtifactResourceKey i2_k1 = new(k2, "file1");
        ArtifactResourceInfo i2_r1 = new(i2_k1, "v0");
        ArtifactResourceKey i2_k2 = new(k2, "file2", "somepath");
        ArtifactResourceInfo i2_r2 = new(i2_k2);
        ArtifactResourceKey i2_k3 = new(k2, "DUM");
        ArtifactResourceInfo i2_r3 = new(i2_k3);

        await _r.AddArtifactAsync(i1);

        await _r.AddResourceAsync(i1_r1);
        await _r.AddResourceAsync(i1_r2);

        await _r.AddArtifactAsync(i1); // dupe

        await _r.AddArtifactAsync(i2);

        await _r.AddResourceAsync(i2_r1);
        await _r.AddResourceAsync(i2_r2);
        await _r.AddResourceAsync(i2_r3);

        await _r.RemoveArtifactAsync(k1);
        await _r.RemoveResourceAsync(i2_k3);

        Assert.That(await _r.TryGetArtifactAsync(k1), Is.Null);
        Assert.That(await _r.TryGetResourceAsync(i1_k1), Is.Null);
        Assert.That(await _r.TryGetResourceAsync(i1_k2), Is.Null);
        Assert.That(await _r.TryGetArtifactAsync(k2), Is.EqualTo(i2));
        Assert.That(await _r.TryGetResourceAsync(i2_k1), Is.EqualTo(i2_r1));
        Assert.That(await _r.TryGetResourceAsync(i2_k2), Is.EqualTo(i2_r2));
        Assert.That(await _r.TryGetResourceAsync(i2_k3), Is.Null);
        Assert.Pass();
    }
}
