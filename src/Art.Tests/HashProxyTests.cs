using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using Art.IO;
using NUnit.Framework;

namespace Art.Tests;

public class HashProxyTests
{
    [Test, Combinatorial]
    public void TestHashProxy(
        [Values(0, 8, 3098 * 1024)] int inputSize,
        [Values(typeof(SHA1), typeof(SHA256), typeof(SHA384), typeof(SHA512), typeof(MD5))]
        Type hashType)
    {
        MethodInfo? method = hashType.GetMethod("Create", BindingFlags.Static | BindingFlags.Public, Array.Empty<Type>());
        if (method == null)
        {
            Assert.Ignore($"Type {hashType.FullName} does not have a public static parameterless Create method");
            return;
        }
        if (method.Invoke(null, Array.Empty<object?>()) is not HashAlgorithm hashAlgorithm)
        {
            Assert.Ignore($"Return of Create method on {hashType.FullName} is not a hash algorithm instance");
            return;
        }
        TestHashProxy(inputSize, hashAlgorithm);
    }

    private static void TestHashProxy(int inputSize, HashAlgorithm hashAlgorithm)
    {
        byte[] arr = new byte[inputSize];
        Random.Shared.NextBytes(arr);
        byte[] expected = hashAlgorithm.ComputeHash(arr);
        HashProxyStream hps = new(new MemoryStream(arr), hashAlgorithm);
        hps.CopyTo(new MemoryStream());
        byte[] actual = hps.GetHash();
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void TestHashProxyDisposal()
    {
        HashProxyStream hps = new(new MemoryStream(), SHA256.Create());
        Assert.That(() => hps.Read(new byte[1], 0, 1), Throws.Nothing);
        hps.Dispose();
        Assert.That(() => hps.Read(new byte[1], 0, 1), Throws.InstanceOf<ObjectDisposedException>());
    }

    [Test]
    public void TestHashProxyNoDisposalFromGetHash()
    {
        HashProxyStream hps = new(new MemoryStream(), SHA256.Create());
        Assert.That(() => hps.Read(new byte[1], 0, 1), Throws.Nothing);
        hps.GetHash();
        Assert.That(() => hps.Read(new byte[1], 0, 1), Throws.Nothing);
    }
}
