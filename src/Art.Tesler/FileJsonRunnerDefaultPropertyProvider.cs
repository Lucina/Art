using System.Text.Json;

namespace Art.Tesler;

public class FileJsonRunnerDefaultPropertyProvider : IRunnerDefaultPropertyProvider
{
    private readonly string _propertyFile;

    public FileJsonRunnerDefaultPropertyProvider(string propertyFile)
    {
        _propertyFile = propertyFile;
    }

    public IEnumerable<KeyValuePair<string, JsonElement>> EnumerateDefaultProperties()
    {
        if (File.Exists(_propertyFile) && DirectoryJsonPropertyFileUtility.LoadPropertiesFromFile(_propertyFile) is { } map)
        {
            return map;
        }
        return Array.Empty<KeyValuePair<string, JsonElement>>();
    }

    public bool TryGetDefaultProperty(string key, out JsonElement value)
    {
        if (File.Exists(_propertyFile) && DirectoryJsonPropertyFileUtility.LoadPropertiesFromFile(_propertyFile) is { } map)
        {
            return map.TryGetValue(key, out value);
        }
        value = default;
        return false;
    }
}
