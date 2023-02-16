using System.Diagnostics.CodeAnalysis;
using System.Net;
using Art.BrowserCookies.Chromium;

namespace Art.BrowserCookies;

/// <summary>
/// Represents a browser or browser profile that can be used to retrieve cookies.
/// </summary>
public abstract record CookieSource
{
    /// <summary>
    /// A simple name that identifies this browser type.
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Attempts to resolve the provided source to a valid configuration.
    /// </summary>
    /// <exception cref="BrowserProfileNotFoundException">Thrown for unknown browser profile name.</exception>
    /// <remarks>Implementations can, for example, attempt to resolve a user-configured profile name to the profile directory name on the filesystem.</remarks>
    public abstract CookieSource Resolve();

    private static readonly Dictionary<string, BrowserInfo> s_info = new(StringComparer.InvariantCultureIgnoreCase);

    static CookieSource()
    {
        AddBrowserInfo<EdgeCookieSource>("edge"); // MS Edge
        AddBrowserInfo<ChromeCookieSource>("chrome"); // Chrome
    }

    private static void AddBrowserInfo<T>(string name) where T : IPlatformSupportCheck, ICookieSourceFactory
    {
        if (T.IsSupported)
        {
            s_info.Add(name, new BrowserInfo<T>(name));
        }
    }

    /// <summary>
    /// Loads cookies for a domain into the specified <see cref="CookieContainer"/>.
    /// </summary>
    /// <param name="cookieContainer">Container to populate.</param>
    /// <param name="domain">Target domain.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    /// <exception cref="ArgumentException">Thrown for invalid domain specification.</exception>
    public abstract Task LoadCookiesAsync(CookieContainer cookieContainer, CookieFilter domain, CancellationToken cancellationToken = default);

    /// <summary>
    /// Loads cookies for a domain into the specified <see cref="CookieContainer"/>.
    /// </summary>
    /// <param name="cookieContainer">Container to populate.</param>
    /// <param name="domains">Target domains.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    /// <exception cref="ArgumentException">Thrown for invalid domain specification.</exception>
    public abstract Task LoadCookiesAsync(CookieContainer cookieContainer, IReadOnlyCollection<CookieFilter> domains, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets supported browser names.
    /// </summary>
    /// <returns>Supported browser names.</returns>
    public static string[] GetSupportedBrowserNames()
    {
        return s_info.Keys.ToArray();
    }

    /// <summary>
    /// Attempts to get a <see cref="CookieSource"/> for the specified browser.
    /// </summary>
    /// <param name="browserName">Browser name.</param>
    /// <param name="cookieSource">Retrieved <see cref="CookieSource"/>.</param>
    /// <param name="profile">Optional browser profile name to initialize with.</param>
    /// <returns>True if <paramref name="cookieSource"/> was found.</returns>
    /// <remarks>
    /// This method only checks if the supplied browser name corresponds to a supported type.
    /// This method does not check if the browser is installed.
    /// </remarks>
    public static bool TryGetBrowserFromName(string browserName, [NotNullWhen(true)] out CookieSource? cookieSource, string? profile = null)
    {
        if (s_info.TryGetValue(browserName, out var info) && info.CreateCookieSource(profile) is { } source)
        {
            cookieSource = source;
            return true;
        }
        cookieSource = null;
        return false;
    }

    /// <summary>
    /// Loads cookies for a domain into the specified <see cref="CookieContainer"/>.
    /// </summary>
    /// <param name="cookieContainer">Container to populate.</param>
    /// <param name="domain">Target domain.</param>
    /// <param name="profile">Optional browser profile name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="browserName">Browser name.</param>
    /// <returns>Task.</returns>
    /// <exception cref="ArgumentException">Thrown for invalid domain specification.</exception>
    /// <exception cref="BrowserNotFoundException">Thrown for unknown browser name.</exception>
    /// <exception cref="BrowserProfileNotFoundException">Thrown for unknown browser profile name.</exception>
    public static Task LoadCookiesAsync(CookieContainer cookieContainer, CookieFilter domain, string browserName, string? profile = null, CancellationToken cancellationToken = default)
    {
        if (!TryGetBrowserFromName(browserName, out var cookieSource, profile))
        {
            throw new BrowserNotFoundException(browserName);
        }
        cookieSource = cookieSource.Resolve();
        return cookieSource.LoadCookiesAsync(cookieContainer, domain, cancellationToken);
    }

    /// <summary>
    /// Loads cookies for a domain into the specified <see cref="CookieContainer"/>.
    /// </summary>
    /// <param name="cookieContainer">Container to populate.</param>
    /// <param name="domains">Target domains.</param>
    /// <param name="profile">Optional browser profile name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="browserName">Browser name.</param>
    /// <returns>Task.</returns>
    /// <exception cref="ArgumentException">Thrown for invalid domain specification.</exception>
    /// <exception cref="BrowserNotFoundException">Thrown for unknown browser name.</exception>
    /// <exception cref="BrowserProfileNotFoundException">Thrown for unknown browser profile name.</exception>
    public static Task LoadCookiesAsync(CookieContainer cookieContainer, IReadOnlyCollection<CookieFilter> domains, string browserName, string? profile = null, CancellationToken cancellationToken = default)
    {
        if (!TryGetBrowserFromName(browserName, out var cookieSource, profile))
        {
            throw new BrowserNotFoundException(browserName);
        }
        cookieSource = cookieSource.Resolve();
        return cookieSource.LoadCookiesAsync(cookieContainer, domains, cancellationToken);
    }
}
