namespace Art.Common;

/// <summary>
/// Utility for IO operations.
/// </summary>
public static class ArtIOUtility
{
    /// <summary>
    /// Creates directories to contain a file if they don't already exist.
    /// </summary>
    /// <param name="file">Path to file for which to create containing directories.</param>
    /// <exception cref="IOException">Thrown for errors creating the directories.</exception>
    public static void EnsureDirectoryForFileCreated(string file)
    {
        if (Path.GetDirectoryName(Path.GetFullPath(file)) is { } parentPath)
        {
            EnsureDirectoryCreated(parentPath);
        }
    }

    /// <summary>
    /// Creates a directory if it doesn't already exist.
    /// </summary>
    /// <param name="directory">Path to directory to create.</param>
    /// <exception cref="IOException">Thrown for errors creating the directories.</exception>
    public static void EnsureDirectoryCreated(string directory)
    {
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

}