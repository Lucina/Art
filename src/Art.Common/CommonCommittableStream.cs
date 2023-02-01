namespace Art.Common;

/// <summary>
/// Represents a stream that will be committed upon disposal if <see cref="CommittableStream.ShouldCommit"/> is set.
/// </summary>
public abstract class CommonCommittableStream : CommittableStream
{
    /// <summary>
    /// If true, this stream has been committed.
    /// </summary>
    protected bool Committed { get; private set; }


    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        try
        {
            CommitInternal(ShouldCommit);
        }
        finally
        {
            base.Dispose(disposing);
        }
    }

    /// <inheritdoc />
    public override async ValueTask DisposeAsync()
    {
        try
        {
            await CommitInternalAsync(ShouldCommit).ConfigureAwait(false);
        }
        finally
        {
            await base.DisposeAsync().ConfigureAwait(false);
        }
    }

    private void CommitInternal(bool shouldCommit)
    {
        if (Committed) return;
        Committed = true;
        Commit(shouldCommit);
    }

    private async ValueTask CommitInternalAsync(bool shouldCommit)
    {
        if (Committed) return;
        Committed = true;
        await CommitAsync(shouldCommit).ConfigureAwait(false);
    }

    /// <summary>
    /// Ensures this instance has not yet been committed.
    /// </summary>
    /// <exception cref="InvalidOperationException">Stream has been committed.</exception>
    protected void EnsureNotCommitted()
    {
        if (Committed) throw new InvalidOperationException("Stream has already been committed");
    }

    /// <summary>
    /// Perform data commit.
    /// </summary>
    /// <param name="shouldCommit">If true, perform commit. Otherwise, perform appropriate cleanup.</param>
    protected abstract void Commit(bool shouldCommit);

    /// <summary>
    /// Perform data commit.
    /// </summary>
    /// <param name="shouldCommit">If true, perform commit. Otherwise, perform appropriate cleanup.</param>
    protected virtual ValueTask CommitAsync(bool shouldCommit)
    {
        Commit(shouldCommit);
        return ValueTask.CompletedTask;
    }
}
