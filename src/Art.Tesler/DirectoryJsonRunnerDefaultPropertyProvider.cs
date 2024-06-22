using System.Text.Json;
using Art.Common;

namespace Art.Tesler;

public class DirectoryJsonRunnerDefaultPropertyProvider : IRunnerDefaultPropertyProvider
{
    private readonly string _propertyFile;

    public DirectoryJsonRunnerDefaultPropertyProvider(string propertyFile)
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
}
