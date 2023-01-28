using Art.BrowserCookies.Util;

namespace Art.BrowserCookies.Chromium;

/// <summary>
/// Represents a <see cref="CookieSource"/>
/// </summary>
/// <param name="Profile">Profile name.</param>
public record EdgeCookieSource(string Profile = "Default") : ChromiumCookieSource
{
    internal const string Name = "Edge";

    /// <inheritdoc />
    public override void Validate()
    {
        try
        {
            GetCookieFilePath();
        }
        catch
        {
            throw new BrowserProfileNotFoundException(Name, Profile);
        }
    }

    /// <inheritdoc />
    public override Task<IKeychain> GetKeychainAsync(CancellationToken cancellationToken = default)
    {
        if (OperatingSystem.IsWindows())
        {
            return ChromiumKeychainUtil.GetWindowsKeychainAsync(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft/Edge/User Data"), cancellationToken);
        }
        if (OperatingSystem.IsMacOS())
        {
            return ChromiumKeychainUtil.GetMacosKeychainAsync("Microsoft Edge Safe Storage", cancellationToken);
        }
        if (OperatingSystem.IsLinux())
        {
            throw new NotImplementedException();
        }
        throw new PlatformNotSupportedException();
    }

    /// <inheritdoc />
    public override string GetCookieFilePath()
    {
        if (OperatingSystem.IsWindows())
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft/Edge/User Data", Profile, "Network/Cookies");
        }
        if (OperatingSystem.IsMacOS())
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library/Application Support/Microsoft Edge", Profile, "Cookies");
        }
        if (OperatingSystem.IsLinux())
        {
            return Path.Combine(PathUtil.GetXdgConfigHomeOrFallback(), "microsoft-edge", Profile, "Cookies");
        }
        throw new PlatformNotSupportedException();
    }
}
