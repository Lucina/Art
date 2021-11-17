namespace Art;

/// <summary>
/// Represents an object used to locate typed values from a string key.
/// </summary>
/// <typeparam name="T">Value type.</typeparam>
public interface IAsyncFinder<T>
{
    /// <summary>
    /// Finds a value.
    /// </summary>
    /// <param name="key">Key.</param>
    /// <returns>Task returning value if it exists or null.</returns>
    ValueTask<T?> FindAsync(string key);
}
