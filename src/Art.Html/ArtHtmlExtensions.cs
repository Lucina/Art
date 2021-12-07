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

    /// <summary>
    /// Returns the first element within the document
    /// (using depth-first pre-order traversal of the document's nodes)
    /// that matches the specified group of selectors.
    /// </summary>
    /// <param name="document">Source document.</param>
    /// <param name="selectors">The group of selectors to use.</param>
    /// <returns>The found element.</returns>
    /// <exception cref="IOException">Thrown if element is not found.</exception>
    public static IElement QuerySelectorRequired(this IDocument document, string selectors)
    {
        return document.QuerySelector(selectors) ?? throw new IOException($"Missing {selectors}");
    }

    /// <summary>
    /// Returns the first element within the element
    /// (using depth-first pre-order traversal of the document's nodes)
    /// that matches the specified group of selectors.
    /// </summary>
    /// <param name="element">Source element.</param>
    /// <param name="selectors">The group of selectors to use.</param>
    /// <returns>The found element.</returns>
    /// <exception cref="IOException">Thrown if element is not found.</exception>
    public static IElement QuerySelectorRequired(this IElement element, string selectors)
    {
        return element.QuerySelector(selectors) ?? throw new IOException($"Missing {selectors}");
    }

    /// <summary>
    /// Gets a required attribute from an element.
    /// </summary>
    /// <param name="element">Source element.</param>
    /// <param name="attribute">Attribute.</param>
    /// <returns>The found attribute.</returns>
    /// <exception cref="IOException">Thrown if attribute is not found.</exception>
    public static string GetAttributeRequired(this IElement element, string attribute)
    {
        return element.GetAttribute(attribute) ?? throw new IOException($"Missing {attribute}");
    }
}

