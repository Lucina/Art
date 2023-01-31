﻿namespace Art.Http;

public partial class HttpArtifactTool
{
    #region Direct downloads

    /// <summary>
    /// Downloads a resource.
    /// </summary>
    /// <param name="requestUri">Uri to download from.</param>
    /// <param name="stream">Target stream.</param>
    /// <param name="httpRequestConfig">Custom request configuration.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    /// <exception cref="TaskCanceledException">Thrown with <see cref="TimeoutException"/> <see cref="Exception.InnerException"/> for a timeout.</exception>
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    /// <exception cref="ArtHttpResponseMessageException">Thrown on HTTP response indicating non-successful response.</exception>
    public async Task DownloadResourceAsync(
        string requestUri,
        Stream stream,
        HttpRequestConfig? httpRequestConfig = null,
        CancellationToken cancellationToken = default)
    {
        NotDisposed();
        HttpRequestMessage req = new(HttpMethod.Get, requestUri);
        ConfigureHttpRequest(req);
        using HttpResponseMessage res = await HttpRequestConfig.SendConfiguredAsync(httpRequestConfig, HttpClient, req, DownloadCompletionOption, cancellationToken).ConfigureAwait(false);
        ArtHttpResponseMessageException.EnsureSuccessStatusCode(res);
        await res.Content.CopyToAsync(stream, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Downloads a resource.
    /// </summary>
    /// <param name="requestUri">Uri to download from.</param>
    /// <param name="key">Resource key.</param>
    /// <param name="httpRequestConfig">Custom request configuration.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    /// <exception cref="TaskCanceledException">Thrown with <see cref="TimeoutException"/> <see cref="Exception.InnerException"/> for a timeout.</exception>
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    /// <exception cref="ArtHttpResponseMessageException">Thrown on HTTP response indicating non-successful response.</exception>
    public async Task DownloadResourceAsync(
        string requestUri,
        ArtifactResourceKey key,
        HttpRequestConfig? httpRequestConfig = null,
        CancellationToken cancellationToken = default)
    {
        NotDisposed();
        HttpRequestMessage req = new(HttpMethod.Get, requestUri);
        ConfigureHttpRequest(req);
        await DownloadResourceInternalAsync(req, httpRequestConfig, key, cancellationToken);
    }

    /// <summary>
    /// Downloads a resource.
    /// </summary>
    /// <param name="requestUri">Uri to download from.</param>
    /// <param name="file">Target filename.</param>
    /// <param name="key">Artifact key.</param>
    /// <param name="path">File path to prepend.</param>
    /// <param name="httpRequestConfig">Custom request configuration.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    /// <exception cref="TaskCanceledException">Thrown with <see cref="TimeoutException"/> <see cref="Exception.InnerException"/> for a timeout.</exception>
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    /// <exception cref="ArtHttpResponseMessageException">Thrown on HTTP response indicating non-successful response.</exception>
    public Task DownloadResourceAsync(
        string requestUri,
        string file,
        ArtifactKey key,
        string path = "",
        HttpRequestConfig? httpRequestConfig = null,
        CancellationToken cancellationToken = default)
        => DownloadResourceAsync(requestUri, new ArtifactResourceKey(key, file, path), httpRequestConfig, cancellationToken);

    /// <summary>
    /// Downloads a resource.
    /// </summary>
    /// <param name="requestUri"><see cref="Uri"/> to download from.</param>
    /// <param name="stream">Target stream.</param>
    /// <param name="httpRequestConfig">Custom request configuration.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    /// <exception cref="TaskCanceledException">Thrown with <see cref="TimeoutException"/> <see cref="Exception.InnerException"/> for a timeout.</exception>
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    /// <exception cref="ArtHttpResponseMessageException">Thrown on HTTP response indicating non-successful response.</exception>
    public async Task DownloadResourceAsync(
        Uri requestUri,
        Stream stream,
        HttpRequestConfig? httpRequestConfig = null,
        CancellationToken cancellationToken = default)
    {
        NotDisposed();
        HttpRequestMessage req = new(HttpMethod.Get, requestUri);
        ConfigureHttpRequest(req);
        // M3U behaviour depends on members always using this instance's HttpClient.
        using HttpResponseMessage res = await HttpRequestConfig.SendConfiguredAsync(httpRequestConfig, HttpClient, req, DownloadCompletionOption, cancellationToken).ConfigureAwait(false);
        ArtHttpResponseMessageException.EnsureSuccessStatusCode(res);
        await res.Content.CopyToAsync(stream, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Downloads a resource.
    /// </summary>
    /// <param name="requestUri"><see cref="Uri"/> to download from.</param>
    /// <param name="key">Resource key.</param>
    /// <param name="httpRequestConfig">Custom request configuration.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    /// <exception cref="TaskCanceledException">Thrown with <see cref="TimeoutException"/> <see cref="Exception.InnerException"/> for a timeout.</exception>
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    /// <exception cref="ArtHttpResponseMessageException">Thrown on HTTP response indicating non-successful response.</exception>
    public async Task DownloadResourceAsync(
        Uri requestUri,
        ArtifactResourceKey key,
        HttpRequestConfig? httpRequestConfig = null,
        CancellationToken cancellationToken = default)
    {
        NotDisposed();
        HttpRequestMessage req = new(HttpMethod.Get, requestUri);
        ConfigureHttpRequest(req);
        await DownloadResourceInternalAsync(req, httpRequestConfig, key, cancellationToken);
    }

    /// <summary>
    /// Downloads a resource.
    /// </summary>
    /// <param name="requestUri"><see cref="Uri"/> to download from.</param>
    /// <param name="file">Target filename.</param>
    /// <param name="key">Artifact key.</param>
    /// <param name="path">File path to prepend.</param>
    /// <param name="httpRequestConfig">Custom request configuration.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    /// <exception cref="TaskCanceledException">Thrown with <see cref="TimeoutException"/> <see cref="Exception.InnerException"/> for a timeout.</exception>
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    /// <exception cref="ArtHttpResponseMessageException">Thrown on HTTP response indicating non-successful response.</exception>
    public Task DownloadResourceAsync(
        Uri requestUri,
        string file,
        ArtifactKey key,
        string path = "",
        HttpRequestConfig? httpRequestConfig = null,
        CancellationToken cancellationToken = default)
        => DownloadResourceAsync(requestUri, new ArtifactResourceKey(key, file, path), httpRequestConfig, cancellationToken);

    /// <summary>
    /// Downloads a resource.
    /// </summary>
    /// <param name="requestMessage">Request to send.</param>
    /// <param name="stream">Target stream.</param>
    /// <param name="httpRequestConfig">Custom request configuration.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    /// <exception cref="TaskCanceledException">Thrown with <see cref="TimeoutException"/> <see cref="Exception.InnerException"/> for a timeout.</exception>
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    /// <exception cref="ArtHttpResponseMessageException">Thrown on HTTP response indicating non-successful response.</exception>
    public async Task DownloadResourceAsync(
        HttpRequestMessage requestMessage,
        Stream stream,
        HttpRequestConfig? httpRequestConfig = null,
        CancellationToken cancellationToken = default)
    {
        NotDisposed();
        // M3U behaviour depends on members always using this instance's HttpClient.
        using HttpResponseMessage res = await HttpRequestConfig.SendConfiguredAsync(httpRequestConfig, HttpClient, requestMessage, DownloadCompletionOption, cancellationToken).ConfigureAwait(false);
        ArtHttpResponseMessageException.EnsureSuccessStatusCode(res);
        await res.Content.CopyToAsync(stream, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Downloads a resource.
    /// </summary>
    /// <param name="requestMessage">Request to send.</param>
    /// <param name="key">Resource key.</param>
    /// <param name="httpRequestConfig">Custom request configuration.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    /// <exception cref="TaskCanceledException">Thrown with <see cref="TimeoutException"/> <see cref="Exception.InnerException"/> for a timeout.</exception>
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    /// <exception cref="ArtHttpResponseMessageException">Thrown on HTTP response indicating non-successful response.</exception>
    public async Task DownloadResourceAsync(
        HttpRequestMessage requestMessage,
        ArtifactResourceKey key,
        HttpRequestConfig? httpRequestConfig = null,
        CancellationToken cancellationToken = default)
    {
        NotDisposed();
        await DownloadResourceInternalAsync(requestMessage, httpRequestConfig, key, cancellationToken);
    }

    /// <summary>
    /// Downloads a resource.
    /// </summary>
    /// <param name="requestMessage">Request to send.</param>
    /// <param name="file">Target filename.</param>
    /// <param name="key">Artifact key.</param>
    /// <param name="path">File path to prepend.</param>
    /// <param name="httpRequestConfig">Custom request configuration.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    /// <exception cref="TaskCanceledException">Thrown with <see cref="TimeoutException"/> <see cref="Exception.InnerException"/> for a timeout.</exception>
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    /// <exception cref="ArtHttpResponseMessageException">Thrown on HTTP response indicating non-successful response.</exception>
    public Task DownloadResourceAsync(
        HttpRequestMessage requestMessage,
        string file,
        ArtifactKey key,
        string path = "",
        HttpRequestConfig? httpRequestConfig = null,
        CancellationToken cancellationToken = default)
    {
        NotDisposed();
        return DownloadResourceInternalAsync(requestMessage, httpRequestConfig, new ArtifactResourceKey(key, file, path), cancellationToken);
    }

    /// <summary>
    /// Default <see cref="HttpCompletionOption"/> for download requests.
    /// </summary>
    public virtual HttpCompletionOption DownloadCompletionOption => HttpCompletionOption.ResponseHeadersRead;

    private async Task DownloadResourceInternalAsync(HttpRequestMessage requestMessage, HttpRequestConfig? httpRequestConfig, ArtifactResourceKey key, CancellationToken cancellationToken = default)
    {
        using HttpResponseMessage res = await HttpRequestConfig.SendConfiguredAsync(httpRequestConfig, HttpClient, requestMessage, DownloadCompletionOption, cancellationToken).ConfigureAwait(false);
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
