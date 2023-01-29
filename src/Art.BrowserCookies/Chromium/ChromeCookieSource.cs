namespace Art.BrowserCookies.Chromium;

/// <summary>
/// Represents a <see cref="CookieSource"/> for the Google Chrome web browser.
/// </summary>
/// <param name="Profile">Profile name.</param>
public record ChromeCookieSource(string Profile = "Default") : ChromiumCookieSource
{
    internal const string Name = "Chrome";

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
    public override Task<IChromiumKeychain> GetKeychainAsync(CancellationToken cancellationToken = default)
    {
        if (OperatingSystem.IsWindows())
        {
            return ChromiumKeychainUtil.GetWindowsKeychainAsync(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Google/Chrome/User Data"), cancellationToken);
        }
        if (OperatingSystem.IsMacOS())
        {
            throw new NotImplementedException();
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
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Google/Chrome/User Data", Profile, "Network/Cookies");
        }
        if (OperatingSystem.IsMacOS())
        {
            throw new NotImplementedException();
        }
        if (OperatingSystem.IsLinux())
        {
            throw new NotImplementedException();
        }
        throw new PlatformNotSupportedException();
    }
}
