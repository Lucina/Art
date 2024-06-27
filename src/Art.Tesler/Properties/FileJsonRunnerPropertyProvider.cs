using System.Text.Json;

namespace Art.Tesler.Properties;

public class FileJsonRunnerPropertyProvider : IWritableRunnerPropertyProvider
{
    private readonly string _propertyFile;

    public FileJsonRunnerPropertyProvider(string propertyFile)
    {
        _propertyFile = propertyFile;
    }

    public IEnumerable<KeyValuePair<string, JsonElement>> GetProperties()
    {
        if (File.Exists(_propertyFile) && JsonPropertyFileUtility.LoadPropertiesFromFile(_propertyFile) is { } map)
        {
            return map;
        }
        return Array.Empty<KeyValuePair<string, JsonElement>>();
    }

    public bool TryGetProperty(string key, out JsonElement value)
    {
        if (File.Exists(_propertyFile) && JsonPropertyFileUtility.LoadPropertiesFromFile(_propertyFile) is { } map)
        {
            return map.TryGetValue(key, out value);
        }
        value = default;
        return false;
    }

    public bool TrySetProperty(string key, JsonElement value)
    {
        Dictionary<string, JsonElement>? map = null;
        if (File.Exists(_propertyFile))
        {
            map = JsonPropertyFileUtility.LoadPropertiesFromFileWritable(_propertyFile);
        }
        bool toCreate;
        if (map == null)
        {
            toCreate = true;
            map = new Dictionary<string, JsonElement>();
        }
        else
        {
            toCreate = false;
        }
        map[key] = value;
        if (toCreate)
        {
            ConfigDirectoryUtility.EnsureDirectoryForFileCreated(_propertyFile);
        }
        JsonPropertyFileUtility.StorePropertiesToFile(_propertyFile, map);
        return true;
    }
}
