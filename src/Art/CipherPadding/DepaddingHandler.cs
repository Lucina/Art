using System.Buffers;
using System.Security.Cryptography;

namespace Art.CipherPadding;

/// <summary>
/// Provides streaming depadding support.
/// </summary>
public abstract class DepaddingHandler
{
    /// <summary>
    /// Validates a block size against allowed block sizes.
    /// </summary>
    /// <param name="keySizes">Block size set to check against.</param>
    /// <param name="blockSize">Block size.</param>
    /// <returns>True if block size is allowed.</returns>
    /// <exception cref="ArgumentException">Thrown for any illegally configured <paramref name="keySizes"/>.</exception>
    protected static bool ValidateBlockSize(KeySizes keySizes, int blockSize)
    {
        if (keySizes.MinSize <= 0) throw new ArgumentException("Min block size must be positive", nameof(keySizes));
        if (keySizes.MaxSize <= 0) throw new ArgumentException("Max block size must be positive", nameof(keySizes));
        if (keySizes.SkipSize < 0) throw new ArgumentException("Block size skip cannot be negative", nameof(keySizes));
        if (keySizes.MaxSize < keySizes.MinSize) throw new ArgumentException("Invalid block size range", nameof(keySizes));
        if (keySizes.SkipSize != 0) return blockSize >= keySizes.MinSize && blockSize <= keySizes.MaxSize && (blockSize - keySizes.MinSize) % keySizes.SkipSize == 0;
        if (keySizes.MinSize != keySizes.MaxSize) throw new ArgumentException("Block size skip cannot be 0 for non-matching min and max size", nameof(keySizes));
        return blockSize == keySizes.MinSize;
    }

    /// <summary>
    /// Updates padding state and returns buffers if any data is available.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="a">First buffer, must be used first.</param>
    /// <param name="b">Second buffer (sub-span of <paramref name="data"/>), must be used second.</param>
    /// <returns>True if any data is to be written.</returns>
    public abstract bool TryUpdate(ReadOnlyMemory<byte> data, out ReadOnlyMemory<byte> a, out ReadOnlyMemory<byte> b);

    /// <summary>
    /// Updates padding state and returns buffers if any data is available.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="a">First buffer, must be used first.</param>
    /// <param name="b">Second buffer (sub-span of <paramref name="data"/>), must be used second.</param>
    /// <returns>True if any data is to be written.</returns>
    public abstract bool TryUpdate(ReadOnlySpan<byte> data, out ReadOnlySpan<byte> a, out ReadOnlySpan<byte> b);

    /// <summary>
    /// Updates padding state with final block.
    /// </summary>
    /// <param name="buf">Buffer.</param>
    public abstract void DoFinal(out ReadOnlyMemory<byte> buf);

    /// <summary>
    /// Copies <paramref name="from"/> to <paramref name="to"/> with depadding operation.
    /// </summary>
    /// <param name="from">Source stream.</param>
    /// <param name="to">Target stream.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if successful.</returns>
    /// <exception cref="ArgumentNullException">Thrown for null <paramref cref="from"/> or <paramref cref="to"/>.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="from"/> is not readable or <paramref name="to"/> is not writeable.</exception>
    public Task CopyDepaddedAsync(Stream from, Stream to, CancellationToken cancellationToken = default)
    {
        if (from == null) throw new ArgumentNullException(nameof(from));
        if (to == null) throw new ArgumentNullException(nameof(to));
        if (!from.CanRead) throw new ArgumentException("Stream must be readable", nameof(from));
        if (!to.CanWrite) throw new ArgumentException("Stream must be writeable", nameof(from));
        return CopyDepaddedCoreAsync(from, to, cancellationToken);
    }

    private async Task CopyDepaddedCoreAsync(Stream from, Stream to, CancellationToken cancellationToken)
    {
        byte[] tmp = ArrayPool<byte>.Shared.Rent(16 * 1024);
        try
        {
            while (true)
            {
                int read = await from.ReadAsync(tmp, cancellationToken);
                if (read == 0)
                {
                    DoFinal(out var final);
                    if (final.Length != 0) await to.WriteAsync(final, cancellationToken);
                    return;
                }
                else
                {
                    if (TryUpdate(new ReadOnlyMemory<byte>(tmp, 0, read), out var a, out var b))
                    {
                        if (a.Length != 0) await to.WriteAsync(a, cancellationToken);
                        if (b.Length != 0) await to.WriteAsync(b, cancellationToken);
                    }
                }
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(tmp);
        }
    }
}
