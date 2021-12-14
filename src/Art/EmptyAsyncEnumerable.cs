namespace Art;

/// <summary>
/// Represents empty async enumerable.
/// </summary>
/// <typeparam name="T">Value type.</typeparam>
public class EmptyAsyncEnumerable<T> : IAsyncEnumerable<T>
{
    /// <summary>
    /// Singleton instance.
    /// </summary>
    public static readonly EmptyAsyncEnumerable<T> Singleton = new();

    /// <summary>
    /// Gets enumerator.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Enumerator.</returns>
    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default) => EmptyAsyncEnumerator<T>.Singleton;
}
