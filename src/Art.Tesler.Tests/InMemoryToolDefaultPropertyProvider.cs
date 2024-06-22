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

    public void WriteDefaultProperties(ArtifactToolID artifactToolId, Dictionary<string, JsonElement> dictionary)
    {
        foreach (var sharedPair in _shared)
        {
            dictionary[sharedPair.Key] = sharedPair.Value;
        }
        if (_perTool.TryGetValue(artifactToolId, out var dict))
        {
            foreach (var toolPair in dict)
            {
                dictionary[toolPair.Key] = toolPair.Value;
            }
        }
    }
}
