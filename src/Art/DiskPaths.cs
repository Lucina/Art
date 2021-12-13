namespace Art;

internal static class DiskPaths
{
    internal static string GetBasePath(string baseDirectory, string tool, string group)
        => Path.Combine(baseDirectory, tool.SafeifyFileName(), group.SafeifyFileName());

    internal static string GetResourceDir(string toolGroupDirectory, string path)
    {
        string dir = toolGroupDirectory;
        if (!string.IsNullOrEmpty(path)) dir = Path.Combine(dir, path);
        return dir;
    }
}
