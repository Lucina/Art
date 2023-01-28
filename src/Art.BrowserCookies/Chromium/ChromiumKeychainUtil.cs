using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Art.BrowserCookies.Chromium;

internal static class ChromiumKeychainUtil
{
    private const int MaxPasswordSize = 1024;

    public static async Task<IKeychain> GetMacosKeychainAsync(string service, CancellationToken cancellationToken = default)
    {
        var process = new Process { StartInfo = { FileName = "security", ArgumentList = { "find-generic-password", "-ws", service }, RedirectStandardOutput = true, UseShellExecute = false } };
        process.Start();
        Memory<char> buf = new char[MaxPasswordSize];
        try
        {
            int n = await process.StandardOutput.ReadBlockAsync(buf, cancellationToken);
            if (n == MaxPasswordSize && process.StandardOutput.Peek() != -1)
            {
                throw new InvalidDataException("Password exceeds max length");
            }
            await process.WaitForExitAsync(cancellationToken);
            return new ChromiumMacosKeychain(buf.Span[..n].Trim());
        }
        finally
        {
            buf.Span.Clear();
        }
    }

    public static async Task<IKeychain> GetWindowsKeychainAsync(string userDataPath, CancellationToken cancellationToken = default)
    {
        if (!OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException();
        }
        string file = Path.Combine(userDataPath, "Local State");
        string encryptedKey;
        using (var stream = File.OpenRead(file))
        {
            encryptedKey = (await JsonSerializer.DeserializeAsync<WindowsLocalState>(stream, cancellationToken: cancellationToken) ?? throw new InvalidDataException()).OsCrypt.EncryptedKey;
        }
        byte[] data = Convert.FromBase64String(encryptedKey)[5..];
        byte[] res = ProtectedData.Unprotect(data, null, DataProtectionScope.CurrentUser);
        data.AsSpan().Clear();
        var keychain = new ChromiumWindowsKeychain(res);
        res.AsSpan().Clear();
        return keychain;
    }

    private record WindowsLocalState([property: JsonPropertyName("os_crypt")] WindowsLocalStateOsCrypt OsCrypt);

    private record WindowsLocalStateOsCrypt([property: JsonPropertyName("encrypted_key")] string EncryptedKey);
}
