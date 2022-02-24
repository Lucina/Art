using System.Security.Cryptography;

namespace Art;

/// <summary>
/// Base handler for PKCS#5/PKCS#7 depadding.
/// </summary>
public abstract class PkcsDepaddingHandler : DepaddingHandler
{
    private readonly int _blockSize;
    private readonly byte[][] _blockCaches;
    private byte[] _blockCache;
    private int _currentWritten;
    private int _blockCacheIdx;
    private bool _didFinal;

    /// <summary>
    /// Initializes a new instance of <see cref="PkcsDepaddingHandler"/>.
    /// </summary>
    /// <param name="supportedBlockSize">Supported block sizes.</param>
    /// <param name="blockSize">Block size, in bytes.</param>
    /// <exception cref="ArgumentException">Thrown for invalid <see cref="blockSize"/> or illegally configured <paramref name="supportedBlockSize"/>.</exception>
    protected PkcsDepaddingHandler(KeySizes supportedBlockSize, int blockSize)
    {
        if (blockSize <= 0) throw new ArgumentException("Invalid block size", nameof(blockSize));
        if (!ValidateBlockSize(supportedBlockSize, blockSize)) throw new ArgumentException("Invalid block size", nameof(blockSize));
        _blockSize = blockSize;
        _blockCaches = new byte[2][];
        _blockCaches[0] = new byte[blockSize];
        _blockCaches[1] = new byte[blockSize];
        _blockCache = _blockCaches[0];
        _blockCacheIdx = 0;
        _currentWritten = 0;
    }

    /// <inheritdoc />
    public override bool TryUpdate(ArraySegment<byte> data, out ArraySegment<byte> a, out ArraySegment<byte> b)
    {
        if (_didFinal) throw new InvalidOperationException("Already performed final padding");
        if (data.Count == 0)
        {
            a = ArraySegment<byte>.Empty;
            b = ArraySegment<byte>.Empty;
            return false;
        }
        if (_currentWritten == _blockSize)
        {
            // existing buffer was already populated from previous run, reuse since we have at least 1 more byte available after this
            a = _blockCache;
            FlipBuffer();
            // current cache now empty
        }
        else if (_currentWritten != 0)
        {
            // Try to populate existing cache
            int rem = _blockSize - _currentWritten;
            int av = Math.Min(data.Count, rem);
            data[..av].AsSpan().CopyTo(_blockCache.AsSpan(_currentWritten, av));
            data = data[av..];
            _currentWritten += av;
            if (data.Count != 0)
            {
                // buffer remaining after this
                // block size matches
                a = _blockCache;
                FlipBuffer();
                // current cache now empty
            }
            else
            {
                // no buffer remaining after this
                // even if block size matches, don't do anything in case data stream didn't actually end here
                a = ArraySegment<byte>.Empty;
                b = ArraySegment<byte>.Empty;
                return false;
            }
        }
        else
        {
            // cache was empty, so leave empty for now
            a = ArraySegment<byte>.Empty;
            // current cache now empty
        }
        // Currently have empty cache
        int usedBytes = Math.Max((data.Count - 1) / _blockSize, 0) * _blockSize;
        b = data[..usedBytes];
        data = data[usedBytes..];
        // data.Length: [1, BlockSize]
        data.CopyTo(_blockCache);
        _currentWritten += data.Count;
        return true;
    }

    private static bool ValidatePkcsBlock(ReadOnlySpan<byte> buffer, out byte b)
    {
        if (buffer.Length == 0)
        {
            b = 0;
            return true;
        }
        b = buffer[^1];
        if (b > buffer.Length) return false;
        for (int i = buffer.Length - 1, c = 0; i >= 0 && c < b; i--, c++)
            if (buffer[i] != b)
                return false;
        return true;
    }

    /// <inheritdoc />
    public override void DoFinal(out ArraySegment<byte> buf)
    {
        if (_didFinal) throw new InvalidOperationException("Already performed final padding");
        // Should be the case that no data at all was written, so just ignore
        if (_currentWritten == 0)
        {
            buf = ArraySegment<byte>.Empty;
        }
        else
        {
            if (_currentWritten != _blockSize)
                throw new InvalidDataException("Cannot perform final padding: current state indicates non-block-aligned data");
            if (!ValidatePkcsBlock(_blockCache, out byte b)) throw new InvalidDataException("Failed to depad final block: invalid padding");
            buf = new ArraySegment<byte>(_blockCache, 0, _blockSize - b);
        }
        _didFinal = true;
    }

    private void FlipBuffer()
    {
        if (_currentWritten != _blockSize) throw new InvalidOperationException("Failed vibe check, current written != block size");
        _blockCache = _blockCaches[_blockCacheIdx ^= 1];
        _currentWritten = 0;
    }
}
