using System.Text.Json;

namespace Art.Tesler.Properties;

public interface IWritableToolPropertyProvider : IToolPropertyProvider
{
    /// <summary>
    /// Attempts to set property for the specified key.
    /// </summary>
    /// <param name="artifactToolId">ID of tool to get properties for.</param>
    /// <param name="key">Property key.</param>
    /// <param name="value">Property value.</param>
    /// <returns>True if successful.</returns>
    public bool TrySetProperty(ArtifactToolID artifactToolId, string key, JsonElement value);
}
