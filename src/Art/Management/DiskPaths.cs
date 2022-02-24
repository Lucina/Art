namespace Art.Management;

internal static class DiskPaths
{
    internal static string GetSubPath(string baseDirectory, string sub)
        => Path.Combine(baseDirectory, sub);

    internal static string GetSubPath(string baseDirectory, string sub, string tool)
        => Path.Combine(baseDirectory, sub, tool.SafeifyFileName());

    internal static string GetSubPath(string baseDirectory, string sub, string tool, string group)
        => Path.Combine(baseDirectory, sub, tool.SafeifyFileName(), group.SafeifyFileName());

    internal static string GetSubPath(string baseDirectory, string sub, string tool, string group, string id)
        => Path.Combine(baseDirectory, sub, tool.SafeifyFileName(), group.SafeifyFileName(), id.SafeifyFileName());

    internal static string GetBasePath(string baseDirectory, string tool, string group)
        => Path.Combine(baseDirectory, tool.SafeifyFileName(), group.SafeifyFileName());
}
