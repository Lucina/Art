namespace Art.Common.IO;

/// <summary>
/// Base type for <see cref="Stream"/>s that wrap another.
/// </summary>
// https://github.com/dotnet/runtime/blob/71034dd2fbbd2304fac5c24d3a9f764a3c65f781/src/libraries/Common/src/System/IO/DelegatingStream.cs
public abstract class DelegatingStream : Stream
{
    private readonly Stream _innerStream;

    #region Properties

    /// <inheritdoc />
    public override bool CanRead
    {
        get { return _innerStream.CanRead; }
    }

    /// <inheritdoc />
    public override bool CanSeek
    {
        get { return _innerStream.CanSeek; }
    }

    /// <inheritdoc />
    public override bool CanWrite
    {
        get { return _innerStream.CanWrite; }
    }

    /// <inheritdoc />
    public override long Length
    {
        get { return _innerStream.Length; }
    }

    /// <inheritdoc />
    public override long Position
    {
        get { return _innerStream.Position; }
        set { _innerStream.Position = value; }
    }

    /// <inheritdoc />
    public override int ReadTimeout
    {
        get { return _innerStream.ReadTimeout; }
        set { _innerStream.ReadTimeout = value; }
    }

    /// <inheritdoc />
    public override bool CanTimeout
    {
        get { return _innerStream.CanTimeout; }
    }

    /// <inheritdoc />
    public override int WriteTimeout
    {
        get { return _innerStream.WriteTimeout; }
        set { _innerStream.WriteTimeout = value; }
    }

    #endregion Properties

    /// <summary>
    /// Initializes an instance of <see cref="DelegatingStream"/>.
    /// </summary>
    /// <param name="innerStream">Inner stream.</param>
    /// <exception cref="ArgumentNullException">Thrown for null <paramref name="innerStream"/>.</exception>
    protected DelegatingStream(Stream innerStream)
    {
        _innerStream = innerStream ?? throw new ArgumentNullException(nameof(innerStream));
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _innerStream.Dispose();
        }
        base.Dispose(disposing);
    }

    /// <inheritdoc />
    public override ValueTask DisposeAsync()
    {
        return _innerStream.DisposeAsync();
    }

    #region Read

    /// <inheritdoc />
    public override long Seek(long offset, SeekOrigin origin)
    {
        return _innerStream.Seek(offset, origin);
    }

    /// <inheritdoc />
    public override int Read(byte[] buffer, int offset, int count)
    {
        return _innerStream.Read(buffer, offset, count);
    }

    /// <inheritdoc />
    public override int Read(Span<byte> buffer)
    {
        return _innerStream.Read(buffer);
    }

    /// <inheritdoc />
    public override int ReadByte()
    {
        return _innerStream.ReadByte();
    }

    /// <inheritdoc />
    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        return _innerStream.ReadAsync(buffer, offset, count, cancellationToken);
    }

    /// <inheritdoc />
    public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        return _innerStream.ReadAsync(buffer, cancellationToken);
    }

    /// <inheritdoc />
    public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
    {
        return _innerStream.BeginRead(buffer, offset, count, callback, state);
    }

    /// <inheritdoc />
    public override int EndRead(IAsyncResult asyncResult)
    {
        return _innerStream.EndRead(asyncResult);
    }

    /// <inheritdoc />
    public override void CopyTo(Stream destination, int bufferSize)
    {
        _innerStream.CopyTo(destination, bufferSize);
    }

    /// <inheritdoc />
    public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
    {
        return _innerStream.CopyToAsync(destination, bufferSize, cancellationToken);
    }

    #endregion Read

    #region Write

    /// <inheritdoc />
    public override void Flush()
    {
        _innerStream.Flush();
    }

    /// <inheritdoc />
    public override Task FlushAsync(CancellationToken cancellationToken)
    {
        return _innerStream.FlushAsync(cancellationToken);
    }

    /// <inheritdoc />
    public override void SetLength(long value)
    {
        _innerStream.SetLength(value);
    }

    /// <inheritdoc />
    public override void Write(byte[] buffer, int offset, int count)
    {
        _innerStream.Write(buffer, offset, count);
    }

    /// <inheritdoc />
    public override void Write(ReadOnlySpan<byte> buffer)
    {
        _innerStream.Write(buffer);
    }

    /// <inheritdoc />
    public override void WriteByte(byte value)
    {
        _innerStream.WriteByte(value);
    }

    /// <inheritdoc />
    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        return _innerStream.WriteAsync(buffer, offset, count, cancellationToken);
    }

    /// <inheritdoc />
    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        return _innerStream.WriteAsync(buffer, cancellationToken);
    }

    /// <inheritdoc />
    public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
    {
        return _innerStream.BeginWrite(buffer, offset, count, callback, state);
    }

    /// <inheritdoc />
    public override void EndWrite(IAsyncResult asyncResult)
    {
        _innerStream.EndWrite(asyncResult);
    }

    #endregion Write
}
