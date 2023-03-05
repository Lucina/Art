namespace Art.M3U;

internal class SegmentDownloadPlaylistElementProcessor : IPlaylistElementProcessor
{
    private readonly M3UDownloaderContext _context;

    public SegmentDownloadPlaylistElementProcessor(M3UDownloaderContext context)
    {
        _context = context;
    }

    public Task ProcessPlaylistElementAsync(Uri uri, M3UFile? file, long? mediaSequenceNumber = null, string? segmentName = null, CancellationToken cancellationToken = default)
    {
        _context.Tool.LogInformation($"Downloading segment {segmentName ?? "???"}...");
        return _context.DownloadSegmentAsync(uri, file, mediaSequenceNumber, cancellationToken);
    }
}
