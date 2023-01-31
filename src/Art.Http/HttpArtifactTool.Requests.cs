using System.Net.Http.Headers;

namespace Art.Http;

public partial class HttpArtifactTool
{
    #region Raw http requests

    /// <summary>
    /// Sends an HTTP HEAD request.
    /// </summary>
    /// <param name="requestUri">Request.</param>
    /// <param name="httpRequestConfig">Custom request configuration.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning response (status left unchecked).</returns>
    /// <exception cref="TaskCanceledException">Thrown with <see cref="TimeoutException"/> <see cref="Exception.InnerException"/> for a timeout.</exception>
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    public async Task<HttpResponseMessage> HeadAsync(
        string requestUri,
        HttpRequestConfig? httpRequestConfig = null,
        CancellationToken cancellationToken = default)
    {
        NotDisposed();
        HttpRequestMessage req = new(HttpMethod.Head, requestUri);
        ConfigureHttpRequest(req);
        return await HttpClient.SendAsync(req, GenericCompletionOption, httpRequestConfig, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends an HTTP HEAD request.
    /// </summary>
    /// <param name="requestUri">Request.</param>
    /// <param name="httpRequestConfig">Custom request configuration.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning response (status left unchecked).</returns>
    /// <exception cref="TaskCanceledException">Thrown with <see cref="TimeoutException"/> <see cref="Exception.InnerException"/> for a timeout.</exception>
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    public async Task<HttpResponseMessage> HeadAsync(
        Uri requestUri,
        HttpRequestConfig? httpRequestConfig = null,
        CancellationToken cancellationToken = default)
    {
        NotDisposed();
        HttpRequestMessage req = new(HttpMethod.Head, requestUri);
        ConfigureHttpRequest(req);
        return await HttpClient.SendAsync(req, GenericCompletionOption, httpRequestConfig, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends an HTTP GET request.
    /// </summary>
    /// <param name="requestUri">Request.</param>
    /// <param name="httpRequestConfig">Custom request configuration.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning response (status left unchecked).</returns>
    /// <exception cref="TaskCanceledException">Thrown with <see cref="TimeoutException"/> <see cref="Exception.InnerException"/> for a timeout.</exception>
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    public async Task<HttpResponseMessage> GetAsync(
        string requestUri,
        HttpRequestConfig? httpRequestConfig = null,
        CancellationToken cancellationToken = default)
    {
        NotDisposed();
        HttpRequestMessage req = new(HttpMethod.Get, requestUri);
        ConfigureHttpRequest(req);
        return await HttpClient.SendAsync(req, GenericCompletionOption, httpRequestConfig, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends an HTTP GET request.
    /// </summary>
    /// <param name="requestUri">Request.</param>
    /// <param name="httpRequestConfig">Custom request configuration.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning response (status left unchecked).</returns>
    /// <exception cref="TaskCanceledException">Thrown with <see cref="TimeoutException"/> <see cref="Exception.InnerException"/> for a timeout.</exception>
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    public async Task<HttpResponseMessage> GetAsync(
        Uri requestUri,
        HttpRequestConfig? httpRequestConfig = null,
        CancellationToken cancellationToken = default)
    {
        NotDisposed();
        HttpRequestMessage req = new(HttpMethod.Get, requestUri);
        ConfigureHttpRequest(req);
        return await HttpClient.SendAsync(req, GenericCompletionOption, httpRequestConfig, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends an HTTP request.
    /// </summary>
    /// <param name="requestMessage">Request.</param>
    /// <param name="httpRequestConfig">Custom request configuration.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning response (status left unchecked).</returns>
    /// <exception cref="TaskCanceledException">Thrown with <see cref="TimeoutException"/> <see cref="Exception.InnerException"/> for a timeout.</exception>
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    public async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage requestMessage,
        HttpRequestConfig? httpRequestConfig = null,
        CancellationToken cancellationToken = default)
    {
        NotDisposed();
        return await HttpClient.SendAsync(requestMessage, GenericCompletionOption, httpRequestConfig, cancellationToken).ConfigureAwait(false);
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
    /// Default <see cref="HttpCompletionOption"/> for generic requests.
    /// </summary>
    public virtual HttpCompletionOption GenericCompletionOption => HttpCompletionOption.ResponseContentRead;

    #endregion
}
