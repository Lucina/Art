using System.Text.Json;

namespace Art;

/// <summary>
/// Provides artifact information.
/// </summary>
/// <param name="Tool">Tool id.</param>
/// <param name="Group">Group.</param>
/// <param name="Id">Artifact ID.</param>
/// <param name="Date">Artifact creation date.</param>
/// <param name="UpdateDate">Artifact update date.</param>
/// <param name="Properties">Artifact properties.</param>
public record ArtifactInfo(string Tool, string Group, string Id, DateTimeOffset? Date, DateTimeOffset? UpdateDate, IReadOnlyDictionary<string, JsonElement> Properties)
{
    /// <summary>
    /// Creates a new instance of <see cref="ArtifactInfo"/>.
    /// </summary>
    /// <param name="tool">Tool id.</param>
    /// <param name="group">Group.</param>
    /// <param name="id">Artifact ID.</param>
    /// <param name="date">Artifact creation date.</param>
    /// <param name="updateDate">Artifact update date.</param>
    /// <param name="properties">Artifact properties.</param>
    /// <returns>Value.</returns>
    public static ArtifactInfo Create(string tool, string group, string id, DateTimeOffset? date = null, DateTimeOffset? updateDate = null, IReadOnlyDictionary<string, JsonElement>? properties = null)
        => new(tool, group, id, date, updateDate, properties ?? EmptyProperties);

    /// <summary>
    /// Singleton dummy no-entry properties.
    /// </summary>
    public static readonly IReadOnlyDictionary<string, JsonElement> EmptyProperties = new Dictionary<string, JsonElement>();
}
