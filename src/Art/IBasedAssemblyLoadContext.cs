namespace Art;

/// <summary>
/// Assembly load context that has a base path.
/// </summary>
public interface IBasedAssemblyLoadContext
{
    /// <summary>
    /// Base path.
    /// </summary>
    string BasePath { get; }
}
