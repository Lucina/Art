namespace Art.BrowserCookies.Util;

internal static class PathUtil
{
    /// <summary>
    /// Gets known Linux user config directory.
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// Should follow XDG Base Directory Specification.
    /// </remarks>
    /// <exception cref="DirectoryNotFoundException">Thrown if configured directory does not exist.</exception>
    public static string GetXdgConfigHomeOrFallback()
    {
        // ref: https://specifications.freedesktop.org/basedir-spec/basedir-spec-latest.html
        // ref: https://github.com/dotnet/runtime/issues/69449
        string dir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        if (string.IsNullOrWhiteSpace(dir))
        {
            throw new DirectoryNotFoundException("Could not find existing directory corresponding to XDG config home dir");
        }
        return dir;
    }
}
