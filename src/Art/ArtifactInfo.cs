using System.Text.Json;

namespace Art;

/// <summary>
/// Provides artifact information.
/// </summary>
/// <param name="Id">Artifact ID.</param>
/// <param name="Date">Artifact creation date.</param>
/// <param name="UpdateDate">Artifact update date.</param>
/// <param name="Properties">Artifact properties.</param>
public record ArtifactInfo(string Id, DateTimeOffset? Date, DateTimeOffset? UpdateDate, IReadOnlyDictionary<string, string> Properties)
{
    /// <summary>
    /// Singleton dummy no-entry properties.
    /// </summary>
    public static readonly IReadOnlyDictionary<string, string> EmptyProperties = new Dictionary<string, string>();
}
