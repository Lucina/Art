using System.Globalization;
using AngleSharp.Dom;

namespace Art.Html;

/// <summary>
/// HTML-specific extensions.
/// </summary>
public static class ArtHtmlExtensions
{
    /// <summary>
    /// Converts a <see cref="Url"/> to a <see cref="Uri"/>.
    /// </summary>
    /// <param name="url">Url to convert.</param>
    /// <returns>Uri.</returns>
    public static Uri ToUri(this Url url) => new UriBuilder()
    {
        Scheme = url.Scheme,
        Host = url.HostName,
        Port = int.Parse(url.Port, CultureInfo.InvariantCulture),
        UserName = url.UserName,
        Password = url.Password,
        Path = url.Path,
        Query = url.Query,
        Fragment = url.Fragment
    }.Uri;
}

