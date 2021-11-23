using System.Text.Json;

namespace Art;

/// <summary>
/// Provides artifact information.
/// </summary>
/// <param name="Key">Artifact key.</param>
/// <param name="Date">Artifact creation date.</param>
/// <param name="UpdateDate">Artifact update date.</param>
/// <param name="Properties">Artifact properties.</param>
/// <param name="Full">True if this is a full artifact.</param>
public record ArtifactInfo(ArtifactKey Key, DateTimeOffset? Date, DateTimeOffset? UpdateDate, IReadOnlyDictionary<string, JsonElement> Properties, bool Full)
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
    /// <param name="full">True if this is a full artifact.</param>
    /// <returns>Value.</returns>
    public static ArtifactInfo Create(string tool, string group, string id, DateTimeOffset? date = null, DateTimeOffset? updateDate = null, IReadOnlyDictionary<string, JsonElement>? properties = null, bool full = true)
        => new(new ArtifactKey(tool, group, id), date, updateDate, properties ?? EmptyProperties, full);

    /// <summary>
    /// Creates a new instance of <see cref="ArtifactInfo"/>.
    /// </summary>
    /// <param name="key">Artifact key.</param>
    /// <param name="date">Artifact creation date.</param>
    /// <param name="updateDate">Artifact update date.</param>
    /// <param name="properties">Artifact properties.</param>
    /// <param name="full">True if this is a full artifact.</param>
    /// <returns>Value.</returns>
    public static ArtifactInfo Create(ArtifactKey key, DateTimeOffset? date = null, DateTimeOffset? updateDate = null, IReadOnlyDictionary<string, JsonElement>? properties = null, bool full = true)
        => new(key, date, updateDate, properties ?? EmptyProperties, full);

    /// <summary>
    /// Singleton dummy no-entry properties.
    /// </summary>
    public static readonly IReadOnlyDictionary<string, JsonElement> EmptyProperties = new Dictionary<string, JsonElement>();
}
