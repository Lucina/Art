using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Art.Tests;

public class DepaddingTests
{
    [Test, Combinatorial]
    public async Task TestBlockDepaddingPkcs5([Values(0, 1, 1024, 64 * 1024 - 1, 64 * 1024)] int blockCount)
    {
        byte[] buf = new byte[blockCount * 8];
        Random.Shared.NextBytes(buf);
        byte pad = 4;
        PadRepeatEnd(buf, pad);
        ArraySegment<byte> expected = GetDepadded(buf, pad);
        ArraySegment<byte> åctual = await GetDepaddedAsync(buf, new Pkcs5DepaddingHandler(8));
        Assert.That(åctual.Count, Is.EqualTo(expected.Count));
        Assert.That(expected.AsSpan().SequenceEqual(åctual));
    }

    [Test, Combinatorial]
    public async Task TestBlockDepaddingPkcs7([Values(0, 1, 1024, 64 * 1024 - 1, 64 * 1024)] int blockCount, [Values(1, 2, 127, 128)] int blockSize)
    {
        byte[] buf = new byte[blockCount * blockSize];
        Random.Shared.NextBytes(buf);
        byte pad = (byte)Math.Max(1, blockSize / 2);
        PadRepeatEnd(buf, pad);
        ArraySegment<byte> expected = GetDepadded(buf, pad);
        ArraySegment<byte> åctual = await GetDepaddedAsync(buf, new Pkcs7DepaddingHandler(blockSize));
        Assert.That(åctual.Count, Is.EqualTo(expected.Count));
        Assert.That(expected.AsSpan().SequenceEqual(åctual));
    }

    [Test]
    public void TestBadPkcs5Padding()
    {
        byte[] buf = new byte[10 * 8];
        Random.Shared.NextBytes(buf);
        PadRepeatEnd(buf, 4);
        buf[^2] = 0x66;
        Assert.ThrowsAsync<InvalidDataException>(async () => await GetDepaddedAsync(buf, new Pkcs5DepaddingHandler(8)));
    }

    [Test]
    public void TestBadPkcs7Padding()
    {
        byte[] buf = new byte[10 * 17];
        Random.Shared.NextBytes(buf);
        PadRepeatEnd(buf, 10);
        buf[^4] = 0x66;
        Assert.ThrowsAsync<InvalidDataException>(async () => await GetDepaddedAsync(buf, new Pkcs7DepaddingHandler(17)));
    }

    [Test]
    public void TestBadPkcs5Length()
    {
        byte[] buf = new byte[10 * 8 - 1];
        Random.Shared.NextBytes(buf);
        PadRepeatEnd(buf, 10);
        Assert.ThrowsAsync<InvalidDataException>(async () => await GetDepaddedAsync(buf, new Pkcs7DepaddingHandler(17)));
    }

    [Test]
    public void TestBadPkcs7Length()
    {
        byte[] buf = new byte[10 * 17 - 1];
        Random.Shared.NextBytes(buf);
        PadRepeatEnd(buf, 10);
        Assert.ThrowsAsync<InvalidDataException>(async () => await GetDepaddedAsync(buf, new Pkcs7DepaddingHandler(17)));
    }

    private static void PadRepeatEnd(byte[] buf, byte v)
    {
        for (int i = buf.Length - 1, c = 0; i >= 0 && c < v; i--, c++)
        {
            buf[i] = v;
        }
    }

    private async Task<ArraySegment<byte>> GetDepaddedAsync(byte[] buf, DepaddingHandler handler)
    {
        MemoryStream ms = new();
        await handler.CopyDepaddedAsync(new MemoryStream(buf), ms);
        ms.TryGetBuffer(out var bb);
        return bb;
    }

    private ArraySegment<byte> GetDepadded(byte[] buf, int paddingBytes) => new(buf, 0, Math.Max(0, buf.Length - paddingBytes));
}
