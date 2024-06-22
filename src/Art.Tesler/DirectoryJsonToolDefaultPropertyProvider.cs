using System.Text.Json;
using Art.Common;

namespace Art.Tesler;

public class DirectoryJsonToolDefaultPropertyProvider : IToolDefaultPropertyProvider
{
    private readonly string _directory;
    private readonly Func<ArtifactToolID, string> _fileNameTransform;

    public DirectoryJsonToolDefaultPropertyProvider(string directory, Func<ArtifactToolID, string> fileNameTransform)
    {
        _directory = directory;
        _fileNameTransform = fileNameTransform;
    }

    public string GetPropertyFilePath(ArtifactToolID artifactToolId)
    {
        return Path.Combine(_directory, _fileNameTransform(artifactToolId));
    }

    public IEnumerable<KeyValuePair<string, JsonElement>> EnumerateDefaultProperties(ArtifactToolID artifactToolId)
    {
        string propertyFilePath = GetPropertyFilePath(artifactToolId);
        if (File.Exists(propertyFilePath) && DirectoryJsonPropertyFileUtility.LoadPropertiesFromFile(propertyFilePath) is { } map)
        {
            return map;
        }
        return Array.Empty<KeyValuePair<string, JsonElement>>();
    }

    public static string DefaultFileNameTransform(ArtifactToolID artifactToolId)
    {
        string toolNameSafe = artifactToolId.GetToolString().SafeifyFileName();
        return $"toolconfig-{toolNameSafe}.json";
    }
}
