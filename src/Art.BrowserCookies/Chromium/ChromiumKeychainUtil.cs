using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;

namespace Art.BrowserCookies.Chromium;

internal static class ChromiumKeychainUtil
{
    private const int MaxPasswordSize = 1024;

    public static IChromiumKeychain GetMacosKeychain(string service)
    {
        var process = new Process { StartInfo = { FileName = "security", ArgumentList = { "find-generic-password", "-ws", service }, RedirectStandardOutput = true, UseShellExecute = false } };
        process.Start();
        Span<char> buf = new char[MaxPasswordSize];
        try
        {
            int n = process.StandardOutput.ReadBlock(buf);
            if (n == MaxPasswordSize && process.StandardOutput.Peek() != -1)
            {
                throw new InvalidDataException("Password exceeds max length");
            }
            process.WaitForExit();
            return new ChromiumMacosKeychain(buf[..n].Trim());
        }
        finally
        {
            buf.Clear();
        }
    }

    public static async Task<IChromiumKeychain> GetMacosKeychainAsync(string service, CancellationToken cancellationToken = default)
    {
        var process = new Process { StartInfo = { FileName = "security", ArgumentList = { "find-generic-password", "-ws", service }, RedirectStandardOutput = true, UseShellExecute = false } };
        process.Start();
        Memory<char> buf = new char[MaxPasswordSize];
        try
        {
            int n = await process.StandardOutput.ReadBlockAsync(buf, cancellationToken).ConfigureAwait(false);
            if (n == MaxPasswordSize && process.StandardOutput.Peek() != -1)
            {
                throw new InvalidDataException("Password exceeds max length");
            }
            await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
            return new ChromiumMacosKeychain(buf.Span[..n].Trim());
        }
        finally
        {
            buf.Span.Clear();
        }
    }

    public static IChromiumKeychain GetWindowsKeychain(string userDataPath)
    {
        if (!OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException();
        }
        string file = Path.Combine(userDataPath, "Local State");
        string encryptedKey;
        using (var stream = File.OpenRead(file))
        {
            encryptedKey = (JsonSerializer.Deserialize(stream, SourceGenerationContext.Default.ChromiumWindowsLocalState) ?? throw new InvalidDataException()).OsCrypt.EncryptedKey;
        }
        byte[] data = Convert.FromBase64String(encryptedKey)[5..];
        byte[] res = ProtectedData.Unprotect(data, null, DataProtectionScope.CurrentUser);
        data.AsSpan().Clear();
        var keychain = new ChromiumWindowsKeychain(res);
        res.AsSpan().Clear();
        return keychain;
    }

    public static async Task<IChromiumKeychain> GetWindowsKeychainAsync(string userDataPath, CancellationToken cancellationToken = default)
    {
        if (!OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException();
        }
        string file = Path.Combine(userDataPath, "Local State");
        string encryptedKey;
        using (var stream = File.OpenRead(file))
        {
            encryptedKey = (await JsonSerializer.DeserializeAsync(stream, SourceGenerationContext.Default.ChromiumWindowsLocalState, cancellationToken: cancellationToken).ConfigureAwait(false) ?? throw new InvalidDataException()).OsCrypt.EncryptedKey;
        }
        byte[] data = Convert.FromBase64String(encryptedKey)[5..];
        byte[] res = ProtectedData.Unprotect(data, null, DataProtectionScope.CurrentUser);
        data.AsSpan().Clear();
        var keychain = new ChromiumWindowsKeychain(res);
        res.AsSpan().Clear();
        return keychain;
    }
}
