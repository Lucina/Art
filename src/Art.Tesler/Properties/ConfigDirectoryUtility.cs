namespace Art.Tesler.Properties;

public static class ConfigDirectoryUtility
{
    public static void EnsureDirectoryForFileCreated(string file)
    {
        if (Path.GetDirectoryName(Path.GetFullPath(file)) is { } parentPath)
        {
            EnsureDirectoryCreated(parentPath);
        }
    }

    public static void EnsureDirectoryCreated(string directory)
    {
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }
}
