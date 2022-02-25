using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Art.Web;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace Art.Tests;

public class HttpTestsBase
{
    protected class TestHttpArtifactTool : HttpArtifactTool
    {
    }

    protected HttpArtifactTool Tool = null!;
    protected MockHttpMessageHandler MockHandler = null!;
    protected ArtifactData Data = null!;

    [SetUp]
    public async Task SetUpAsync()
    {
        Tool = new TestHttpArtifactTool();
        await Tool.InitializeAsync();
        MockHandler = new MockHttpMessageHandler();
        Tool.HttpClient = MockHandler.ToHttpClient();
        Data = Tool.CreateData("default");
    }

    [TearDown]
    public void TearDown()
    {
        Tool.Dispose();
    }

    protected static void PreludeRandomContent(int length, out byte[] originalData, out MemoryStream source)
    {
        originalData = new byte[length];
        Random.Shared.NextBytes(originalData);
        source = new MemoryStream(originalData);
    }

    protected static void PreludeRandomEncryptedContent(Func<SymmetricAlgorithm> saf, CipherMode mode, PaddingMode paddingMode, int length, int keyBits, int ivBits, out byte[] originalData, out byte[] key, out byte[] iv, out MemoryStream encrypted)
    {
        originalData = new byte[length];
        key = new byte[keyBits / 8];
        iv = new byte[ivBits / 8];
        Random.Shared.NextBytes(originalData);
        Random.Shared.NextBytes(key);
        Random.Shared.NextBytes(iv);
        encrypted = new MemoryStream();
        using SymmetricAlgorithm sa = saf();
        sa.Mode = mode;
        sa.Padding = paddingMode;
        using ICryptoTransform enc = sa.CreateEncryptor(key, iv);
        using CryptoStream cs = new(encrypted, enc, CryptoStreamMode.Write, true);
        cs.Write(originalData);
        cs.FlushFinalBlock();
        encrypted.Position = 0;
    }
}
