using System.Net;
using System.Net.WebSockets;

namespace Art.Web;

/// <summary>
/// Represents an instance of an artifact tool that depends on an <see cref="System.Net.Http.HttpClient"/>.
/// </summary>
public abstract partial class HttpArtifactTool : ArtifactTool
{
    #region Fields

    /// <summary>
    /// Option used to specify path to http client cookie file.
    /// </summary>
    public const string OptCookieFile = "cookieFile";

    /// <summary>
    /// HTTP cookie container used by this instance.
    /// </summary>
    public CookieContainer CookieContainer
    {
        get
        {
            NotDisposed();
            return _cookieContainer;
        }
        set
        {
            NotDisposed();
            _cookieContainer = value;
        }
    }

    /// <summary>
    /// HTTP client used by this instance.
    /// </summary>
    public HttpClient HttpClient
    {
        get
        {
            NotDisposed();
            return _httpClient;
        }
        set
        {
            NotDisposed();
            _httpClient = value;
        }
    }

    /// <summary>
    /// Http message handler for <see cref="HttpClient"/>.
    /// </summary>
    public HttpMessageHandler HttpMessageHandler
    {
        get
        {
            NotDisposed();
            return _httpMessageHandler;
        }
        set
        {
            NotDisposed();
            _httpMessageHandler = value;
        }
    }

    /// <summary>
    /// Dummy default user agent string.
    /// </summary>
    public const string DefaultUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.85 Safari/537.36 Edg/90.0.818.46";

    #endregion

    #region Private fields

    private CookieContainer _cookieContainer = null!;

    private HttpClient _httpClient = null!;

    private HttpMessageHandler _httpMessageHandler = null!;

    private bool _disposed;

    #endregion

    #region Configuration

    /// <inheritdoc/>
    public override Task ConfigureAsync(CancellationToken cancellationToken = default)
    {
        _cookieContainer = CreateCookieContainer();
        _httpMessageHandler = CreateHttpMessageHandler();
        _httpClient = CreateHttpClient(_httpMessageHandler);
        _httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd(DefaultUserAgent);
        return Task.CompletedTask;
    }

    #endregion

    #region Http client configuration

    /// <summary>
    /// Creates a cookie container (by default using the <see cref="OptCookieFile"/> configuration option).
    /// </summary>
    /// <returns>A cookie container.</returns>
    public virtual CookieContainer CreateCookieContainer()
    {
        CookieContainer cookies = new();
        if (TryGetOption(OptCookieFile, out string? cookieFile))
            using (StreamReader f = File.OpenText(cookieFile))
                cookies.LoadCookieFile(f);
        return cookies;
    }

    /// <summary>
    /// Creates an <see cref="HttpMessageHandler"/> instance.
    /// </summary>
    /// <returns>Cookie container.</returns>
    public virtual HttpMessageHandler CreateHttpMessageHandler() => new HttpClientHandler { AutomaticDecompression = DecompressionMethods.All, CookieContainer = _cookieContainer };

    /// <summary>
    /// Creates an <see cref="System.Net.Http.HttpClient"/> instance configured to use the specified message handler.
    /// </summary>
    /// <param name="httpMessageHandler">HTTP message handler.</param>
    /// <returns>Configured HTTP client.</returns>
    public virtual HttpClient CreateHttpClient(HttpMessageHandler httpMessageHandler) => new(httpMessageHandler);

    /// <summary>
    /// Creates a <see cref="ClientWebSocket"/> using this instance's <see cref="CookieContainer"/>.
    /// </summary>
    /// <returns>New instance of <see cref="ClientWebSocket"/>.</returns>
    public ClientWebSocket CreateClientWebSocket()
    {
        ClientWebSocket cws = new();
        cws.Options.Cookies = CookieContainer;
        return cws;
    }

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
                try
                {
                    _httpClient.Dispose();
                }
                catch
                {
                    // ignored
                }
                try
                {
                    _httpMessageHandler.Dispose();
                }
                catch
                {
                    // ignored
                }
            }

            // free unmanaged resources (unmanaged objects) and override finalizer
            // set large fields to null
            _httpClient = null!;
            _httpMessageHandler = null!;
            _disposed = true;
        }
    }

    private void NotDisposed()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(HttpArtifactTool));
    }

    #endregion
}
