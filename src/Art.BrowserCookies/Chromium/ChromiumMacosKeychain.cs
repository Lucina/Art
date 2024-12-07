using System.Buffers;
using System.Security.Cryptography;
using System.Text;

namespace Art.BrowserCookies.Chromium;

internal class ChromiumMacosKeychain : IChromiumKeychain
{
    // https://chromium.googlesource.com/chromium/src/+/refs/heads/main/components/os_crypt/os_crypt_mac.mm
    private readonly Aes _aes;
    private bool _disposed;

    public ChromiumMacosKeychain(ReadOnlySpan<char> source)
    {
        byte[] keyV10 = new byte[16];
        Rfc2898DeriveBytes.Pbkdf2(source, "saltysalt"u8, keyV10, 1003, HashAlgorithmName.SHA1);
        _aes = Aes.Create();
        _aes.Key = keyV10;
        keyV10.AsSpan().Clear();
    }

    public string Unlock(byte[] buffer, IToolLogHandler? toolLogHandler)
    {
        EnsureNotDisposed();
        ReadOnlySpan<byte> src = buffer;
        if (!src[..3].SequenceEqual("v10"u8))
        {
            // fallback
            return Encoding.UTF8.GetString(src);
        }
        src = src[3..];
        Span<byte> iv = stackalloc byte[16];
        iv.Fill((int)' ');
        byte[] buf = ArrayPool<byte>.Shared.Rent(src.Length);
        Span<byte> span = buf;
        try
        {
            int count = _aes.DecryptCbc(src, iv, span, paddingMode: PaddingMode.PKCS7);
            return Encoding.UTF8.GetString(buf[..count]);
        }
        finally
        {
            span.Clear();
            ArrayPool<byte>.Shared.Return(buf);
        }
    }

    public void Dispose()
    {
        _disposed = true;
        _aes.Dispose();
    }

    private void EnsureNotDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(ChromiumMacosKeychain));
        }
    }
}
