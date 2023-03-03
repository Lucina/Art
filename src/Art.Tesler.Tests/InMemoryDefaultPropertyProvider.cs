using System.Text.Json;

namespace Art.Tesler.Tests;

internal class InMemoryDefaultPropertyProvider : IDefaultPropertyProvider
{
    private readonly IReadOnlyDictionary<string, JsonElement> _shared;
    private readonly IReadOnlyDictionary<ArtifactToolID, IReadOnlyDictionary<string, JsonElement>> _perTool;

    public InMemoryDefaultPropertyProvider(IReadOnlyDictionary<string, JsonElement> shared, IReadOnlyDictionary<ArtifactToolID, IReadOnlyDictionary<string, JsonElement>> perTool)
    {
        _shared = shared;
        _perTool = perTool;
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
