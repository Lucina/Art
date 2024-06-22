using System.Text.Json;

namespace Art.Tesler;

public static class DirectoryJsonPropertyFileUtility
{
    public static IReadOnlyDictionary<string, JsonElement>? LoadPropertiesFromFile(string path)
    {
        using var stream = File.OpenRead(path);
        return LoadProperties(stream);
    }

    public static IReadOnlyDictionary<string, JsonElement>? LoadProperties(Stream stream)
    {
        return JsonSerializer.Deserialize(stream, SourceGenerationContext.s_context.IReadOnlyDictionaryStringJsonElement);
    }
}