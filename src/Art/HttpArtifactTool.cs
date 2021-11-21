using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Art;

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
    /// HTTP client used by this instance.
    /// </summary>
    protected HttpClient HttpClient
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
    /// Http client handler for <see cref="HttpClient"/>.
    /// </summary>
    protected HttpClientHandler HttpClientHandler
    {
        get
        {
            NotDisposed();
            return _httpClientHandler;
        }
        set
        {
            NotDisposed();
            _httpClientHandler = value;
        }
    }

    #endregion

    #region Private fields

    private HttpClient _httpClient;

    private HttpClientHandler _httpClientHandler;

    private bool _disposed;

    #endregion

    #region Constructor

    /// <summary>
    /// Creates a new instance of <see cref="HttpArtifactTool"/>.
    /// </summary>
    protected HttpArtifactTool()
    {
        CookieContainer cookies = new();
        ConfigureCookieContainer(cookies);
        _httpClientHandler = CreateHttpClientHandler(cookies);
        _httpClient = CreateHttpClient(_httpClientHandler);
    }

    /// <summary>
    /// Creates a new instance of <see cref="HttpArtifactTool"/>.
    /// </summary>
    /// <param name="httpClientInstance">Optional existing http client instance to use.</param>
    /// <remarks>
    /// No configuration will be performed on the <see cref="System.Net.Http.HttpClient"/> if provided. However, derived constructors can access the <see cref="HttpClient"/> member for configuration.
    /// </remarks>
    protected HttpArtifactTool(HttpClientInstance? httpClientInstance)
    {
        if (httpClientInstance != null)
        {
            _httpClient = httpClientInstance.HttpClient;
            _httpClientHandler = httpClientInstance.HttpClientHandler;
        }
        else
        {
            CookieContainer cookies = new();
            ConfigureCookieContainer(cookies);
            _httpClientHandler = CreateHttpClientHandler(cookies);
            _httpClient = CreateHttpClient(_httpClientHandler);
        }
    }

    #endregion

    #region Http client configuraiton

    /// <summary>
    /// Auto-configure an existing cookie container (using the <see cref="OptCookieFile"/> configuration option).
    /// </summary>
    /// <param name="cookies">Cookie container to configure.</param>
    protected virtual void ConfigureCookieContainer(CookieContainer cookies)
    {
        if (TryGetOption(OptCookieFile, out string? cookieFile))
            using (StreamReader? f = File.OpenText(cookieFile))
                cookies.LoadCookieFile(f);
    }

    /// <summary>
    /// Creates an <see cref="HttpClientHandler"/> instance configured to use the specified cookies.
    /// </summary>
    /// <param name="cookies"></param>
    /// <returns></returns>
    protected virtual HttpClientHandler CreateHttpClientHandler(CookieContainer cookies) => new()
    {
        AutomaticDecompression = DecompressionMethods.All,
        CookieContainer = cookies
    };

    /// <summary>
    /// Creates an <see cref="System.Net.Http.HttpClient"/> instance configured to use the specified client handler.
    /// </summary>
    /// <param name="httpClientHandler">Client handler.</param>
    /// <returns>Configured HTTP client.</returns>
    protected virtual HttpClient CreateHttpClient(HttpClientHandler httpClientHandler)
        => new(httpClientHandler);

    #endregion

    #region HTTP

    #region Raw http requests

    /// <summary>
    /// Sends an HTTP HEAD request.
    /// </summary>
    /// <param name="requestUri">Request.</param>
    /// <returns>Task returning reponse.</returns>
    protected async ValueTask<HttpResponseMessage> HeadAsync(string requestUri)
    {
        return await HttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, requestUri)).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends an HTTP HEAD request.
    /// </summary>
    /// <param name="requestUri">Request.</param>
    /// <returns>Task returning reponse.</returns>
    protected async ValueTask<HttpResponseMessage> HeadAsync(Uri requestUri)
    {
        return await HttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, requestUri)).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends an HTTP GET request.
    /// </summary>
    /// <param name="requestUri">Request.</param>
    /// <returns>Task returning reponse.</returns>
    protected async ValueTask<HttpResponseMessage> GetAsync(string requestUri)
    {
        return await HttpClient.GetAsync(requestUri).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends an HTTP GET request.
    /// </summary>
    /// <param name="requestUri">Request.</param>
    /// <returns>Task returning reponse.</returns>
    protected async ValueTask<HttpResponseMessage> GetAsync(Uri requestUri)
    {
        return await HttpClient.GetAsync(requestUri).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends an HTTP request.
    /// </summary>
    /// <param name="requestMessage">Request.</param>
    /// <returns>Task returning reponse.</returns>
    protected async ValueTask<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage)
    {
        return await HttpClient.SendAsync(requestMessage).ConfigureAwait(false);
    }

    #endregion

    #region JSON

    /// <summary>
    /// Retrieve deserialized JSON using a uri.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="requestUri">Request URI.</param>
    /// <param name="origin">Request origin.</param>
    /// <param name="referrer">Request referrer.</param>
    /// <returns>Task returning deserialized data.</returns>
    /// <remarks>
    /// This overload usees <see cref="ArtifactTool.JsonOptions"/> member automatically.
    /// </remarks>
    protected async ValueTask<T> GetDeserializedJsonAsync<T>(string requestUri, string? origin = null, string? referrer = null)
    {
        NotDisposed();
        HttpRequestMessage req = new(HttpMethod.Get, requestUri);
        SetOriginAndReferrer(req, origin, referrer);
        ConfigureJsonRequest(req);
        using HttpResponseMessage res = await HttpClient.SendAsync(req).ConfigureAwait(false);
        res.EnsureSuccessStatusCode();
        return await DeserializeJsonWithDebugAsync<T>(res).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieve deserialized JSON using a uri and <see cref="JsonSerializerOptions"/>.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="requestUri">Request URI.</param>
    /// <param name="jsonSerializerOptions">Optional deserialization options.</param>
    /// <param name="origin">Request origin.</param>
    /// <param name="referrer">Request referrer.</param>
    /// <returns>Task returning deserialized data.</returns>
    protected async ValueTask<T> GetDeserializedJsonAsync<T>(string requestUri, JsonSerializerOptions? jsonSerializerOptions, string? origin = null, string? referrer = null)
    {
        NotDisposed();
        HttpRequestMessage req = new(HttpMethod.Get, requestUri);
        SetOriginAndReferrer(req, origin, referrer);
        ConfigureJsonRequest(req);
        using HttpResponseMessage res = await HttpClient.SendAsync(req).ConfigureAwait(false);
        res.EnsureSuccessStatusCode();
        return await DeserializeJsonWithDebugAsync<T>(res).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieve deserialized JSON using a <see cref="Uri"/>.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="requestUri">Request URI.</param>
    /// <param name="origin">Request origin.</param>
    /// <param name="referrer">Request referrer.</param>
    /// <returns>Task returning deserialized data.</returns>
    /// <remarks>
    /// This overload usees <see cref="ArtifactTool.JsonOptions"/> member automatically.
    /// </remarks>
    protected async ValueTask<T> GetDeserializedJsonAsync<T>(Uri requestUri, string? origin = null, string? referrer = null)
    {
        NotDisposed();
        HttpRequestMessage req = new(HttpMethod.Get, requestUri);
        SetOriginAndReferrer(req, origin, referrer);
        ConfigureJsonRequest(req);
        using HttpResponseMessage res = await HttpClient.SendAsync(req).ConfigureAwait(false);
        res.EnsureSuccessStatusCode();
        return await DeserializeJsonWithDebugAsync<T>(res).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieve deserialized JSON using a <see cref="Uri"/> and <see cref="JsonSerializerOptions"/>.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="requestUri">Request URI.</param>
    /// <param name="jsonSerializerOptions">Optional deserialization options.</param>
    /// <param name="origin">Request origin.</param>
    /// <param name="referrer">Request referrer.</param>
    /// <returns>Task returning deserialized data.</returns>
    protected async ValueTask<T> GetDeserializedJsonAsync<T>(Uri requestUri, JsonSerializerOptions? jsonSerializerOptions, string? origin = null, string? referrer = null)
    {
        NotDisposed();
        HttpRequestMessage req = new(HttpMethod.Get, requestUri);
        SetOriginAndReferrer(req, origin, referrer);
        ConfigureJsonRequest(req);
        using HttpResponseMessage res = await HttpClient.SendAsync(req).ConfigureAwait(false);
        res.EnsureSuccessStatusCode();
        return await DeserializeJsonWithDebugAsync<T>(res).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieve deserialized JSON using a <see cref="HttpRequestMessage"/>.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="requestMessage">Request to send.</param>
    /// <returns>Task returning deserialized data.</returns>
    /// <remarks>
    /// This overload usees <see cref="ArtifactTool.JsonOptions"/> member automatically.
    /// </remarks>
    protected async ValueTask<T> RetrieveDeserializedJsonAsync<T>(HttpRequestMessage requestMessage)
    {
        NotDisposed();
        using HttpResponseMessage res = await HttpClient.SendAsync(requestMessage).ConfigureAwait(false);
        res.EnsureSuccessStatusCode();
        return (await DeserializeJsonAsync<T>(await res.Content.ReadAsStreamAsync().ConfigureAwait(false), JsonOptions).ConfigureAwait(false))!;
    }

    /// <summary>
    /// Retrieve deserialized JSON using a <see cref="HttpRequestMessage"/> and <see cref="JsonSerializerOptions"/>.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="requestMessage">Request to send.</param>
    /// <param name="jsonSerializerOptions">Optional deserialization options.</param>
    /// <returns>Task returning deserialized data.</returns>
    protected async ValueTask<T> RetrieveDeserializedJsonAsync<T>(HttpRequestMessage requestMessage, JsonSerializerOptions? jsonSerializerOptions)
    {
        NotDisposed();
        using HttpResponseMessage res = await HttpClient.SendAsync(requestMessage).ConfigureAwait(false);
        res.EnsureSuccessStatusCode();
        return (await DeserializeJsonAsync<T>(await res.Content.ReadAsStreamAsync().ConfigureAwait(false), jsonSerializerOptions).ConfigureAwait(false))!;
    }

    /// <summary>
    /// Deserialize JSON asynchronously, with debug output if <see cref="ArtifactTool.DebugMode"/> is enabled.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="response">Response to read from.</param>
    /// <returns>Task returning value.</returns>
    /// <remarks>
    /// This overload usees <see cref="ArtifactTool.JsonOptions"/> member automatically.
    /// </remarks>
    protected ValueTask<T> DeserializeJsonWithDebugAsync<T>(HttpResponseMessage response)
        => DeserializeJsonWithDebugAsync<T>(response, JsonOptions);

    /// <summary>
    /// Deserialize JSON asynchronously, with debug output if <see cref="ArtifactTool.DebugMode"/> is enabled.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="response">Response to read from.</param>
    /// <param name="jsonSerializerOptions">Optional deserialization options.</param>
    /// <returns>Task returning value.</returns>
    protected async ValueTask<T> DeserializeJsonWithDebugAsync<T>(HttpResponseMessage response, JsonSerializerOptions? jsonSerializerOptions)
    {
        response.EnsureSuccessStatusCode();
        if (!DebugMode)
            return await DeserializeJsonAsync<T>(await response.Content.ReadAsStreamAsync().ConfigureAwait(false), jsonSerializerOptions).ConfigureAwait(false);
        else
        {
            string text = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            LogInformation($"JSON from {response.RequestMessage?.RequestUri?.ToString() ?? "unknown request"}", text);
            return DeserializeJson<T>(text, jsonSerializerOptions);
        }
    }

    /// <summary>
    /// Sets origin and referrer on a request.
    /// </summary>
    /// <param name="request">Request to configure.</param>
    /// <param name="origin">Request origin.</param>
    /// <param name="referrer">Request referrer.</param>
    protected static void SetOriginAndReferrer(HttpRequestMessage request, string? origin, string? referrer)
    {
        if (origin != null)
            request.Headers.Add("origin", referrer ?? origin);
        if (referrer != null || origin != null)
            request.Headers.Referrer = new((referrer ?? origin)!);
    }

    /// <summary>
    /// Configures a JSON request.
    /// </summary>
    /// <param name="request">Request to configure.</param>
    protected virtual void ConfigureJsonRequest(HttpRequestMessage request)
    {
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
        request.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue("en-US"));
        request.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue("en", 0.9));
        request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
        request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
        request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("br"));
    }

    #endregion

    #endregion

    #region Direct downloads

    /// <summary>
    /// Downloads a resource.
    /// </summary>
    /// <param name="requestUri">Uri to download from.</param>
    /// <param name="stream">Target stream.</param>
    /// <returns>Task.</returns>
    protected internal async ValueTask DownloadResourceAsync(string requestUri, Stream stream)
    {
        NotDisposed();
        using HttpResponseMessage? fr = await HttpClient.GetAsync(requestUri).ConfigureAwait(false);
        fr.EnsureSuccessStatusCode();
        await fr.Content.CopyToAsync(stream).ConfigureAwait(false);
    }

    /// <summary>
    /// Downloads a resource.
    /// </summary>
    /// <param name="requestUri">Uri to download from.</param>
    /// <param name="file">Target filename.</param>
    /// <param name="artifactInfo">Target artifact.</param>
    /// <param name="path">File path to prepend.</param>
    /// <param name="inArtifactFolder">If false, place artifact under common root.</param>
    /// <returns>Task.</returns>
    protected async ValueTask DownloadResourceAsync(string requestUri, string file, ArtifactInfo artifactInfo, string? path = null, bool inArtifactFolder = true)
    {
        NotDisposed();
        using HttpResponseMessage? fr = await HttpClient.GetAsync(requestUri).ConfigureAwait(false);
        fr.EnsureSuccessStatusCode();
        await using Stream stream = await CreateOutputStreamAsync(file, artifactInfo, path, inArtifactFolder).ConfigureAwait(false);
        await fr.Content.CopyToAsync(stream).ConfigureAwait(false);
    }

    /// <summary>
    /// Downloads a resource.
    /// </summary>
    /// <param name="requestUri"><see cref="Uri"/> to download from.</param>
    /// <param name="stream">Target stream.</param>
    /// <returns>Task.</returns>
    protected internal async ValueTask DownloadResourceAsync(Uri requestUri, Stream stream)
    {
        NotDisposed();
        using HttpResponseMessage? fr = await HttpClient.GetAsync(requestUri).ConfigureAwait(false);
        fr.EnsureSuccessStatusCode();
        await fr.Content.CopyToAsync(stream).ConfigureAwait(false);
    }

    /// <summary>
    /// Downloads a resource.
    /// </summary>
    /// <param name="requestUri"><see cref="Uri"/> to download from.</param>
    /// <param name="file">Target filename.</param>
    /// <param name="artifactInfo">Target artifact.</param>
    /// <param name="path">File path to prepend.</param>
    /// <param name="inArtifactFolder">If false, place artifact under common root.</param>
    /// <returns>Task.</returns>
    protected async ValueTask DownloadResourceAsync(Uri requestUri, string file, ArtifactInfo artifactInfo, string? path = null, bool inArtifactFolder = true)
    {
        NotDisposed();
        using HttpResponseMessage? fr = await HttpClient.GetAsync(requestUri).ConfigureAwait(false);
        fr.EnsureSuccessStatusCode();
        await using Stream stream = await CreateOutputStreamAsync(file, artifactInfo, path, inArtifactFolder).ConfigureAwait(false);
        await fr.Content.CopyToAsync(stream).ConfigureAwait(false);
    }

    /// <summary>
    /// Downloads a resource.
    /// </summary>
    /// <param name="requestMessage">Request to send.</param>
    /// <param name="stream">Target stream.</param>
    /// <returns>Task.</returns>
    protected internal async ValueTask DownloadResourceAsync(HttpRequestMessage requestMessage, Stream stream)
    {
        NotDisposed();
        using HttpResponseMessage? fr = await HttpClient.SendAsync(requestMessage).ConfigureAwait(false);
        fr.EnsureSuccessStatusCode();
        await fr.Content.CopyToAsync(stream).ConfigureAwait(false);
    }

    /// <summary>
    /// Downloads a resource.
    /// </summary>
    /// <param name="requestMessage">Request to send.</param>
    /// <param name="file">Target filename.</param>
    /// <param name="artifactInfo">Target artifact.</param>
    /// <param name="path">File path to prepend.</param>
    /// <param name="inArtifactFolder">If false, place artifact under common root.</param>
    /// <returns>Task.</returns>
    protected async ValueTask DownloadResourceAsync(HttpRequestMessage requestMessage, string file, ArtifactInfo artifactInfo, string? path = null, bool inArtifactFolder = true)
    {
        NotDisposed();
        using HttpResponseMessage? fr = await HttpClient.SendAsync(requestMessage).ConfigureAwait(false);
        fr.EnsureSuccessStatusCode();
        await using Stream stream = await CreateOutputStreamAsync(file, artifactInfo, path, inArtifactFolder).ConfigureAwait(false);
        await fr.Content.CopyToAsync(stream).ConfigureAwait(false);
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
                _httpClient.Dispose();
                _httpClientHandler.Dispose();
            }

            // free unmanaged resources (unmanaged objects) and override finalizer
            // set large fields to null
            _httpClient = null!;
            _httpClientHandler = null!;
            _disposed = true;
        }
    }

    private void NotDisposed()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(HttpArtifactTool));
    }

    #endregion
}
