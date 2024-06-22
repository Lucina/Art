using System.Text.Json;

namespace Art.Tesler;

public interface IRunnerDefaultPropertyProvider
{
    /// <summary>
    /// Enumerates default properties. Pairs returned later override earlier values for the same key.
    /// </summary>
    /// <returns>Sequence of key-value pairs for configuration.</returns>
    IEnumerable<KeyValuePair<string, JsonElement>> EnumerateDefaultProperties();
}
