namespace Art.Http;

public partial class HttpArtifactTool
{
    #region Direct downloads

    /// <summary>
    /// Downloads a resource.
    /// </summary>
    /// <param name="requestUri">Uri to download from.</param>
    /// <param name="stream">Target stream.</param>
    /// <param name="origin">Request origin.</param>
    /// <param name="referrer">Request referrer.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    /// <exception cref="ArtHttpResponseMessageException">Thrown on HTTP response indicating non-successful response.</exception>
    public async Task DownloadResourceAsync(string requestUri, Stream stream, string? origin = null, string? referrer = null, CancellationToken cancellationToken = default)
    {
        NotDisposed();
        HttpRequestMessage req = new(HttpMethod.Get, requestUri);
        SetOriginAndReferrer(req, origin, referrer);
        ConfigureHttpRequest(req);
        using HttpResponseMessage res = await HttpClient.SendAsync(req, DownloadCompletionOption, cancellationToken).ConfigureAwait(false);
        ArtHttpResponseMessageException.EnsureSuccessStatusCode(res);
        await res.Content.CopyToAsync(stream, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Downloads a resource.
    /// </summary>
    /// <param name="requestUri">Uri to download from.</param>
    /// <param name="key">Resource key.</param>
    /// <param name="origin">Request origin.</param>
    /// <param name="referrer">Request referrer.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    /// <exception cref="ArtHttpResponseMessageException">Thrown on HTTP response indicating non-successful response.</exception>
    public async Task DownloadResourceAsync(string requestUri, ArtifactResourceKey key, string? origin = null, string? referrer = null, CancellationToken cancellationToken = default)
    {
        NotDisposed();
        HttpRequestMessage req = new(HttpMethod.Get, requestUri);
        SetOriginAndReferrer(req, origin, referrer);
        ConfigureHttpRequest(req);
        await DownloadResourceInternalAsync(req, key, cancellationToken);
    }

    /// <summary>
    /// Downloads a resource.
    /// </summary>
    /// <param name="requestUri">Uri to download from.</param>
    /// <param name="file">Target filename.</param>
    /// <param name="key">Artifact key.</param>
    /// <param name="path">File path to prepend.</param>
    /// <param name="origin">Request origin.</param>
    /// <param name="referrer">Request referrer.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    /// <exception cref="ArtHttpResponseMessageException">Thrown on HTTP response indicating non-successful response.</exception>
    public Task DownloadResourceAsync(string requestUri, string file, ArtifactKey key, string path = "", string? origin = null, string? referrer = null, CancellationToken cancellationToken = default)
        => DownloadResourceAsync(requestUri, new ArtifactResourceKey(key, file, path), origin, referrer, cancellationToken);

    /// <summary>
    /// Downloads a resource.
    /// </summary>
    /// <param name="requestUri"><see cref="Uri"/> to download from.</param>
    /// <param name="stream">Target stream.</param>
    /// <param name="origin">Request origin.</param>
    /// <param name="referrer">Request referrer.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    /// <exception cref="ArtHttpResponseMessageException">Thrown on HTTP response indicating non-successful response.</exception>
    public async Task DownloadResourceAsync(Uri requestUri, Stream stream, string? origin = null, string? referrer = null, CancellationToken cancellationToken = default)
    {
        NotDisposed();
        HttpRequestMessage req = new(HttpMethod.Get, requestUri);
        SetOriginAndReferrer(req, origin, referrer);
        ConfigureHttpRequest(req);
        using HttpResponseMessage res = await HttpClient.SendAsync(req, DownloadCompletionOption, cancellationToken).ConfigureAwait(false);
        ArtHttpResponseMessageException.EnsureSuccessStatusCode(res);
        await res.Content.CopyToAsync(stream, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Downloads a resource.
    /// </summary>
    /// <param name="requestUri"><see cref="Uri"/> to download from.</param>
    /// <param name="key">Resource key.</param>
    /// <param name="origin">Request origin.</param>
    /// <param name="referrer">Request referrer.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    /// <exception cref="ArtHttpResponseMessageException">Thrown on HTTP response indicating non-successful response.</exception>
    public async Task DownloadResourceAsync(Uri requestUri, ArtifactResourceKey key, string? origin = null, string? referrer = null, CancellationToken cancellationToken = default)
    {
        NotDisposed();
        HttpRequestMessage req = new(HttpMethod.Get, requestUri);
        SetOriginAndReferrer(req, origin, referrer);
        ConfigureHttpRequest(req);
        await DownloadResourceInternalAsync(req, key, cancellationToken);
    }

    /// <summary>
    /// Downloads a resource.
    /// </summary>
    /// <param name="requestUri"><see cref="Uri"/> to download from.</param>
    /// <param name="file">Target filename.</param>
    /// <param name="key">Artifact key.</param>
    /// <param name="path">File path to prepend.</param>
    /// <param name="origin">Request origin.</param>
    /// <param name="referrer">Request referrer.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    /// <exception cref="ArtHttpResponseMessageException">Thrown on HTTP response indicating non-successful response.</exception>
    public Task DownloadResourceAsync(Uri requestUri, string file, ArtifactKey key, string path = "", string? origin = null, string? referrer = null, CancellationToken cancellationToken = default)
        => DownloadResourceAsync(requestUri, new ArtifactResourceKey(key, file, path), origin, referrer, cancellationToken);

    /// <summary>
    /// Downloads a resource.
    /// </summary>
    /// <param name="requestMessage">Request to send.</param>
    /// <param name="stream">Target stream.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    /// <exception cref="ArtHttpResponseMessageException">Thrown on HTTP response indicating non-successful response.</exception>
    public async Task DownloadResourceAsync(HttpRequestMessage requestMessage, Stream stream, CancellationToken cancellationToken = default)
    {
        NotDisposed();
        using HttpResponseMessage res = await HttpClient.SendAsync(requestMessage, DownloadCompletionOption, cancellationToken).ConfigureAwait(false);
        ArtHttpResponseMessageException.EnsureSuccessStatusCode(res);
        await res.Content.CopyToAsync(stream, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Downloads a resource.
    /// </summary>
    /// <param name="requestMessage">Request to send.</param>
    /// <param name="key">Resource key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    /// <exception cref="ArtHttpResponseMessageException">Thrown on HTTP response indicating non-successful response.</exception>
    public async Task DownloadResourceAsync(HttpRequestMessage requestMessage, ArtifactResourceKey key, CancellationToken cancellationToken = default)
    {
        NotDisposed();
        await DownloadResourceInternalAsync(requestMessage, key, cancellationToken);
    }

    /// <summary>
    /// Downloads a resource.
    /// </summary>
    /// <param name="requestMessage">Request to send.</param>
    /// <param name="file">Target filename.</param>
    /// <param name="key">Artifact key.</param>
    /// <param name="path">File path to prepend.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    /// <exception cref="ArtHttpResponseMessageException">Thrown on HTTP response indicating non-successful response.</exception>
    public Task DownloadResourceAsync(HttpRequestMessage requestMessage, string file, ArtifactKey key, string path = "", CancellationToken cancellationToken = default)
    {
        NotDisposed();
        return DownloadResourceInternalAsync(requestMessage, new ArtifactResourceKey(key, file, path), cancellationToken);
    }

    /// <summary>
    /// Default <see cref="HttpCompletionOption"/> for download requests.
    /// </summary>
    public virtual HttpCompletionOption DownloadCompletionOption => HttpCompletionOption.ResponseHeadersRead;

    private async Task DownloadResourceInternalAsync(HttpRequestMessage requestMessage, ArtifactResourceKey key, CancellationToken cancellationToken = default)
    {
        using HttpResponseMessage res = await HttpClient.SendAsync(requestMessage, DownloadCompletionOption, cancellationToken).ConfigureAwait(false);
        ArtHttpResponseMessageException.EnsureSuccessStatusCode(res);
        await StreamDownloadAsync(res, key, cancellationToken);
    }

    private async Task StreamDownloadAsync(HttpResponseMessage response, ArtifactResourceKey key, CancellationToken cancellationToken)
    {
        OutputStreamOptions options = OutputStreamOptions.Default;
        if (response.Content.Headers.ContentLength is { } contentLength) options = options with { PreallocationSize = contentLength };
        await using CommittableStream stream = await CreateOutputStreamAsync(key, options, cancellationToken).ConfigureAwait(false);
        await response.Content.CopyToAsync(stream, cancellationToken).ConfigureAwait(false);
        stream.ShouldCommit = true;
    }

    #endregion

}
