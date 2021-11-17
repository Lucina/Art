using System.Text.Json;

namespace Art;

/// <summary>
/// Provides artifact information.
/// </summary>
/// <param name="Id">Artifact ID.</param>
/// <param name="Date">Artifact creation date.</param>
/// <param name="UpdateDate">Artifact update date.</param>
/// <param name="Properties">Artifact properties.</param>
public record ArtifactInfo(string Id, DateTimeOffset? Date, DateTimeOffset? UpdateDate, IReadOnlyDictionary<string, JsonElement> Properties)
{
    /// <summary>
    /// Creates a new instance of <see cref="ArtifactInfo"/>.
    /// </summary>
    /// <param name="Id">Artifact ID.</param>
    /// <param name="Date">Artifact creation date.</param>
    /// <param name="UpdateDate">Artifact update date.</param>
    public ArtifactInfo(string Id, DateTimeOffset? Date = null, DateTimeOffset? UpdateDate = null) : this(Id, Date, UpdateDate, EmptyProperties)
    {
    }

    /// <summary>
    /// Singleton dummy no-entry properties.
    /// </summary>
    public static readonly IReadOnlyDictionary<string, JsonElement> EmptyProperties = new Dictionary<string, JsonElement>();
}
