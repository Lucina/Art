using System.Text.Json;

namespace Art.Tesler.Tests;

internal class InMemoryToolDefaultPropertyProvider : IToolDefaultPropertyProvider
{
    private readonly IReadOnlyDictionary<string, JsonElement> _shared;
    private readonly IReadOnlyDictionary<ArtifactToolID, IReadOnlyDictionary<string, JsonElement>> _perTool;

    public InMemoryToolDefaultPropertyProvider(IReadOnlyDictionary<string, JsonElement> shared, IReadOnlyDictionary<ArtifactToolID, IReadOnlyDictionary<string, JsonElement>> perTool)
    {
        _shared = shared;
        _perTool = perTool;
    }

    public IEnumerable<KeyValuePair<string, JsonElement>> EnumerateDefaultProperties(ArtifactToolID artifactToolId)
    {
        if (_perTool.TryGetValue(artifactToolId, out var dict))
        {
            return _shared.Concat(dict);
        }
        else
        {
            return _shared;
        }
    }

    public bool TryGetDefaultProperty(ArtifactToolID artifactToolId, string key, out JsonElement value)
    {
        if (_perTool.TryGetValue(artifactToolId, out var dict))
        {
            if (dict.TryGetValue(key, out value))
            {
                return true;
            }
        }
        return _shared.TryGetValue(key, out value);
    }
}
