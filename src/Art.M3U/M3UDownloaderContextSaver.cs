using Art.Http;

namespace Art.M3U;

/// <summary>
/// Base class for <see cref="M3UDownloaderContext"/>-bound savers.
/// </summary>
public abstract class M3UDownloaderContextSaver : M3UDownloaderContextProcessor
{
    /// <summary>
    /// Initializes a new instance of <see cref="M3UDownloaderContextSaver"/>.
    /// </summary>
    /// <param name="context">Parent context.</param>
    protected M3UDownloaderContextSaver(M3UDownloaderContext context) : base(context)
    {
    }

    /// <summary>
    /// Runs implementation.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    /// <exception cref="ArtHttpResponseMessageException">Thrown on HTTP response indicating non-successful response.</exception>
    public abstract Task RunAsync(CancellationToken cancellationToken = default);
}
