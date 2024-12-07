using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
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

    public static IChromiumKeychain GetWindowsKeychain(ChromiumVariant chromiumVariant, string userDataPath, IToolLogHandler? toolLogHandler = null)
    {
        if (!OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException();
        }
        string file = Path.Combine(userDataPath, "Local State");
        using var stream = File.OpenRead(file);
        return GetWindowsKeychainInternal(chromiumVariant, JsonSerializer.Deserialize(stream, SourceGenerationContext.Default.ChromiumWindowsLocalState) ?? throw new InvalidDataException(), toolLogHandler);
    }

    public static async Task<IChromiumKeychain> GetWindowsKeychainAsync(ChromiumVariant chromiumVariant, string userDataPath, IToolLogHandler? toolLogHandler = null, CancellationToken cancellationToken = default)
    {
        if (!OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException();
        }
        string file = Path.Combine(userDataPath, "Local State");
        await using var stream = File.OpenRead(file);
        return GetWindowsKeychainInternal(chromiumVariant, await JsonSerializer.DeserializeAsync(stream, SourceGenerationContext.Default.ChromiumWindowsLocalState, cancellationToken: cancellationToken).ConfigureAwait(false) ?? throw new InvalidDataException(), toolLogHandler);
    }

    private static Func<string>[] s_wcunlockASearchPathFuncs =
    [
        () => Path.Join(GetBaseDirectory(typeof(ChromiumKeychainUtil).Assembly), "runtimes", RuntimeInformation.RuntimeIdentifier, "native", "wcunlockA.exe"), //
        () => Path.Join(GetBaseDirectory(typeof(ChromiumKeychainUtil).Assembly), "wcunlockA.exe") //
    ];

    private static string? GetFile(IEnumerable<Func<string>> pathFuncs)
    {
        foreach (var func in pathFuncs)
        {
            string path = func();
            if (File.Exists(path))
            {
                return path;
            }
        }
        return null;
    }

    private static string GetBaseDirectory(Assembly assembly)
    {
        var context = AssemblyLoadContext.GetLoadContext(assembly);
        if (context is IBasedAssemblyLoadContext basedAssemblyLoadContext)
        {
            return basedAssemblyLoadContext.BasePath;
        }
        return AppDomain.CurrentDomain.BaseDirectory;
    }

    private static ChromiumWindowsKeychain GetWindowsKeychainInternal(ChromiumVariant chromiumVariant, ChromiumWindowsLocalState state, IToolLogHandler? toolLogHandler)
    {
        if (!OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException();
        }
        byte[] data10 = Convert.FromBase64String(state.OsCrypt.EncryptedKey)[5..];
        byte[] res10 = ProtectedData.Unprotect(data10, null, DataProtectionScope.CurrentUser);
        data10.AsSpan().Clear();
        byte[] data20 = Convert.FromBase64String(state.OsCrypt.AppBoundEncryptedKey);
        if (!data20.AsSpan().StartsWith("APPB"u8))
        {
            throw new InvalidDataException();
        }
        byte[] data20Sub = data20[4..];
        data20.AsSpan().Clear();
        string? wcunlockA = GetFile(s_wcunlockASearchPathFuncs);
        if (wcunlockA == null)
        {
            throw new FileNotFoundException("Missing required executable wcunlockA.exe");
        }
        string tmpIn = Path.GetTempFileName();
        try
        {
            string tmpOut = Path.GetTempFileName();
            try
            {
                File.WriteAllBytes(tmpIn, data20Sub);
                data20Sub.AsSpan().Clear();
                ProcessStartInfo psi = new() { FileName = "psexec", Verb = "runas", UseShellExecute = true };
                psi.ArgumentList.Add("-nobanner");
                psi.ArgumentList.Add("-s");
                psi.ArgumentList.Add(wcunlockA);
                psi.ArgumentList.Add(tmpIn);
                psi.ArgumentList.Add(tmpOut);
                toolLogHandler?.Log("Need Elevation", "Elevation is needed to decrypt keys. A UAC prompt may appear.", LogLevel.Information);
                try
                {
                    var process = Process.Start(psi);
                    toolLogHandler?.Log("Running decryption helper via psexec...", null, LogLevel.Information);
                    process?.WaitForExit();
                }
                catch (Exception e)
                {
                    throw new IOException("An error occurred. Please make sure psexec is on your path.", e);
                }
                byte[] res20 = ProtectedData.Unprotect(File.ReadAllBytes(tmpOut), null, DataProtectionScope.CurrentUser);
                var keychain = new ChromiumWindowsKeychain(res10, res20, chromiumVariant);
                res10.AsSpan().Clear();
                res20.AsSpan().Clear();
                return keychain;
            }
            finally
            {
                File.Delete(tmpOut);
            }
        }
        finally
        {
            File.Delete(tmpIn);
        }
    }
}
