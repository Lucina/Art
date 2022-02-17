namespace Art;

/// <summary>
/// Represents a stream that will be committed upon disposal if <see cref="ShouldCommit"/> is set.
/// </summary>
public abstract class CommittableStream : Stream
{
    /// <summary>
    /// If true, commit this stream upon disposal.
    /// </summary>
    public bool ShouldCommit { get; set; }

    /// <summary>
    /// If true, this stream has been committed.
    /// </summary>
    protected bool Committed { get; private set; }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        try
        {
            if (ShouldCommit) CommitInternal();
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
            if (ShouldCommit) await CommitInternalAsync();
        }
        finally
        {
            await base.DisposeAsync();
        }
    }

    private void CommitInternal()
    {
        if (Committed) return;
        Committed = true;
        Commit();
    }

    private async ValueTask CommitInternalAsync()
    {
        if (Committed) return;
        Committed = true;
        await CommitAsync();
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
    protected abstract void Commit();

    /// <summary>
    /// Perform data commit.
    /// </summary>
    protected virtual ValueTask CommitAsync()
    {
        Commit();
        return ValueTask.CompletedTask;
    }
}
