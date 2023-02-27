using System.Text.Json;
using Art.Common;

namespace Art.Tesler;

public class DirectoryDefaultPropertyProvider : IDefaultPropertyProvider
{
    private readonly string _directory;

    public DirectoryDefaultPropertyProvider(string directory)
    {
        if (!Directory.Exists(directory))
        {
            throw new DirectoryNotFoundException($"Configuration directory \"{directory}\" does not exist");
        }
        _directory = directory;
    }

    public void WriteDefaultProperties(ArtifactToolID artifactToolId, Dictionary<string, JsonElement> dictionary)
    {
        string toolNameSafe = artifactToolId.ToolString.SafeifyFileName();
        string toolFile = Path.Combine(_directory, $"{toolNameSafe}.json");
        if (File.Exists(toolFile))
        {
            using var file = File.OpenRead(toolFile);
            var dict = JsonSerializer.Deserialize(file, SourceGenerationContext.Default.IReadOnlyDictionaryStringJsonElement);
            if (dict != null)
            {
                foreach (var pair in dict)
                {
                    dictionary[pair.Key] = pair.Value;
                }
            }
        }
    }
}
