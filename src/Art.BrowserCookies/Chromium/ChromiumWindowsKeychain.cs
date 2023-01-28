using System.Security.Cryptography;
using System.Text;

namespace Art.BrowserCookies.Chromium;

internal class ChromiumWindowsKeychain : IKeychain
{
    // https://chromium.googlesource.com/chromium/src/+/refs/heads/main/components/os_crypt/os_crypt_win.cc
    // https://armmbed.github.io/mbed-crypto/html/api/ops/aead.html
    private const int AeadTagBytes = 16;
    private const int AeadKeyBytes = 256 / 8;
    private const int AeadNonceBytes = 96 / 8;

    private readonly AesGcm _aes;
    private bool _disposed;

    public ChromiumWindowsKeychain(byte[] keyV10)
    {
        if (keyV10.Length != AeadKeyBytes)
        {
            throw new ArgumentException();
        }
        _aes = new AesGcm(keyV10);
    }

    public string Unlock(byte[] buffer)
    {
        if (!OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException();
        }
        EnsureNotDisposed();
        ReadOnlySpan<byte> src = buffer;
        if (!src[..3].SequenceEqual("v10"u8))
        {
            // fallback
            return Encoding.UTF8.GetString(ProtectedData.Unprotect(buffer, null, DataProtectionScope.CurrentUser));
        }
        src = src[3..];
        var nonce = src[..AeadNonceBytes];
        src = src[AeadNonceBytes..];
        var tag = src[^AeadTagBytes..];
        src = src[..^AeadTagBytes];
        Span<byte> target = stackalloc byte[src.Length];
        _aes.Decrypt(nonce, src, tag, target);
        return Encoding.UTF8.GetString(target);
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
            throw new ObjectDisposedException(nameof(ChromiumWindowsKeychain));
        }
    }
}
