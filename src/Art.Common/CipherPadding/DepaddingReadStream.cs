namespace Art.Common.CipherPadding;

/// <summary>
/// Represents a stream that uses a <see cref="DepaddingHandler"/> to read de-padded content from a source stream.
/// </summary>
public class DepaddingReadStream : Stream
{
    private readonly DepaddingHandler _handler;
    private readonly Stream _sourceStream;
    private readonly bool _keepOpen;
    private readonly object __lock = new();
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of <see cref="DepaddingReadStream"/>.
    /// </summary>
    /// <param name="handler">Depadding handler.</param>
    /// <param name="sourceStream">Source stream.</param>
    /// <param name="keepOpen">If true, keeps <paramref name="sourceStream"/> open after disposal.</param>
    public DepaddingReadStream(DepaddingHandler handler, Stream sourceStream, bool keepOpen = false)
    {
        _handler = handler;
        _sourceStream = sourceStream;
        _keepOpen = keepOpen;
    }

    /// <inheritdoc />
    public override void Flush() => _sourceStream.Flush();

    /// <inheritdoc />
    public override Task FlushAsync(CancellationToken cancellationToken) => _sourceStream.FlushAsync(cancellationToken);

    /// <inheritdoc />
    public override int Read(byte[] buffer, int offset, int count) => throw new NotImplementedException();

    /// <inheritdoc />
    public override int Read(Span<byte> buffer) => throw new NotImplementedException();

    /// <inheritdoc />
    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) => throw new NotImplementedException();

    /// <inheritdoc />
    public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    /// <inheritdoc />
    public override long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();

    /// <inheritdoc />
    public override void SetLength(long value) => throw new NotImplementedException();

    /// <inheritdoc />
    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

    /// <inheritdoc />
    public override void Write(ReadOnlySpan<byte> buffer) => throw new NotSupportedException();

    /// <inheritdoc />
    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) => throw new NotSupportedException();

    /// <inheritdoc />
    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default) => throw new NotSupportedException();

    private void DisposeCore()
    {
        if (!_keepOpen) _sourceStream.Dispose();
    }

    private async ValueTask DisposeCoreAsync()
    {
        if (!_keepOpen) await _sourceStream.DisposeAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        lock (__lock)
        {
            if (_disposed) return;
            _disposed = true;
        }
        DisposeCore();
    }

    /// <inheritdoc />
    public override ValueTask DisposeAsync()
    {
        lock (__lock)
        {
            if (_disposed) return ValueTask.CompletedTask;
            _disposed = true;
        }
        return DisposeCoreAsync();
    }

    /// <inheritdoc />
    public override bool CanRead => true;

    /// <inheritdoc />
    public override bool CanSeek => false;

    /// <inheritdoc />
    public override bool CanWrite => false;

    /// <inheritdoc />
    public override long Length => throw new NotSupportedException();

    /// <inheritdoc />
    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }
}
