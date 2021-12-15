using System.Collections.Concurrent;

namespace Art;

internal class EagerAsyncEnumerator<T> : IAsyncEnumerator<T>
{
    private readonly IAsyncEnumerator<T> _e;
    private readonly CancellationToken _cancellationToken;
    private readonly ConcurrentQueue<Task<bool>> _qt;
    private readonly ConcurrentQueue<T> _queue;
    private bool _over;

    public EagerAsyncEnumerator(IAsyncEnumerator<T> e, CancellationToken cancellationToken = default)
    {
        _e = e;
        _cancellationToken = cancellationToken;
        _qt = new ConcurrentQueue<Task<bool>>();
        _queue = new ConcurrentQueue<T>();
        Current = default!;
        ScheduleNext(NextInternal);
    }

    private void ScheduleNext(Func<Task<bool>> task)
    {
        _qt.Enqueue(Task.Run(task, _cancellationToken));
    }

    private async Task<bool> NextInternal()
    {
        bool advanced = await _e.MoveNextAsync().ConfigureAwait(false);
        _cancellationToken.ThrowIfCancellationRequested();
        if (advanced) _queue.Enqueue(_e.Current);
        if (advanced) ScheduleNext(NextInternal);
        return advanced;
    }

    public ValueTask DisposeAsync() => _e.DisposeAsync();

    public async ValueTask<bool> MoveNextAsync()
    {
        if (_over) return false;
        if (!_qt.TryDequeue(out Task<bool>? task)) throw new InvalidOperationException($"State violation ({nameof(_qt)}");
        bool advanced = await task.ConfigureAwait(false);
        if (advanced)
        {
            if (!_queue.TryDequeue(out T? value)) throw new InvalidOperationException($"State violation ({nameof(_queue)})");
            Current = value;
        }
        else
        {
            _over = true;
            Current = default!;
        }
        return advanced;
    }

    public T Current { get; private set; }
}
