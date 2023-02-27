using System.Text.Json;
using Art.Common;

namespace Art.Tesler;

public class DirectoryJsonDefaultPropertyProvider : IDefaultPropertyProvider
{
    private readonly string _directory;
    private readonly IReadOnlyDictionary<string, JsonElement>? _commonProperties;

    public DirectoryJsonDefaultPropertyProvider(string directory, string? commonPropertyFile = null)
    {
        if (!Directory.Exists(directory))
        {
            throw new DirectoryNotFoundException($"Configuration directory \"{directory}\" does not exist");
        }
        _directory = directory;
        if (commonPropertyFile != null)
        {
            commonPropertyFile = Path.Combine(_directory, commonPropertyFile);
            if (!File.Exists(commonPropertyFile))
            {
                throw new FileNotFoundException($"Could not find common property file {commonPropertyFile}");
            }
            using var fs = File.OpenRead(commonPropertyFile);
            _commonProperties = JsonSerializer.Deserialize(fs, SourceGenerationContext.Default.IReadOnlyDictionaryStringJsonElement);
        }
    }

    public void WriteDefaultProperties(ArtifactToolID artifactToolId, Dictionary<string, JsonElement> dictionary)
    {
        string toolNameSafe = artifactToolId.GetToolString().SafeifyFileName();
        string toolFile = Path.Combine(_directory, $"{toolNameSafe}.json");
        if (_commonProperties != null)
        {
            foreach (var pair in _commonProperties)
            {
                dictionary[pair.Key] = pair.Value;
            }
        }
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
