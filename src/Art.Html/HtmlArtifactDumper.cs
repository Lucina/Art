using AngleSharp;
using AngleSharp.Dom;

namespace Art.Html;

/// <summary>
/// Represents an instance of an artifact dumper that works on HTML content using a <see cref="IBrowsingContext"/>.
/// </summary>
public abstract class HtmlArtifactDumper : HttpArtifactDumper
{
    /// <summary>
    /// Active browsing context.
    /// </summary>
    protected IBrowsingContext Browser
    {
        get
        {
            NotDisposed();
            return _browser;
        }
        set
        {
            NotDisposed();
            _browser = value;
        }
    }

    private IBrowsingContext _browser;

    /// <summary>
    /// Current document, if one is loaded.
    /// </summary>
    protected IDocument? Document;

    /// <summary>
    /// Current document.
    /// </summary>
    /// <remarks>
    /// Accessing this property throws an <see cref="InvalidOperationException"/> if a document is not loaded.
    /// </remarks>
    protected IDocument DocumentNotNull => Document ?? throw new InvalidOperationException("Document not currently loaded");

    private bool _disposed;

    /// <summary>
    /// Creates a new instance of <see cref="HtmlArtifactDumper"/>, with an existing <see cref="System.Net.Http.HttpClient"/> (no automatic configuration).
    /// </summary>
    /// <param name="registrationManager">Registration manager to use for this instance.</param>
    /// <param name="dataManager">Data manager to use for this instance.</param>
    /// <param name="artifactDumpingProfile">Origin dumping profile.</param>
    /// <param name="httpClient">Optional existing http client to use.</param>
    /// <param name="configuration">Optional browsing context configuration.</param>
    /// <remarks>
    /// No configuration will be performed on the <see cref="System.Net.Http.HttpClient"/> if provided. However, derived constructors can access the <see cref="HttpClient"/> member for configuration.
    /// </remarks>

    protected HtmlArtifactDumper(ArtifactRegistrationManager registrationManager, ArtifactDataManager dataManager, ArtifactDumpingProfile artifactDumpingProfile, HttpClient? httpClient = null, IConfiguration? configuration = null)
        : base(registrationManager, dataManager, artifactDumpingProfile, httpClient)
    {
        _browser = BrowsingContext.New(configuration);
    }

    /// <summary>
    /// Opens a new document loaded from the provided address.
    /// </summary>
    /// <param name="address">Address to load.</param>
    /// <returns>Task returning the loaded document.</returns>
    protected async ValueTask<IDocument> OpenAsync(string address)
    {
        NotDisposed();
        return Document = await Browser.OpenAsync(address).ConfigureAwait(false);
    }

    /// <summary>
    /// Opens a new document loaded from the provided address.
    /// </summary>
    /// <param name="address">Address to load.</param>
    /// <returns>Task returning the loaded document.</returns>
    protected async ValueTask<IDocument> OpenAsync(AngleSharp.Dom.Url address)
    {
        NotDisposed();
        return Document = await Browser.OpenAsync(address).ConfigureAwait(false);
    }

    /// <summary>
    /// Returns the first element within the document
    /// (using depth-first pre-order traversal of the document's nodes)
    /// that matches the specified group of selectors.
    /// </summary>
    /// <param name="selectors">The group of selectors to use.</param>
    /// <returns>The found element.</returns>
    protected IElement? QuerySelector(string selectors)
    {
        NotDisposed();
        return DocumentNotNull.QuerySelector(selectors);
    }

    /// <summary>
    /// Returns the first element within the document
    /// (using depth-first pre-order traversal of the document's nodes)
    /// that matches the specified group of selectors.
    /// </summary>
    /// <param name="selectors">The group of selectors to use.</param>
    /// <returns>The found element.</returns>
    protected T? QuerySelector<T>(string selectors) where T : class, IElement
    {
        NotDisposed();
        return DocumentNotNull.QuerySelector<T>(selectors);
    }

    /// <summary>
    /// Returns a list of the elements within the document
    /// (using depth-first pre-order traversal of the document's nodes)
    /// that match the specified group of selectors.
    /// </summary>
    /// <param name="selectors">The group of selectors to use.</param>
    /// <returns>The found elements.</returns>
    protected IHtmlCollection<IElement> QuerySelectorAll(string selectors)
    {
        NotDisposed();
        return DocumentNotNull.QuerySelectorAll(selectors);
    }

    /// <summary>
    /// Returns a list of the elements within the document
    /// (using depth-first pre-order traversal of the document's nodes)
    /// that match the specified group of selectors.
    /// </summary>
    /// <param name="selectors">The group of selectors to use.</param>
    /// <returns>The found elements.</returns>
    protected IEnumerable<T> QuerySelectorAll<T>(string selectors) where T : class, IElement
    {
        NotDisposed();
        return DocumentNotNull.QuerySelectorAll<T>(selectors);
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (!_disposed)
        {
            if (disposing)
            {
                // dispose managed state (managed objects)
                _browser.Dispose();
                Document?.Dispose();
            }

            // free unmanaged resources (unmanaged objects) and override finalizer
            // set large fields to null
            _browser = null!;
            Document = null;
            _disposed = true;
        }
    }

    private void NotDisposed()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(HtmlArtifactDumper));
    }
}
