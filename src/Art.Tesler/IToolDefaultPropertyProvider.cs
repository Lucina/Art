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
}
