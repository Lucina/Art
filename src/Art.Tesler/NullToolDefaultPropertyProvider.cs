using System.Text.Json;

namespace Art.Tesler;

public class NullDefaultPropertyProvider : IToolDefaultPropertyProvider
{
    public IEnumerable<KeyValuePair<string, JsonElement>> EnumerateDefaultProperties(ArtifactToolID artifactToolId)
    {
        return Array.Empty<KeyValuePair<string, JsonElement>>();
    }
}
