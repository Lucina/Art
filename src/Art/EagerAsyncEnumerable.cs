namespace Art;

internal class EagerAsyncEnumerable<T> : IAsyncEnumerable<T>
{
    private readonly IAsyncEnumerable<T> _e;

    public EagerAsyncEnumerable(IAsyncEnumerable<T> e) => _e = e;

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        => new EagerAsyncEnumerator<T>(_e.GetAsyncEnumerator(cancellationToken), cancellationToken);
}
