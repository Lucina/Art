using System;
using System.IO;
using System.Threading.Tasks;
using Art.EF.Sqlite;
using NUnit.Framework;

namespace Art.Tests;

public class SqliteTests
{
    [Test]
    public async Task TestSqliteDatabaseFile()
    {
        string tempFile = Path.GetTempFileName();
        try
        {
            using SqliteArtifactRegistrationManager r = new(tempFile);
            await TestSqliteDatabase(r);
        }
        catch
        {
            try
            {
                File.Delete(tempFile);
            }
            catch
            {
                // ignored
            }
            throw;
        }
        for (int i = 10; --i >= 0;)
            try
            {
                File.Delete(tempFile);
            }
            catch
            {
                if (i == 0) throw;
                await Task.Delay(TimeSpan.FromSeconds(0.5));
            }
    }

    [Test]
    public async Task TestSqliteDatabaseMemory()
    {
        using SqliteArtifactRegistrationManager r = new();
        await TestSqliteDatabase(r);
    }

    private async Task TestSqliteDatabase(SqliteArtifactRegistrationManager r)
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

        await r.AddArtifactAsync(i1);

        await r.AddResourceAsync(i1_r1);
        await r.AddResourceAsync(i1_r2);

        await r.AddArtifactAsync(i1); // dupe

        await r.AddArtifactAsync(i2);

        await r.AddResourceAsync(i2_r1);
        await r.AddResourceAsync(i2_r2);
        await r.AddResourceAsync(i2_r3);

        await r.RemoveArtifactAsync(k1);
        await r.RemoveResourceAsync(i2_k3);

        Assert.That(await r.TryGetArtifactAsync(k1), Is.Null);
        Assert.That(await r.TryGetResourceAsync(i1_k1), Is.Null);
        Assert.That(await r.TryGetResourceAsync(i1_k2), Is.Null);
        Assert.That(await r.TryGetArtifactAsync(k2), Is.EqualTo(i2));
        Assert.That(await r.TryGetResourceAsync(i2_k1), Is.EqualTo(i2_r1));
        Assert.That(await r.TryGetResourceAsync(i2_k2), Is.EqualTo(i2_r2));
        Assert.That(await r.TryGetResourceAsync(i2_k3), Is.Null);
        Assert.That(await r.ListArtifactsAsync("abec").CountAsync(), Is.EqualTo(1));
        Assert.That(await r.ListArtifactsAsync("abec", "group1").CountAsync(), Is.EqualTo(1));
        Assert.That(await r.ListArtifactsAsync("abec2", "group1").CountAsync(), Is.EqualTo(0));
        Assert.That(await r.ListResourcesAsync(k2).CountAsync(), Is.EqualTo(2));
        Assert.That(await r.ListResourcesAsync(k1).CountAsync(), Is.EqualTo(0));
    }
}
