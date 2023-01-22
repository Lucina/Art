using System;
using System.IO;
using System.Threading.Tasks;
using Art.CipherPadding;
using NUnit.Framework;

namespace Art.Common.Tests.CipherPadding;

public class DepaddingTests
{
    [Test, Combinatorial]
    public async Task DepaddingStream_CopyToAsync_Pkcs5Blocks_Valid([Values(0, 1, 1024, 64 * 1024 - 1, 64 * 1024)] int blockCount)
    {
        byte[] buf = new byte[blockCount * 8];
        Random.Shared.NextBytes(buf);
        byte pad = 4;
        PadRepeatEnd(buf, pad);
        ArraySegment<byte> expected = GetDepadded(buf, pad);
        ArraySegment<byte> actual = await CopyToAsync(buf, new Pkcs5DepaddingHandler(8));
        Assert.That(expected.AsSpan().SequenceEqual(actual));
    }

    [Test, Combinatorial]
    public async Task DepaddingStream_CopyToAsync_Pkcs7Blocks_Valid([Values(0, 1, 1024, 64 * 1024 - 1, 64 * 1024)] int blockCount, [Values(1, 2, 127, 128)] int blockSize)
    {
        byte[] buf = new byte[blockCount * blockSize];
        Random.Shared.NextBytes(buf);
        byte pad = (byte)Math.Max(1, blockSize / 2);
        PadRepeatEnd(buf, pad);
        ArraySegment<byte> expected = GetDepadded(buf, pad);
        ArraySegment<byte> actual = await CopyToAsync(buf, new Pkcs7DepaddingHandler(blockSize));
        Assert.That(expected.AsSpan().SequenceEqual(actual));
    }

    [Test]
    public void DepaddingStream_CopyToAsync_Pkcs5BadPad_InvalidDataException()
    {
        byte[] buf = new byte[10 * 8];
        Random.Shared.NextBytes(buf);
        PadRepeatEnd(buf, 4);
        buf[^2] = 0x66;
        Assert.ThrowsAsync<InvalidDataException>(async () => await CopyToAsync(buf, new Pkcs5DepaddingHandler(8)));
    }

    [Test]
    public void DepaddingStream_CopyToAsync_Pkcs7BadPad_InvalidDataException()
    {
        byte[] buf = new byte[10 * 17];
        Random.Shared.NextBytes(buf);
        PadRepeatEnd(buf, 10);
        buf[^4] = 0x66;
        Assert.ThrowsAsync<InvalidDataException>(async () => await CopyToAsync(buf, new Pkcs7DepaddingHandler(17)));
    }

    [Test, Combinatorial]
    public void DepaddingStream_CopyToAsync_Pkcs5BadLength_InvalidDataException([Range(1, 7)] int sub)
    {
        byte[] buf = new byte[10 * 8 - sub];
        Random.Shared.NextBytes(buf);
        PadRepeatEnd(buf, 10);
        Assert.ThrowsAsync<InvalidDataException>(async () => await CopyToAsync(buf, new Pkcs7DepaddingHandler(17)));
    }

    [Test, Combinatorial]
    public void DepaddingStream_CopyToAsync_Pkcs7BadLength_InvalidDataException([Range(1, 16)] int sub)
    {
        byte[] buf = new byte[10 * 17 - sub];
        Random.Shared.NextBytes(buf);
        PadRepeatEnd(buf, 10);
        Assert.ThrowsAsync<InvalidDataException>(async () => await CopyToAsync(buf, new Pkcs7DepaddingHandler(17)));
    }

    private static void PadRepeatEnd(byte[] buf, byte v)
    {
        for (int i = buf.Length - 1, c = 0; i >= 0 && c < v; i--, c++)
        {
            buf[i] = v;
        }
    }

    private static async Task<ArraySegment<byte>> CopyToAsync(byte[] buf, DepaddingHandler handler)
    {
        MemoryStream ms = new();
        using (DepaddingStream ds = new(handler, ms, true))
            await new MemoryStream(buf).CopyToAsync(ds);
        ms.TryGetBuffer(out var bb);
        return bb;
    }

    private static ArraySegment<byte> GetDepadded(byte[] buf, int paddingBytes) => new(buf, 0, Math.Max(0, buf.Length - paddingBytes));
}
