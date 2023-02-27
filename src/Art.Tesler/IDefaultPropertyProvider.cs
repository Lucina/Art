using System.Text.Json;

namespace Art.Tesler;

public interface IDefaultPropertyProvider
{
    void WriteDefaultProperties(ArtifactToolID artifactToolId, Dictionary<string, JsonElement> dictionary);
}
