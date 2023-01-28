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
    /// Checks if this browser appears to be a valid configuration.
    /// </summary>
    /// <exception cref="BrowserProfileNotFoundException">Thrown for unknown browser profile name.</exception>
    public abstract void Validate();

    private static readonly Dictionary<string, Func<string?, CookieSource?>> s_factories = new(StringComparer.InvariantCultureIgnoreCase)
    {
        { EdgeCookieSource.Name, p => new EdgeCookieSource(p ?? "Default") } // MS Edge
    };

    /// <summary>
    /// Loads cookies for a domain into the specified <see cref="CookieContainer"/>.
    /// </summary>
    /// <param name="cookieContainer">Container to populate.</param>
    /// <param name="domain">Target domain.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    /// <exception cref="ArgumentException">Thrown for invalid domain specification.</exception>
    public abstract Task LoadCookiesAsync(CookieContainer cookieContainer, string domain, CancellationToken cancellationToken = default);

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
        if (s_factories.TryGetValue(browserName, out var factory) && factory.Invoke(profile) is { } source)
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
    public static Task LoadCookiesAsync(CookieContainer cookieContainer, string domain, string browserName, string? profile = null, CancellationToken cancellationToken = default)
    {
        if (!TryGetBrowserFromName(browserName, out var cookieSource, profile))
        {
            throw new BrowserNotFoundException(browserName);
        }
        cookieSource.Validate();
        return cookieSource.LoadCookiesAsync(cookieContainer, domain, cancellationToken);
    }
}
