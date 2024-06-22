using System.Text.Json;

namespace Art.Tesler;

public class NullRunnerDefaultPropertyProvider : IRunnerDefaultPropertyProvider
{
    public IEnumerable<KeyValuePair<string, JsonElement>> EnumerateDefaultProperties()
    {
        return Array.Empty<KeyValuePair<string, JsonElement>>();
    }
}
