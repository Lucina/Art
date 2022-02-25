namespace Art.Async;

/// <summary>
/// Represents empty async enumerator.
/// </summary>
/// <typeparam name="T">Value type.</typeparam>
public sealed class EmptyAsyncEnumerator<T> : IAsyncEnumerator<T>
{
    /// <summary>
    /// Singleton instance.
    /// </summary>
    public static readonly EmptyAsyncEnumerator<T> Singleton = new();

    /// <summary>
    /// Current value.
    /// </summary>
    public T Current => default!;

    /// <summary>
    /// Disposes enumerator.
    /// </summary>
    /// <returns></returns>
    public ValueTask DisposeAsync() => default;

    /// <summary>
    /// Checks enumerator.
    /// </summary>
    /// <returns></returns>
    public ValueTask<bool> MoveNextAsync() => new(false);
}
