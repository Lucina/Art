namespace Art.BrowserCookies;

/// <summary>
/// Represents a search filter for cookies.
/// </summary>
/// <param name="Domain">Primary domain.</param>
/// <param name="IncludeSubdomains">Include subdomains.</param>
public record struct CookieFilter(string Domain, bool IncludeSubdomains = true)
{
    /// <summary>
    /// Validates this instance.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown for invalid value.</exception>
    public void Validate()
    {
        if (Domain.StartsWith('.'))
        {
            throw new ArgumentException("Domain for cookie filter should not start with leading '.'");
        }
    }
}
