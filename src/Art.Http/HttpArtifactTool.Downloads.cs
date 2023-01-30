namespace Art.Http;

public partial class HttpArtifactTool
{
    #region Direct downloads

    /// <summary>
    /// Downloads a resource.
    /// </summary>
    /// <param name="requestUri">Uri to download from.</param>
    /// <param name="stream">Target stream.</param>
    /// <param name="requestAction">Custom configuration callback for the <see cref="HttpRequestMessage"/> created.</param>
    /// <param name="httpCompletionOption">Custom <see cref="System.Net.Http.HttpCompletionOption"/>.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    /// <exception cref="ArtHttpResponseMessageException">Thrown on HTTP response indicating non-successful response.</exception>
    public async Task DownloadResourceAsync(
        string requestUri,
        Stream stream,
        Action<HttpRequestMessage>? requestAction = null,
        HttpCompletionOption? httpCompletionOption = null,
        CancellationToken cancellationToken = default)
    {
        NotDisposed();
        HttpRequestMessage req = new(HttpMethod.Get, requestUri);
        ConfigureHttpRequest(req);
        requestAction?.Invoke(req);
        using HttpResponseMessage res = await HttpClient.SendAsync(req, httpCompletionOption ?? DownloadCompletionOption, cancellationToken).ConfigureAwait(false);
        ArtHttpResponseMessageException.EnsureSuccessStatusCode(res);
        await res.Content.CopyToAsync(stream, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Downloads a resource.
    /// </summary>
    /// <param name="requestUri">Uri to download from.</param>
    /// <param name="key">Resource key.</param>
    /// <param name="requestAction">Custom configuration callback for the <see cref="HttpRequestMessage"/> created.</param>
    /// <param name="httpCompletionOption">Custom <see cref="System.Net.Http.HttpCompletionOption"/>.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    /// <exception cref="ArtHttpResponseMessageException">Thrown on HTTP response indicating non-successful response.</exception>
    public async Task DownloadResourceAsync(
        string requestUri,
        ArtifactResourceKey key,
        Action<HttpRequestMessage>? requestAction = null,
        HttpCompletionOption? httpCompletionOption = null,
        CancellationToken cancellationToken = default)
    {
        NotDisposed();
        HttpRequestMessage req = new(HttpMethod.Get, requestUri);
        ConfigureHttpRequest(req);
        requestAction?.Invoke(req);
        await DownloadResourceInternalAsync(req, httpCompletionOption, key, cancellationToken);
    }

    /// <summary>
    /// Downloads a resource.
    /// </summary>
    /// <param name="requestUri">Uri to download from.</param>
    /// <param name="file">Target filename.</param>
    /// <param name="key">Artifact key.</param>
    /// <param name="path">File path to prepend.</param>
    /// <param name="requestAction">Custom configuration callback for the <see cref="HttpRequestMessage"/> created.</param>
    /// <param name="httpCompletionOption">Custom <see cref="System.Net.Http.HttpCompletionOption"/>.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    /// <exception cref="ArtHttpResponseMessageException">Thrown on HTTP response indicating non-successful response.</exception>
    public Task DownloadResourceAsync(
        string requestUri,
        string file,
        ArtifactKey key,
        string path = "",
        Action<HttpRequestMessage>? requestAction = null,
        HttpCompletionOption? httpCompletionOption = null,
        CancellationToken cancellationToken = default)
        => DownloadResourceAsync(requestUri, new ArtifactResourceKey(key, file, path), requestAction, httpCompletionOption, cancellationToken);

    /// <summary>
    /// Downloads a resource.
    /// </summary>
    /// <param name="requestUri"><see cref="Uri"/> to download from.</param>
    /// <param name="stream">Target stream.</param>
    /// <param name="requestAction">Custom configuration callback for the <see cref="HttpRequestMessage"/> created.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    /// <exception cref="ArtHttpResponseMessageException">Thrown on HTTP response indicating non-successful response.</exception>
    /// <param name="httpCompletionOption">Custom <see cref="System.Net.Http.HttpCompletionOption"/>.</param>
    public async Task DownloadResourceAsync(
        Uri requestUri,
        Stream stream,
        Action<HttpRequestMessage>? requestAction = null,
        HttpCompletionOption? httpCompletionOption = null,
        CancellationToken cancellationToken = default)
    {
        NotDisposed();
        HttpRequestMessage req = new(HttpMethod.Get, requestUri);
        ConfigureHttpRequest(req);
        requestAction?.Invoke(req);
        // M3U behaviour depends on members always using this instance's HttpClient.
        using HttpResponseMessage res = await HttpClient.SendAsync(req, httpCompletionOption ?? DownloadCompletionOption, cancellationToken).ConfigureAwait(false);
        ArtHttpResponseMessageException.EnsureSuccessStatusCode(res);
        await res.Content.CopyToAsync(stream, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Downloads a resource.
    /// </summary>
    /// <param name="requestUri"><see cref="Uri"/> to download from.</param>
    /// <param name="key">Resource key.</param>
    /// <param name="requestAction">Custom configuration callback for the <see cref="HttpRequestMessage"/> created.</param>
    /// <param name="httpCompletionOption">Custom <see cref="System.Net.Http.HttpCompletionOption"/>.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    /// <exception cref="ArtHttpResponseMessageException">Thrown on HTTP response indicating non-successful response.</exception>
    public async Task DownloadResourceAsync(
        Uri requestUri,
        ArtifactResourceKey key,
        Action<HttpRequestMessage>? requestAction = null,
        HttpCompletionOption? httpCompletionOption = null,
        CancellationToken cancellationToken = default)
    {
        NotDisposed();
        HttpRequestMessage req = new(HttpMethod.Get, requestUri);
        ConfigureHttpRequest(req);
        requestAction?.Invoke(req);
        await DownloadResourceInternalAsync(req, httpCompletionOption, key, cancellationToken);
    }

    /// <summary>
    /// Downloads a resource.
    /// </summary>
    /// <param name="requestUri"><see cref="Uri"/> to download from.</param>
    /// <param name="file">Target filename.</param>
    /// <param name="key">Artifact key.</param>
    /// <param name="path">File path to prepend.</param>
    /// <param name="requestAction">Custom configuration callback for the <see cref="HttpRequestMessage"/> created.</param>
    /// <param name="httpCompletionOption">Custom <see cref="System.Net.Http.HttpCompletionOption"/>.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    /// <exception cref="ArtHttpResponseMessageException">Thrown on HTTP response indicating non-successful response.</exception>
    public Task DownloadResourceAsync(
        Uri requestUri,
        string file,
        ArtifactKey key,
        string path = "",
        Action<HttpRequestMessage>? requestAction = null,
        HttpCompletionOption? httpCompletionOption = null,
        CancellationToken cancellationToken = default)
        => DownloadResourceAsync(requestUri, new ArtifactResourceKey(key, file, path), requestAction, httpCompletionOption, cancellationToken);

    /// <summary>
    /// Downloads a resource.
    /// </summary>
    /// <param name="requestMessage">Request to send.</param>
    /// <param name="stream">Target stream.</param>
    /// <param name="httpCompletionOption">Custom <see cref="System.Net.Http.HttpCompletionOption"/>.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    /// <exception cref="ArtHttpResponseMessageException">Thrown on HTTP response indicating non-successful response.</exception>
    public async Task DownloadResourceAsync(
        HttpRequestMessage requestMessage,
        Stream stream,
        HttpCompletionOption? httpCompletionOption = null,
        CancellationToken cancellationToken = default)
    {
        NotDisposed();
        // M3U behaviour depends on members always using this instance's HttpClient.
        using HttpResponseMessage res = await HttpClient.SendAsync(requestMessage, httpCompletionOption ?? DownloadCompletionOption, cancellationToken).ConfigureAwait(false);
        ArtHttpResponseMessageException.EnsureSuccessStatusCode(res);
        await res.Content.CopyToAsync(stream, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Downloads a resource.
    /// </summary>
    /// <param name="requestMessage">Request to send.</param>
    /// <param name="key">Resource key.</param>
    /// <param name="httpCompletionOption">Custom <see cref="System.Net.Http.HttpCompletionOption"/>.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    /// <exception cref="ArtHttpResponseMessageException">Thrown on HTTP response indicating non-successful response.</exception>
    public async Task DownloadResourceAsync(
        HttpRequestMessage requestMessage,
        ArtifactResourceKey key,
        HttpCompletionOption? httpCompletionOption = null,
        CancellationToken cancellationToken = default)
    {
        NotDisposed();
        await DownloadResourceInternalAsync(requestMessage, httpCompletionOption, key, cancellationToken);
    }

    /// <summary>
    /// Downloads a resource.
    /// </summary>
    /// <param name="requestMessage">Request to send.</param>
    /// <param name="file">Target filename.</param>
    /// <param name="key">Artifact key.</param>
    /// <param name="path">File path to prepend.</param>
    /// <param name="httpCompletionOption">Custom <see cref="System.Net.Http.HttpCompletionOption"/>.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    /// <exception cref="ArtHttpResponseMessageException">Thrown on HTTP response indicating non-successful response.</exception>
    public Task DownloadResourceAsync(
        HttpRequestMessage requestMessage,
        string file,
        ArtifactKey key,
        string path = "",
        HttpCompletionOption? httpCompletionOption = null,
        CancellationToken cancellationToken = default)
    {
        NotDisposed();
        return DownloadResourceInternalAsync(requestMessage, httpCompletionOption, new ArtifactResourceKey(key, file, path), cancellationToken);
    }

    /// <summary>
    /// Default <see cref="HttpCompletionOption"/> for download requests.
    /// </summary>
    public virtual HttpCompletionOption DownloadCompletionOption => HttpCompletionOption.ResponseHeadersRead;

    private async Task DownloadResourceInternalAsync(HttpRequestMessage requestMessage, HttpCompletionOption? httpCompletionOption, ArtifactResourceKey key, CancellationToken cancellationToken = default)
    {
        using HttpResponseMessage res = await HttpClient.SendAsync(requestMessage, httpCompletionOption ?? DownloadCompletionOption, cancellationToken).ConfigureAwait(false);
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
