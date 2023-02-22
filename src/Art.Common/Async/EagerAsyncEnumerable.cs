namespace Art.Common.Async;

internal class EagerAsyncEnumerable<T> : IAsyncEnumerable<T>
{
    private readonly IAsyncEnumerable<T> _e;
    private readonly int _maxPreemptiveAccesses;

    public EagerAsyncEnumerable(IAsyncEnumerable<T> e, int maxPreemptiveAccesses = -1)
    {
        _e = e;
        _maxPreemptiveAccesses = maxPreemptiveAccesses;
    }

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return new EagerAsyncEnumerator<T>(_e.GetAsyncEnumerator(cancellationToken), _maxPreemptiveAccesses, cancellationToken);
    }
}
