using System.Text.Json;
using Art.BrowserCookies.Util;

namespace Art.BrowserCookies.Chromium;

/// <summary>
/// Represents a <see cref="CookieSource"/> for the Microsoft Edge web browser.
/// </summary>
/// <param name="Profile">Profile name.</param>
public record EdgeCookieSource(string Profile = "Default") : ChromiumCookieSource
{
    internal const string Name = "Edge";

    /// <inheritdoc />
    public override EdgeCookieSource Resolve()
    {
        try
        {
            string path = GetCookieFilePath();
            if (!File.Exists(path))
            {
                throw new BrowserProfileNotFoundException(Name, Profile);
            }
            return this;
        }
        catch
        {
            foreach (string profileDirectory in Directory.EnumerateDirectories(GetUserDataPath(), "*Profile*"))
            {
                string newProfile = Path.GetFileName(profileDirectory);
                string preferences = GetPath(newProfile, UserDataKind.Preferences);
                if (File.Exists(preferences))
                {
                    using var fs = File.OpenRead(preferences);
                    string name = (JsonSerializer.Deserialize<ChromiumPreferences>(fs) ?? throw new InvalidDataException()).Profile.Name;
                    if (name.Equals(Profile, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return this with { Profile = newProfile };
                    }
                }
            }
            throw;
        }
    }

    private static string GetUserDataPath()
    {
        if (OperatingSystem.IsWindows())
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft/Edge/User Data");
        }
        if (OperatingSystem.IsMacOS())
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library/Application Support/Microsoft Edge");
        }
        if (OperatingSystem.IsLinux())
        {
            return Path.Combine(PathUtil.GetXdgConfigHomeOrFallback(), "microsoft-edge");
        }
        throw new PlatformNotSupportedException();
    }

    private static string GetPath(string profile, UserDataKind kind)
    {
        string userDataPath = GetUserDataPath();
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
    public override Task<IChromiumKeychain> GetKeychainAsync(CancellationToken cancellationToken = default)
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
        return GetPath(Profile, UserDataKind.Cookies);
    }
}
