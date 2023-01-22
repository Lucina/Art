namespace Art.Common.Management;

/// <summary>
/// Represents a wrapper around a <see cref="Stream"/>.
/// </summary>
public abstract class CommittableWrappingStream : CommittableStream
{
    /// <summary>
    /// Base stream.
    /// </summary>
    protected Stream BaseStream;

    private bool _streamDisposed;

    /// <summary>
    /// Creates a new instance of <see cref="CommittableWrappingStream"/>.
    /// </summary>
    /// <seealso cref="FileStream(string,FileMode)"/>
    protected CommittableWrappingStream() : this(Null)
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="CommittableWrappingStream"/>.
    /// </summary>
    /// <param name="baseStream">Stream to wrap.</param>
    /// <seealso cref="FileStream(string,FileMode)"/>
    protected CommittableWrappingStream(Stream baseStream)
    {
        BaseStream = baseStream;
    }

    /// <inheritdoc />
    public override void Flush()
    {
        EnsureNotCommitted();
        BaseStream.Flush();
    }

    /// <inheritdoc />
    public override Task FlushAsync(CancellationToken cancellationToken)
    {
        EnsureNotCommitted();
        return BaseStream.FlushAsync(cancellationToken);
    }

    /// <inheritdoc />
    public override int ReadByte()
    {
        EnsureNotCommitted();
        return BaseStream.ReadByte();
    }

    /// <inheritdoc />
    public override int Read(byte[] buffer, int offset, int count)
    {
        EnsureNotCommitted();
        return BaseStream.Read(buffer, offset, count);
    }

    /// <inheritdoc />
    public override int Read(Span<byte> buffer)
    {
        EnsureNotCommitted();
        return BaseStream.Read(buffer);
    }

    /// <inheritdoc />
    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        EnsureNotCommitted();
        return BaseStream.ReadAsync(buffer, offset, count, cancellationToken);
    }

    /// <inheritdoc />
    public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        EnsureNotCommitted();
        return BaseStream.ReadAsync(buffer, cancellationToken);
    }

    /// <inheritdoc />
    public override long Seek(long offset, SeekOrigin origin)
    {
        EnsureNotCommitted();
        return BaseStream.Seek(offset, origin);
    }

    /// <inheritdoc />
    public override void SetLength(long value)
    {
        EnsureNotCommitted();
        BaseStream.SetLength(value);
    }

    /// <inheritdoc />
    public override void WriteByte(byte value)
    {
        EnsureNotCommitted();
        BaseStream.WriteByte(value);
    }

    /// <inheritdoc />
    public override void Write(byte[] buffer, int offset, int count)
    {
        EnsureNotCommitted();
        BaseStream.Write(buffer, offset, count);
    }

    /// <inheritdoc />
    public override void Write(ReadOnlySpan<byte> buffer)
    {
        EnsureNotCommitted();
        BaseStream.Write(buffer);
    }

    /// <inheritdoc />
    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        EnsureNotCommitted();
        return BaseStream.WriteAsync(buffer, offset, count, cancellationToken);
    }

    /// <inheritdoc />
    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        EnsureNotCommitted();
        return BaseStream.WriteAsync(buffer, cancellationToken);
    }

    /// <inheritdoc />
    public override bool CanRead => BaseStream.CanRead;

    /// <inheritdoc />
    public override bool CanSeek => BaseStream.CanSeek;

    /// <inheritdoc />
    public override bool CanWrite => BaseStream.CanWrite;

    /// <inheritdoc />
    public override long Length => BaseStream.Length;

    /// <inheritdoc />
    public override long Position
    {
        get => BaseStream.Position;
        set => BaseStream.Position = value;
    }

    /// <summary>
    /// Disposes the wrapped stream.
    /// </summary>
    protected void DisposeStream()
    {
        if (_streamDisposed) return;
        _streamDisposed = true;
        BaseStream.Dispose();
    }

    /// <summary>
    /// Disposes the wrapped stream.
    /// </summary>
    /// <returns>Task.</returns>
    protected ValueTask DisposeStreamAsync()
    {
        if (_streamDisposed) return ValueTask.CompletedTask;
        _streamDisposed = true;
        return BaseStream.DisposeAsync();
    }
}
