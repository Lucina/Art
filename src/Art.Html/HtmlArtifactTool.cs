using System.Text.Json;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Io;

namespace Art.Html;

/// <summary>
/// Represents an instance of an artifact tool that works on HTML content using a <see cref="IBrowsingContext"/>.
/// </summary>
public abstract class HtmlArtifactTool : HttpArtifactTool
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

    private IBrowsingContext _browser = null!;

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

    #region Configuration

    /// <inheritdoc/>
    public override async Task ConfigureAsync(ArtifactToolRuntimeConfig runtimeConfig)
    {
        await base.ConfigureAsync(runtimeConfig);
        IConfiguration configuration = Configuration.Default.WithOnly<ICookieProvider>(new OpenMemoryCookieProvider(HttpClientHandler.CookieContainer));
        _browser = BrowsingContext.New(configuration);
    }

    #endregion

    #region Main API

    /// <summary>
    /// Opens a new document loaded from the provided address.
    /// </summary>
    /// <param name="address">Address to load.</param>
    /// <returns>Task returning the loaded document.</returns>
    protected async Task<IDocument> OpenAsync(string address)
    {
        NotDisposed();
        return Document = await Browser.OpenAsync(address).ConfigureAwait(false);
    }

    /// <summary>
    /// Opens a new document loaded from the provided address.
    /// </summary>
    /// <param name="address">Address to load.</param>
    /// <returns>Task returning the loaded document.</returns>
    protected async Task<IDocument> OpenAsync(Url address)
    {
        NotDisposed();
        return Document = await Browser.OpenAsync(address).ConfigureAwait(false);
    }

    /// <summary>
    /// Opens a new document loaded from the provided request.
    /// </summary>
    /// <param name="request">Request to load.</param>
    /// <returns>Task returning the loaded document.</returns>
    protected async Task<IDocument> OpenAsync(DocumentRequest request)
    {
        NotDisposed();
        return Document = await Browser.OpenAsync(request).ConfigureAwait(false);
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

    #endregion

    #region Http overloads

    /// <summary>
    /// Sends an HTTP HEAD request.
    /// </summary>
    /// <param name="url">Request.</param>
    /// <returns>Task returning response.</returns>
    protected Task<HttpResponseMessage> HeadAsync(Url url)
        => HeadAsync(url.ToUri());

    /// <summary>
    /// Sends an HTTP GET request.
    /// </summary>
    /// <param name="url">Request.</param>
    /// <returns>Task returning response.</returns>
    protected Task<HttpResponseMessage> GetAsync(Url url)
        => GetAsync(url.ToUri());

    /// <summary>
    /// Retrieve deserialized JSON using a <see cref="Url"/>.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="url">Request URL.</param>
    /// <returns>Task returning deserialized data.</returns>
    /// <remarks>
    /// This overload usees <see cref="ArtifactTool.JsonOptions"/> member automatically.
    /// </remarks>
    protected Task<T> GetDeserializedJsonAsync<T>(Url url)
        => GetDeserializedJsonAsync<T>(url.ToUri());

    /// <summary>
    /// Retrieve deserialized JSON using a <see cref="Url"/> and <see cref="JsonSerializerOptions"/>.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="url">Request URL.</param>
    /// <param name="jsonSerializerOptions">Optional deserialization options.</param>
    /// <returns>Task returning deserialized data.</returns>
    protected Task<T> GetDeserializedJsonAsync<T>(Url url, JsonSerializerOptions? jsonSerializerOptions)
        => GetDeserializedJsonAsync<T>(url.ToUri(), jsonSerializerOptions);

    #endregion

    #region IDisposable

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (!_disposed)
        {
            if (disposing)
            {
                // dispose managed state (managed objects)
                _browser?.Dispose();
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
        if (_disposed) throw new ObjectDisposedException(nameof(HtmlArtifactTool));
    }

    #endregion
}
