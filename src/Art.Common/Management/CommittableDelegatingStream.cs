namespace Art.Common.Management;

/// <summary>
/// Base type for <see cref="CommonCommittableStream"/>s that wrap another.
/// </summary>
// https://github.com/dotnet/runtime/blob/71034dd2fbbd2304fac5c24d3a9f764a3c65f781/src/libraries/Common/src/System/IO/DelegatingStream.cs
public abstract class CommittableDelegatingStream : CommonCommittableStream
{
    /// <summary>
    /// Wrapped stream.
    /// </summary>
    protected Stream InnerStream;

    #region Properties

    /// <inheritdoc />
    public override bool CanRead
    {
        get { return InnerStream.CanRead; }
    }

    /// <inheritdoc />
    public override bool CanSeek
    {
        get { return InnerStream.CanSeek; }
    }

    /// <inheritdoc />
    public override bool CanWrite
    {
        get { return InnerStream.CanWrite; }
    }

    /// <inheritdoc />
    public override long Length
    {
        get { return InnerStream.Length; }
    }

    /// <inheritdoc />
    public override long Position
    {
        get { return InnerStream.Position; }
        set { InnerStream.Position = value; }
    }

    /// <inheritdoc />
    public override int ReadTimeout
    {
        get { return InnerStream.ReadTimeout; }
        set { InnerStream.ReadTimeout = value; }
    }

    /// <inheritdoc />
    public override bool CanTimeout
    {
        get { return InnerStream.CanTimeout; }
    }

    /// <inheritdoc />
    public override int WriteTimeout
    {
        get { return InnerStream.WriteTimeout; }
        set { InnerStream.WriteTimeout = value; }
    }

    #endregion Properties

    /// <summary>
    /// Initializes an instance of <see cref="CommittableDelegatingStream"/> without a configured stream.
    /// </summary>
    protected CommittableDelegatingStream()
    {
        InnerStream = null!;
    }

    /// <summary>
    /// Initializes an instance of <see cref="CommittableDelegatingStream"/>.
    /// </summary>
    /// <param name="innerStream">Inner stream.</param>
    /// <exception cref="ArgumentNullException">Thrown for null <paramref name="innerStream"/>.</exception>
    protected CommittableDelegatingStream(Stream innerStream)
    {
        InnerStream = innerStream ?? throw new ArgumentNullException(nameof(innerStream));
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            InnerStream.Dispose();
        }
        base.Dispose(disposing);
    }

    /// <inheritdoc />
    public override async ValueTask DisposeAsync()
    {
        await InnerStream.DisposeAsync().ConfigureAwait(false);
        await base.DisposeAsync().ConfigureAwait(false);
    }

    #region Read

    /// <inheritdoc />
    public override long Seek(long offset, SeekOrigin origin)
    {
        EnsureNotCommitted();
        return InnerStream.Seek(offset, origin);
    }

    /// <inheritdoc />
    public override int Read(byte[] buffer, int offset, int count)
    {
        EnsureNotCommitted();
        return InnerStream.Read(buffer, offset, count);
    }

    /// <inheritdoc />
    public override int Read(Span<byte> buffer)
    {
        EnsureNotCommitted();
        return InnerStream.Read(buffer);
    }

    /// <inheritdoc />
    public override int ReadByte()
    {
        EnsureNotCommitted();
        return InnerStream.ReadByte();
    }

    /// <inheritdoc />
    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        EnsureNotCommitted();
        return InnerStream.ReadAsync(buffer, offset, count, cancellationToken);
    }

    /// <inheritdoc />
    public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        EnsureNotCommitted();
        return InnerStream.ReadAsync(buffer, cancellationToken);
    }

    /// <inheritdoc />
    public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
    {
        EnsureNotCommitted();
        return InnerStream.BeginRead(buffer, offset, count, callback, state);
    }

    /// <inheritdoc />
    public override int EndRead(IAsyncResult asyncResult)
    {
        EnsureNotCommitted();
        return InnerStream.EndRead(asyncResult);
    }

    /// <inheritdoc />
    public override void CopyTo(Stream destination, int bufferSize)
    {
        EnsureNotCommitted();
        InnerStream.CopyTo(destination, bufferSize);
    }

    /// <inheritdoc />
    public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
    {
        EnsureNotCommitted();
        return InnerStream.CopyToAsync(destination, bufferSize, cancellationToken);
    }

    #endregion Read

    #region Write

    /// <inheritdoc />
    public override void Flush()
    {
        EnsureNotCommitted();
        InnerStream.Flush();
    }

    /// <inheritdoc />
    public override Task FlushAsync(CancellationToken cancellationToken)
    {
        EnsureNotCommitted();
        return InnerStream.FlushAsync(cancellationToken);
    }

    /// <inheritdoc />
    public override void SetLength(long value)
    {
        EnsureNotCommitted();
        InnerStream.SetLength(value);
    }

    /// <inheritdoc />
    public override void Write(byte[] buffer, int offset, int count)
    {
        EnsureNotCommitted();
        InnerStream.Write(buffer, offset, count);
    }

    /// <inheritdoc />
    public override void Write(ReadOnlySpan<byte> buffer)
    {
        EnsureNotCommitted();
        InnerStream.Write(buffer);
    }

    /// <inheritdoc />
    public override void WriteByte(byte value)
    {
        EnsureNotCommitted();
        InnerStream.WriteByte(value);
    }

    /// <inheritdoc />
    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        EnsureNotCommitted();
        return InnerStream.WriteAsync(buffer, offset, count, cancellationToken);
    }

    /// <inheritdoc />
    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        EnsureNotCommitted();
        return InnerStream.WriteAsync(buffer, cancellationToken);
    }

    /// <inheritdoc />
    public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
    {
        EnsureNotCommitted();
        return InnerStream.BeginWrite(buffer, offset, count, callback, state);
    }

    /// <inheritdoc />
    public override void EndWrite(IAsyncResult asyncResult)
    {
        EnsureNotCommitted();
        InnerStream.EndWrite(asyncResult);
    }

    #endregion Write
}
