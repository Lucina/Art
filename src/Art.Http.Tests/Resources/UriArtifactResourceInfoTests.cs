using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Art.Crypto;
using Art.Http.Resources;
using Art.Resources;
using Art.Web;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using PaddingMode = System.Security.Cryptography.PaddingMode;

namespace Art.Http.Tests.Resources;

public class UriArtifactResourceInfoTests : HttpTestsBase
{
    [Test]
    public void WithMetadataAsync_Nonexistent_Throws()
    {
        MockHandler.When("http://localhost/harmony.bin").Respond(HttpStatusCode.NotFound);
        var ari = Data.Uri("http://localhost/harmony.bin", "file");
        Assert.That(() => ari.Info.WithMetadataAsync().AsTask().Wait(),
            Throws.InstanceOf<AggregateException>().With.InnerException.Matches(
                Is.InstanceOf<ArtHttpResponseMessageException>().With.Property("StatusCode").EqualTo(HttpStatusCode.NotFound)));
    }

    [Test]
    public void ExportStreamAsync_Nonexistent_Throws()
    {
        MockHandler.When("http://localhost/wintercontingency.bin").Respond(HttpStatusCode.NotFound);
        var inf = Data.Uri("http://localhost/wintercontingency.bin", "file").Info;
        Assert.That(() => inf.ExportStreamAsync(Stream.Null).AsTask().Wait(),
            Throws.InstanceOf<AggregateException>().With.InnerException.Matches(
                Is.InstanceOf<ArtHttpResponseMessageException>().With.Property("StatusCode").EqualTo(HttpStatusCode.NotFound)));
    }

    [Test]
    public async Task WithMetadataAsync_Json_IsJson()
    {
        MockHandler.When("http://localhost/harmony.json").Respond("application/json", @"null");
        var ari = Data.Uri("http://localhost/harmony.json", "file");
        var met = await ari.Info.WithMetadataAsync();
        Assert.That(met.ContentType, Is.EqualTo("application/json"));
    }

    [Test]
    public async Task ExportStreamAsync_Data_Matches()
    {
        PreludeRandomContent(30495, out byte[] originalData, out MemoryStream source);
        MockHandler.When("http://localhost/wintercontingency.bin").Respond("application/octet-bytes", source);
        var inf = Data.Uri("http://localhost/wintercontingency.bin", "file").Info;
        MemoryStream read = new();
        await inf.ExportStreamAsync(read);
        byte[] actual = read.ToArray();
        Assert.That(actual.Length, Is.EqualTo(originalData.Length));
        Assert.That(actual.AsSpan().SequenceEqual(originalData));
    }

    [Test]
    public async Task ExportStreamAsync_Aes256Encryption_Matches()
    {
        PreludeRandomEncryptedContent(Aes.Create, CipherMode.CBC, PaddingMode.PKCS7, 30495, 256, 128, out byte[] originalData, out byte[] key, out byte[] iv, out MemoryStream encrypted);
        MockHandler.When("http://localhost/wintercontingency.bin").Respond("application/octet-bytes", encrypted);
        var inf = Data.Uri("http://localhost/wintercontingency.bin", "file")
            .WithEncryption(CryptoAlgorithm.Aes, key, encIv: iv, mode: CipherMode.CBC, paddingMode: PaddingMode.PKCS7).Info;
        MemoryStream decrypted = new();
        await inf.ExportStreamAsync(decrypted);
        byte[] newDec = decrypted.ToArray();
        Assert.That(newDec.Length, Is.EqualTo(originalData.Length));
        Assert.That(newDec.AsSpan().SequenceEqual(originalData));
    }

    [Test]
    public async Task ExportStreamAsync_Aes256EncryptionWithManualPadding_Matches()
    {
        PreludeRandomEncryptedContent(Aes.Create, CipherMode.CBC, PaddingMode.PKCS7, 30495, 256, 128, out byte[] originalData, out byte[] key, out byte[] iv, out MemoryStream encrypted);
        MockHandler.When("http://localhost/wintercontingency.bin").Respond("application/octet-bytes", encrypted);
        var inf = Data.Uri("http://localhost/wintercontingency.bin", "file")
            .WithEncryption(CryptoAlgorithm.Aes, key, encIv: iv, mode: CipherMode.CBC, paddingMode: PaddingMode.None)
            .WithPadding(ArtPaddingMode.Pkcs7).Info;
        MemoryStream decrypted = new();
        await inf.ExportStreamAsync(decrypted);
        byte[] newDec = decrypted.ToArray();
        Assert.That(newDec.Length, Is.EqualTo(originalData.Length));
        Assert.That(newDec.AsSpan().SequenceEqual(originalData));
    }
}
