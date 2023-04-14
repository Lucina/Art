namespace Art.M3U;

/// <summary>
/// Represents a basic saver.
/// </summary>
public class M3UDownloaderContextStandardSaver : M3UDownloaderContextSaver
{
    private readonly bool _oneOff;
    private readonly TimeSpan _timeout;
    private readonly IExtraSaverOperation? _extraOperation;
    private readonly Func<Uri, SegmentSettings>? _segmentFilter;

    internal M3UDownloaderContextStandardSaver(M3UDownloaderContext context, bool oneOff, TimeSpan timeout, Func<Uri, SegmentSettings>? segmentFilter, IExtraSaverOperation? extraOperation) : base(context)
    {
        _oneOff = oneOff;
        _timeout = timeout;
        _extraOperation = extraOperation;
        _segmentFilter = segmentFilter;
    }

    /// <inheritdoc />
    public override Task RunAsync(CancellationToken cancellationToken = default)
    {
        return ProcessPlaylistAsync(_oneOff, _timeout, new SegmentDownloadPlaylistElementProcessor(Context), _segmentFilter, _extraOperation, cancellationToken);
    }
}
