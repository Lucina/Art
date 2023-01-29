using System.Net.Http.Headers;

namespace Art.Http;

public partial class HttpArtifactTool
{
    #region Raw http requests

    /// <summary>
    /// Sends an HTTP HEAD request.
    /// </summary>
    /// <param name="requestUri">Request.</param>
    /// <param name="requestAction">Custom configuration callback for the <see cref="HttpRequestMessage"/> created.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning response (status left unchecked).</returns>
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    public async Task<HttpResponseMessage> HeadAsync(string requestUri, Action<HttpRequestMessage>? requestAction = null, CancellationToken cancellationToken = default)
    {
        NotDisposed();
        HttpRequestMessage req = new(HttpMethod.Head, requestUri);
        ConfigureHttpRequest(req);
        requestAction?.Invoke(req);
        return await HttpClient.SendAsync(req, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends an HTTP HEAD request.
    /// </summary>
    /// <param name="requestUri">Request.</param>
    /// <param name="requestAction">Custom configuration callback for the <see cref="HttpRequestMessage"/> created.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning response (status left unchecked).</returns>
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    public async Task<HttpResponseMessage> HeadAsync(Uri requestUri, Action<HttpRequestMessage>? requestAction = null, CancellationToken cancellationToken = default)
    {
        NotDisposed();
        HttpRequestMessage req = new(HttpMethod.Head, requestUri);
        ConfigureHttpRequest(req);
        requestAction?.Invoke(req);
        return await HttpClient.SendAsync(req, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends an HTTP GET request.
    /// </summary>
    /// <param name="requestUri">Request.</param>
    /// <param name="requestAction">Custom configuration callback for the <see cref="HttpRequestMessage"/> created.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning response (status left unchecked).</returns>
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    public async Task<HttpResponseMessage> GetAsync(string requestUri, Action<HttpRequestMessage>? requestAction = null, CancellationToken cancellationToken = default)
    {
        NotDisposed();
        HttpRequestMessage req = new(HttpMethod.Get, requestUri);
        ConfigureHttpRequest(req);
        requestAction?.Invoke(req);
        return await HttpClient.SendAsync(req, GenericCompletionOption, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends an HTTP GET request.
    /// </summary>
    /// <param name="requestUri">Request.</param>
    /// <param name="requestAction">Custom configuration callback for the <see cref="HttpRequestMessage"/> created.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning response (status left unchecked).</returns>
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    public async Task<HttpResponseMessage> GetAsync(Uri requestUri, Action<HttpRequestMessage>? requestAction = null, CancellationToken cancellationToken = default)
    {
        NotDisposed();
        HttpRequestMessage req = new(HttpMethod.Get, requestUri);
        ConfigureHttpRequest(req);
        requestAction?.Invoke(req);
        return await HttpClient.SendAsync(req, GenericCompletionOption, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends an HTTP request.
    /// </summary>
    /// <param name="requestMessage">Request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning response (status left unchecked).</returns>
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage, CancellationToken cancellationToken = default)
    {
        NotDisposed();
        return await HttpClient.SendAsync(requestMessage, GenericCompletionOption, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Configures an HTTP request.
    /// </summary>
    /// <param name="request">Request to configure.</param>
    public virtual void ConfigureHttpRequest(HttpRequestMessage request)
    {
        request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
        request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
        request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("br"));
    }

    /// <summary>
    /// Sets origin and referrer on a request.
    /// </summary>
    /// <param name="request">Request to configure.</param>
    /// <param name="origin">Request origin.</param>
    /// <param name="referrer">Request referrer.</param>
    public static void SetOriginAndReferrer(HttpRequestMessage request, string? origin, string? referrer)
    {
        if (referrer != null) SetOriginAndReferrer(request, new Uri(referrer));
        else if (origin != null) SetOriginAndReferrer(request, new Uri(origin));
    }

    private static void SetOriginAndReferrer(HttpRequestMessage request, Uri uri)
    {
        request.Headers.Add("origin", uri.GetComponents(UriComponents.SchemeAndServer, UriFormat.UriEscaped));
        request.Headers.Referrer = uri;
    }

    /// <summary>
    /// Default <see cref="HttpCompletionOption"/> for generic requests.
    /// </summary>
    public virtual HttpCompletionOption GenericCompletionOption => HttpCompletionOption.ResponseContentRead;

    #endregion
}
