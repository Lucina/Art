namespace Art.M3U;

internal class SegmentDownloadPlaylistElementProcessor : IPlaylistElementProcessor
{
    private readonly M3UDownloaderContext _context;

    public SegmentDownloadPlaylistElementProcessor(M3UDownloaderContext context)
    {
        _context = context;
    }

    public Task ProcessPlaylistElementAsync(Uri uri, M3UFile? file, long? mediaSequenceNumber = null, string? segmentName = null, ItemNo? itemNo = null, CancellationToken cancellationToken = default)
    {
        if (itemNo?.GetMessage() is { } itemNumberText)
        {
            _context.Tool.LogInformation($"Downloading segment {itemNumberText} {segmentName ?? "???"}");
        }
        else
        {
            _context.Tool.LogInformation($"Downloading segment {segmentName ?? "???"}");
        }
        return _context.DownloadSegmentAsync(uri, file, mediaSequenceNumber, cancellationToken);
    }
}
