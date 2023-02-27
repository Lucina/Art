using System.Text.Json;

namespace Art.Tesler;

public class NullDefaultPropertyProvider : IDefaultPropertyProvider
{
    public void WriteDefaultProperties(ArtifactToolID artifactToolId, Dictionary<string, JsonElement> dictionary)
    {
    }
}
