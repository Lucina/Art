using System.Text.Json;

namespace Art.Tesler.Properties;

public class FileJsonRunnerPropertyProvider : IRunnerPropertyProvider
{
    private readonly string _propertyFile;

    public FileJsonRunnerPropertyProvider(string propertyFile)
    {
        _propertyFile = propertyFile;
    }

    public IEnumerable<KeyValuePair<string, JsonElement>> GetProperties()
    {
        if (File.Exists(_propertyFile) && DirectoryJsonPropertyFileUtility.LoadPropertiesFromFile(_propertyFile) is { } map)
        {
            return map;
        }
        return Array.Empty<KeyValuePair<string, JsonElement>>();
    }

    public bool TryGetProperty(string key, out JsonElement value)
    {
        if (File.Exists(_propertyFile) && DirectoryJsonPropertyFileUtility.LoadPropertiesFromFile(_propertyFile) is { } map)
        {
            return map.TryGetValue(key, out value);
        }
        value = default;
        return false;
    }
}
