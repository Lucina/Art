using System.Collections.Concurrent;

namespace Art.Common.Async;

internal class EagerAsyncEnumerator<T> : IAsyncEnumerator<T>
{
    private readonly IAsyncEnumerator<T> _e;
    private readonly CancellationToken _cancellationToken;
    private readonly ConcurrentQueue<JobState> _qt;
    private readonly ConcurrentQueue<T> _queue;
    private readonly int _maxPreemptiveAccesses;
    private volatile bool _over;

    public EagerAsyncEnumerator(IAsyncEnumerator<T> e, int maxPreemptiveAccesses = -1, CancellationToken cancellationToken = default)
    {
        if (maxPreemptiveAccesses is not (-1 or > 0))
        {
            throw new ArgumentOutOfRangeException(nameof(maxPreemptiveAccesses));
        }
        _e = e;
        _cancellationToken = cancellationToken;
        _qt = new ConcurrentQueue<JobState>();
        _queue = new ConcurrentQueue<T>();
        _maxPreemptiveAccesses = maxPreemptiveAccesses;
        Current = default!;
        ScheduleNext();
    }

    private class JobState
    {
        public volatile bool StoppedByAccessLimit;
        public Task<bool> Task = null!;
    }

    private void ScheduleNext()
    {
        var state = new JobState();
        state.Task = Task.Run(NextInternal(state), _cancellationToken);
        _qt.Enqueue(state);
    }

    private Func<Task<bool>> NextInternal(JobState jobState) => async () =>
    {
        bool advanced = await _e.MoveNextAsync().ConfigureAwait(false);
        _cancellationToken.ThrowIfCancellationRequested();
        if (!advanced)
        {
            jobState.StoppedByAccessLimit = false;
            return advanced;
        }
        _queue.Enqueue(_e.Current);

        if (_maxPreemptiveAccesses != -1 && _qt.Count >= _maxPreemptiveAccesses)
        {
            jobState.StoppedByAccessLimit = true;
        }
        else
        {
            jobState.StoppedByAccessLimit = false;
            ScheduleNext();
        }
        return advanced;
    };

    public ValueTask DisposeAsync() => _e.DisposeAsync();

    public async ValueTask<bool> MoveNextAsync()
    {
        if (_over) return false;
        if (!_qt.TryDequeue(out JobState? state)) throw new InvalidOperationException($"State violation ({nameof(_qt)}");
        var task = state.Task;
        await task.ConfigureAwait(false);
        if (task.Exception != null) throw task.Exception;
        bool advanced = task.Result;
        if (advanced)
        {
            if (!_queue.TryDequeue(out T? value)) throw new InvalidOperationException($"State violation ({nameof(_queue)})");
            Current = value;
            if (state.StoppedByAccessLimit)
            {
                ScheduleNext();
            }
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
