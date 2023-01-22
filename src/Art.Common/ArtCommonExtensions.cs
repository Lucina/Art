using Art.Common.Async;

namespace Art.Common;

/// <summary>
/// Provides utility functions.
/// </summary>
public static class ArtCommonExtensions
{
    /// <summary>
    /// Executes enumerable's enumerator independently of move-next calls.
    /// </summary>
    /// <param name="asyncEnumerable">Enumerable.</param>
    /// <typeparam name="T">Value type.</typeparam>
    /// <returns>Enumerable.</returns>
    public static IAsyncEnumerable<T> EagerAsync<T>(this IAsyncEnumerable<T> asyncEnumerable)
        => new EagerAsyncEnumerable<T>(asyncEnumerable);
}
