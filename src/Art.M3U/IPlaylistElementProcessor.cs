namespace Art.M3U;

/// <summary>
/// Represents something that operates on playlist elements.
/// </summary>
public interface IPlaylistElementProcessor
{
    /// <summary>
    /// Processes a playlist element.
    /// </summary>
    /// <param name="uri">Full URI of segment.</param>
    /// <param name="file">M3U file this element is contained in.</param>
    /// <param name="mediaSequenceNumber">Media sequence number, if available.</param>
    /// <param name="segmentName">Segment name, if available.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    Task ProcessPlaylistElementAsync(Uri uri, M3UFile? file, long? mediaSequenceNumber = null, string? segmentName = null, CancellationToken cancellationToken = default);
}
