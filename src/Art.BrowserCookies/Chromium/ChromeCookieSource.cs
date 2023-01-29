namespace Art.BrowserCookies.Chromium;

/// <summary>
/// Represents a <see cref="CookieSource"/> for the Google Chrome web browser.
/// </summary>
/// <param name="Profile">Profile name.</param>
public record ChromeCookieSource(string Profile = "Default") : ChromiumProfileCookieSource(Profile)
{
    /// <inheritdoc />
    public override string Name => "Chrome";

    /// <inheritdoc />
    protected override string GetUserDataDirectory()
    {
        if (OperatingSystem.IsWindows())
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Google/Chrome/User Data");
        }
        if (OperatingSystem.IsMacOS())
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library/Application Support/Google/Chrome");
        }
        if (OperatingSystem.IsLinux())
        {
            throw new NotImplementedException();
        }
        throw new PlatformNotSupportedException();
    }

    /// <inheritdoc />
    protected override string GetPath(UserDataKind kind, string profile)
    {
        string userDataPath = GetUserDataDirectory();
        if (OperatingSystem.IsWindows())
        {
            return kind switch
            {
                UserDataKind.Cookies => Path.Combine(userDataPath, profile, "Network/Cookies"),
                UserDataKind.Preferences => Path.Combine(userDataPath, profile, "Preferences"),
                _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null)
            };
        }
        if (OperatingSystem.IsMacOS())
        {
            return kind switch
            {
                UserDataKind.Cookies => Path.Combine(userDataPath, profile, "Cookies"),
                UserDataKind.Preferences => Path.Combine(userDataPath, profile, "Preferences"),
                _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null)
            };
        }
        if (OperatingSystem.IsLinux())
        {
            return kind switch
            {
                UserDataKind.Cookies => Path.Combine(userDataPath, profile, "Cookies"),
                UserDataKind.Preferences => Path.Combine(userDataPath, profile, "Preferences"),
                _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null)
            };
        }
        throw new PlatformNotSupportedException();
    }

    /// <inheritdoc />
    protected override Task<IChromiumKeychain> GetKeychainAsync(CancellationToken cancellationToken = default)
    {
        if (OperatingSystem.IsWindows())
        {
            return ChromiumKeychainUtil.GetWindowsKeychainAsync(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Google/Chrome/User Data"), cancellationToken);
        }
        if (OperatingSystem.IsMacOS())
        {
            return ChromiumKeychainUtil.GetMacosKeychainAsync("Chrome Safe Storage", cancellationToken);
        }
        if (OperatingSystem.IsLinux())
        {
            throw new NotImplementedException();
        }
        throw new PlatformNotSupportedException();
    }
}
