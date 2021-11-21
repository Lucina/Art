namespace Art;

internal static class DiskPaths
{
    internal static string GetBasePath(string baseDirectory, string tool, string group)
        => Path.Combine(baseDirectory, tool.SafeifyFileName(), group.SafeifyFileName());
}
