namespace Art.M3U;

/// <summary>
/// Represents a basic saver.
/// </summary>
public class M3UDownloaderContextStandardSaver : M3UDownloaderContextSaver
{
    private readonly bool _oneOff;
    private readonly TimeSpan _timeout;

    internal M3UDownloaderContextStandardSaver(M3UDownloaderContext context, bool oneOff, TimeSpan timeout) : base(context)
    {
        _oneOff = oneOff;
        _timeout = timeout;
    }

    /// <inheritdoc />
    public override Task RunAsync(CancellationToken cancellationToken = default)
    {
        return ProcessPlaylistAsync(_oneOff, _timeout, new SegmentDownloadPlaylistElementProcessor(Context), cancellationToken);
    }
}
