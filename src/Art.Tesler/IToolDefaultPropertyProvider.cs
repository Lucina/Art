using System.Text.Json;

namespace Art.Tesler;

public interface IToolDefaultPropertyProvider
{
    /// <summary>
    /// Enumerates default properties. Pairs returned later override earlier values for the same key.
    /// </summary>
    /// <param name="artifactToolId">ID of tool to get properties for.</param>
    /// <returns>Sequence of key-value pairs for configuration.</returns>
    IEnumerable<KeyValuePair<string, JsonElement>> EnumerateDefaultProperties(ArtifactToolID artifactToolId);

    /// <summary>
    /// Attempts to get default property with the specified key.
    /// </summary>
    /// <param name="artifactToolId">ID of tool to get properties for.</param>
    /// <param name="key">Property key.</param>
    /// <param name="value">Resolved property key if successful.</param>
    /// <returns>True if successful.</returns>
    public bool TryGetDefaultProperty(ArtifactToolID artifactToolId, string key, out JsonElement value);
}
