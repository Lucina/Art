using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Art;

/// <summary>
/// Represents an instance of an artifact tool that depends on an <see cref="System.Net.Http.HttpClient"/>.
/// </summary>
public abstract class HttpArtifactTool : ArtifactTool
{
    #region Fields
    /// <summary>
    /// Option used to specify path to http client cookie file.
    /// </summary>
    public const string OptCookieFile = "cookieFile";

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
    /// Http client handler for <see cref="HttpClient"/>.
    /// </summary>
    public HttpClientHandler HttpClientHandler
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

    /// <summary>
    /// Dummy default user agent string.
    /// </summary>
    public const string DefaultUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.85 Safari/537.36 Edg/90.0.818.46";

    #endregion

    #region Private fields

    private HttpClient _httpClient = null!;

    private HttpClientHandler _httpClientHandler = null!;

    private bool _disposed;

    #endregion

    #region Configuration

    /// <inheritdoc/>
    public override async Task ConfigureAsync(ArtifactToolRuntimeConfig runtimeConfig, CancellationToken cancellationToken = default)
    {
        await base.ConfigureAsync(runtimeConfig, cancellationToken);
        CookieContainer cookies = CreateCookieContainer();
        _httpClientHandler = CreateHttpClientHandler(cookies);
        _httpClient = CreateHttpClient(_httpClientHandler);
        _httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd(DefaultUserAgent);
    }
    #endregion

    #region Http client configuraiton

    /// <summary>
    /// Creates a cookie container (by default using the <see cref="OptCookieFile"/> configuration option).
    /// </summary>
    /// <returns>A cookie container.</returns>
    public virtual CookieContainer CreateCookieContainer()
    {
        CookieContainer cookies = new();
        if (TryGetOption(OptCookieFile, out string? cookieFile))
            using (StreamReader? f = File.OpenText(cookieFile))
                cookies.LoadCookieFile(f);
        return cookies;
    }

    /// <summary>
    /// Creates an <see cref="HttpClientHandler"/> instance configured to use the specified cookies.
    /// </summary>
    /// <param name="cookies"></param>
    /// <returns></returns>
    public virtual HttpClientHandler CreateHttpClientHandler(CookieContainer cookies) => new()
    {
        AutomaticDecompression = DecompressionMethods.All,
        CookieContainer = cookies
    };

    /// <summary>
    /// Creates an <see cref="System.Net.Http.HttpClient"/> instance configured to use the specified client handler.
    /// </summary>
    /// <param name="httpClientHandler">Client handler.</param>
    /// <returns>Configured HTTP client.</returns>
    public virtual HttpClient CreateHttpClient(HttpClientHandler httpClientHandler)
        => new(httpClientHandler);

    #endregion

    #region HTTP

    #region Raw http requests

    /// <summary>
    /// Sends an HTTP HEAD request.
    /// </summary>
    /// <param name="requestUri">Request.</param>
    /// <param name="origin">Request origin.</param>
    /// <param name="referrer">Request referrer.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning reponse (status left unchecked).</returns>
    public async Task<HttpResponseMessage> HeadAsync(string requestUri, string? origin = null, string? referrer = null, CancellationToken cancellationToken = default)
    {
        NotDisposed();
        HttpRequestMessage req = new(HttpMethod.Head, requestUri);
        SetOriginAndReferrer(req, origin, referrer);
        ConfigureHttpRequest(req);
        return await HttpClient.SendAsync(req, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends an HTTP HEAD request.
    /// </summary>
    /// <param name="requestUri">Request.</param>
    /// <param name="origin">Request origin.</param>
    /// <param name="referrer">Request referrer.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning reponse (status left unchecked).</returns>
    public async Task<HttpResponseMessage> HeadAsync(Uri requestUri, string? origin = null, string? referrer = null, CancellationToken cancellationToken = default)
    {
        NotDisposed();
        HttpRequestMessage req = new(HttpMethod.Head, requestUri);
        SetOriginAndReferrer(req, origin, referrer);
        ConfigureHttpRequest(req);
        return await HttpClient.SendAsync(req, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends an HTTP GET request.
    /// </summary>
    /// <param name="requestUri">Request.</param>
    /// <param name="origin">Request origin.</param>
    /// <param name="referrer">Request referrer.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning reponse (status left unchecked).</returns>
    public async Task<HttpResponseMessage> GetAsync(string requestUri, string? origin = null, string? referrer = null, CancellationToken cancellationToken = default)
    {
        NotDisposed();
        HttpRequestMessage req = new(HttpMethod.Get, requestUri);
        SetOriginAndReferrer(req, origin, referrer);
        ConfigureHttpRequest(req);
        return await HttpClient.SendAsync(req, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends an HTTP GET request.
    /// </summary>
    /// <param name="requestUri">Request.</param>
    /// <param name="origin">Request origin.</param>
    /// <param name="referrer">Request referrer.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning reponse (status left unchecked).</returns>
    public async Task<HttpResponseMessage> GetAsync(Uri requestUri, string? origin = null, string? referrer = null, CancellationToken cancellationToken = default)
    {
        NotDisposed();
        HttpRequestMessage req = new(HttpMethod.Get, requestUri);
        SetOriginAndReferrer(req, origin, referrer);
        ConfigureHttpRequest(req);
        return await HttpClient.SendAsync(req, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends an HTTP request.
    /// </summary>
    /// <param name="requestMessage">Request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning reponse (status left unchecked).</returns>
    public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage, CancellationToken cancellationToken = default)
    {
        NotDisposed();
        return await HttpClient.SendAsync(requestMessage, cancellationToken).ConfigureAwait(false);
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

    #endregion

    #region JSON

    /// <summary>
    /// Retrieves deserialized JSON using a uri.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="requestUri">Request URI.</param>
    /// <param name="origin">Request origin.</param>
    /// <param name="referrer">Request referrer.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning deserialized data.</returns>
    /// <remarks>
    /// This overload uses <see cref="ArtifactTool.JsonOptions"/> member automatically.
    /// </remarks>
    public async Task<T?> GetDeserializedJsonAsync<T>(string requestUri, string? origin = null, string? referrer = null, CancellationToken cancellationToken = default)
    {
        NotDisposed();
        HttpRequestMessage req = new(HttpMethod.Get, requestUri);
        SetOriginAndReferrer(req, origin, referrer);
        ConfigureJsonRequest(req);
        using HttpResponseMessage res = await HttpClient.SendAsync(req, cancellationToken).ConfigureAwait(false);
        res.EnsureSuccessStatusCode();
        return await DeserializeJsonWithDebugAsync<T>(res, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves deserialized JSON using a uri.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="requestUri">Request URI.</param>
    /// <param name="origin">Request origin.</param>
    /// <param name="referrer">Request referrer.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning deserialized data.</returns>
    /// <remarks>
    /// This overload uses <see cref="ArtifactTool.JsonOptions"/> member automatically.
    /// </remarks>
    public async Task<T> GetDeserializedRequiredJsonAsync<T>(string requestUri, string? origin = null, string? referrer = null, CancellationToken cancellationToken = default)
    {
        return (await GetDeserializedJsonAsync<T>(requestUri, origin, referrer, cancellationToken).ConfigureAwait(false)) ?? throw new NullJsonDataException();
    }

    /// <summary>
    /// Retrieves deserialized JSON using a uri and <see cref="JsonSerializerOptions"/>.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="requestUri">Request URI.</param>
    /// <param name="jsonSerializerOptions">Optional deserialization options.</param>
    /// <param name="origin">Request origin.</param>
    /// <param name="referrer">Request referrer.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning deserialized data.</returns>
    public async Task<T?> GetDeserializedJsonAsync<T>(string requestUri, JsonSerializerOptions? jsonSerializerOptions, string? origin = null, string? referrer = null, CancellationToken cancellationToken = default)
    {
        NotDisposed();
        HttpRequestMessage req = new(HttpMethod.Get, requestUri);
        SetOriginAndReferrer(req, origin, referrer);
        ConfigureJsonRequest(req);
        using HttpResponseMessage res = await HttpClient.SendAsync(req, cancellationToken).ConfigureAwait(false);
        res.EnsureSuccessStatusCode();
        return await DeserializeJsonWithDebugAsync<T>(res, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves deserialized JSON using a uri and <see cref="JsonSerializerOptions"/>.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="requestUri">Request URI.</param>
    /// <param name="jsonSerializerOptions">Optional deserialization options.</param>
    /// <param name="origin">Request origin.</param>
    /// <param name="referrer">Request referrer.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning deserialized data.</returns>
    public async Task<T> GetDeserializedRequiredJsonAsync<T>(string requestUri, JsonSerializerOptions? jsonSerializerOptions, string? origin = null, string? referrer = null, CancellationToken cancellationToken = default)
    {
        return (await GetDeserializedJsonAsync<T>(requestUri, jsonSerializerOptions, origin, referrer, cancellationToken).ConfigureAwait(false)) ?? throw new NullJsonDataException();
    }

    /// <summary>
    /// Retrieves deserialized JSON using a <see cref="Uri"/>.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="requestUri">Request URI.</param>
    /// <param name="origin">Request origin.</param>
    /// <param name="referrer">Request referrer.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning deserialized data.</returns>
    /// <remarks>
    /// This overload uses <see cref="ArtifactTool.JsonOptions"/> member automatically.
    /// </remarks>
    public async Task<T?> GetDeserializedJsonAsync<T>(Uri requestUri, string? origin = null, string? referrer = null, CancellationToken cancellationToken = default)
    {
        NotDisposed();
        HttpRequestMessage req = new(HttpMethod.Get, requestUri);
        SetOriginAndReferrer(req, origin, referrer);
        ConfigureJsonRequest(req);
        using HttpResponseMessage res = await HttpClient.SendAsync(req, cancellationToken).ConfigureAwait(false);
        res.EnsureSuccessStatusCode();
        return await DeserializeJsonWithDebugAsync<T>(res, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves deserialized JSON using a <see cref="Uri"/>.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="requestUri">Request URI.</param>
    /// <param name="origin">Request origin.</param>
    /// <param name="referrer">Request referrer.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning deserialized data.</returns>
    /// <remarks>
    /// This overload uses <see cref="ArtifactTool.JsonOptions"/> member automatically.
    /// </remarks>
    public async Task<T> GetDeserializedRequiredJsonAsync<T>(Uri requestUri, string? origin = null, string? referrer = null, CancellationToken cancellationToken = default)
    {
        return (await GetDeserializedJsonAsync<T>(requestUri, origin, referrer, cancellationToken).ConfigureAwait(false)) ?? throw new NullJsonDataException();
    }

    /// <summary>
    /// Retrieves deserialized JSON using a <see cref="Uri"/> and <see cref="JsonSerializerOptions"/>.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="requestUri">Request URI.</param>
    /// <param name="jsonSerializerOptions">Optional deserialization options.</param>
    /// <param name="origin">Request origin.</param>
    /// <param name="referrer">Request referrer.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning deserialized data.</returns>
    public async Task<T?> GetDeserializedJsonAsync<T>(Uri requestUri, JsonSerializerOptions? jsonSerializerOptions, string? origin = null, string? referrer = null, CancellationToken cancellationToken = default)
    {
        NotDisposed();
        HttpRequestMessage req = new(HttpMethod.Get, requestUri);
        SetOriginAndReferrer(req, origin, referrer);
        ConfigureJsonRequest(req);
        using HttpResponseMessage res = await HttpClient.SendAsync(req, cancellationToken).ConfigureAwait(false);
        res.EnsureSuccessStatusCode();
        return await DeserializeJsonWithDebugAsync<T>(res, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves deserialized JSON using a <see cref="Uri"/> and <see cref="JsonSerializerOptions"/>.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="requestUri">Request URI.</param>
    /// <param name="jsonSerializerOptions">Optional deserialization options.</param>
    /// <param name="origin">Request origin.</param>
    /// <param name="referrer">Request referrer.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning deserialized data.</returns>
    public async Task<T> GetDeserializedRequiredJsonAsync<T>(Uri requestUri, JsonSerializerOptions? jsonSerializerOptions, string? origin = null, string? referrer = null, CancellationToken cancellationToken = default)
    {
        return (await GetDeserializedJsonAsync<T>(requestUri, jsonSerializerOptions, origin, referrer, cancellationToken).ConfigureAwait(false)) ?? throw new NullJsonDataException();
    }

    /// <summary>
    /// Retrieves deserialized JSON using a <see cref="HttpRequestMessage"/>.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="requestMessage">Request to send.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning deserialized data.</returns>
    /// <remarks>
    /// This overload uses <see cref="ArtifactTool.JsonOptions"/> member automatically.
    /// </remarks>
    public async Task<T?> RetrieveDeserializedJsonAsync<T>(HttpRequestMessage requestMessage, CancellationToken cancellationToken = default)
    {
        NotDisposed();
        using HttpResponseMessage res = await HttpClient.SendAsync(requestMessage, cancellationToken).ConfigureAwait(false);
        res.EnsureSuccessStatusCode();
        return (await DeserializeJsonAsync<T>(await res.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false), JsonOptions, cancellationToken).ConfigureAwait(false));
    }

    /// <summary>
    /// Retrieves deserialized JSON using a <see cref="HttpRequestMessage"/>.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="requestMessage">Request to send.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning deserialized data.</returns>
    /// <remarks>
    /// This overload uses <see cref="ArtifactTool.JsonOptions"/> member automatically.
    /// </remarks>
    public async Task<T> RetrieveDeserializedRequiredJsonAsync<T>(HttpRequestMessage requestMessage, CancellationToken cancellationToken = default)
    {
        return (await RetrieveDeserializedJsonAsync<T>(requestMessage, cancellationToken).ConfigureAwait(false)) ?? throw new NullJsonDataException();
    }

    /// <summary>
    /// Retrieves deserialized JSON using a <see cref="HttpRequestMessage"/> and <see cref="JsonSerializerOptions"/>.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="requestMessage">Request to send.</param>
    /// <param name="jsonSerializerOptions">Optional deserialization options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning deserialized data.</returns>
    public async Task<T?> RetrieveDeserializedJsonAsync<T>(HttpRequestMessage requestMessage, JsonSerializerOptions? jsonSerializerOptions, CancellationToken cancellationToken = default)
    {
        NotDisposed();
        using HttpResponseMessage res = await HttpClient.SendAsync(requestMessage, cancellationToken).ConfigureAwait(false);
        res.EnsureSuccessStatusCode();
        return (await DeserializeJsonAsync<T>(await res.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false), jsonSerializerOptions, cancellationToken).ConfigureAwait(false));
    }

    /// <summary>
    /// Retrieves deserialized JSON using a <see cref="HttpRequestMessage"/> and <see cref="JsonSerializerOptions"/>.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="requestMessage">Request to send.</param>
    /// <param name="jsonSerializerOptions">Optional deserialization options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning deserialized data.</returns>
    public async Task<T> RetrieveDeserializedRequiredJsonAsync<T>(HttpRequestMessage requestMessage, JsonSerializerOptions? jsonSerializerOptions, CancellationToken cancellationToken = default)
    {
        return (await RetrieveDeserializedJsonAsync<T>(requestMessage, jsonSerializerOptions, cancellationToken).ConfigureAwait(false)) ?? throw new NullJsonDataException();
    }

    /// <summary>
    /// Deserialize JSON asynchronously, with debug output if <see cref="ArtifactTool.DebugMode"/> is enabled.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="response">Response to read from.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning value.</returns>
    /// <remarks>
    /// This overload uses <see cref="ArtifactTool.JsonOptions"/> member automatically.
    /// </remarks>
    public Task<T?> DeserializeJsonWithDebugAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken = default)
        => DeserializeJsonWithDebugAsync<T>(response, JsonOptions, cancellationToken);

    /// <summary>
    /// Deserialize JSON asynchronously, with debug output if <see cref="ArtifactTool.DebugMode"/> is enabled.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="response">Response to read from.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning value.</returns>
    /// <remarks>
    /// This overload uses <see cref="ArtifactTool.JsonOptions"/> member automatically.
    /// </remarks>
    public Task<T> DeserializeRequiredJsonWithDebugAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken = default)
        => DeserializeRequiredJsonWithDebugAsync<T>(response, JsonOptions, cancellationToken);

    /// <summary>
    /// Deserialize JSON asynchronously, with debug output if <see cref="ArtifactTool.DebugMode"/> is enabled.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="response">Response to read from.</param>
    /// <param name="jsonSerializerOptions">Optional deserialization options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning value.</returns>
    public async Task<T?> DeserializeJsonWithDebugAsync<T>(HttpResponseMessage response, JsonSerializerOptions? jsonSerializerOptions, CancellationToken cancellationToken = default)
    {
        response.EnsureSuccessStatusCode();
        if (!DebugMode)
            return await DeserializeJsonAsync<T>(await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false), jsonSerializerOptions, cancellationToken).ConfigureAwait(false);
        else
        {
            string text = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            LogInformation($"JSON from {response.RequestMessage?.RequestUri?.ToString() ?? "unknown request"}", text);
            return DeserializeJson<T>(text, jsonSerializerOptions);
        }
    }

    /// <summary>
    /// Deserialize JSON asynchronously, with debug output if <see cref="ArtifactTool.DebugMode"/> is enabled.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="response">Response to read from.</param>
    /// <param name="jsonSerializerOptions">Optional deserialization options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning value.</returns>
    public async Task<T> DeserializeRequiredJsonWithDebugAsync<T>(HttpResponseMessage response, JsonSerializerOptions? jsonSerializerOptions, CancellationToken cancellationToken = default)
    {
        response.EnsureSuccessStatusCode();
        if (!DebugMode)
            return await DeserializeRequiredJsonAsync<T>(await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false), jsonSerializerOptions, cancellationToken).ConfigureAwait(false);
        else
        {
            string text = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            LogInformation($"JSON from {response.RequestMessage?.RequestUri?.ToString() ?? "unknown request"}", text);
            return DeserializeRequiredJson<T>(text, jsonSerializerOptions);
        }
    }

    /// <summary>
    /// Sets origin and referrer on a request.
    /// </summary>
    /// <param name="request">Request to configure.</param>
    /// <param name="origin">Request origin.</param>
    /// <param name="referrer">Request referrer.</param>
    public static void SetOriginAndReferrer(HttpRequestMessage request, string? origin, string? referrer)
    {
        if (origin != null)
            request.Headers.Add("origin", referrer ?? origin);
        if (referrer != null)
            request.Headers.Referrer = new(referrer);
        else if (origin != null)
            request.Headers.Referrer = new(origin);
    }

    /// <summary>
    /// Configures a JSON request.
    /// </summary>
    /// <param name="request">Request to configure.</param>
    public virtual void ConfigureJsonRequest(HttpRequestMessage request)
    {
        ConfigureHttpRequest(request);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
        request.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue("en-US"));
        request.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue("en", 0.9));
    }

    #endregion

    #endregion

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
    public async Task DownloadResourceAsync(string requestUri, Stream stream, string? origin = null, string? referrer = null, CancellationToken cancellationToken = default)
    {
        NotDisposed();
        HttpRequestMessage req = new(HttpMethod.Get, requestUri);
        SetOriginAndReferrer(req, origin, referrer);
        ConfigureHttpRequest(req);
        using HttpResponseMessage? fr = await HttpClient.SendAsync(req, cancellationToken).ConfigureAwait(false);
        fr.EnsureSuccessStatusCode();
        await fr.Content.CopyToAsync(stream, cancellationToken).ConfigureAwait(false);
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
    public async Task DownloadResourceAsync(string requestUri, ArtifactResourceKey key, string? origin = null, string? referrer = null, CancellationToken cancellationToken = default)
    {
        NotDisposed();
        HttpRequestMessage req = new(HttpMethod.Get, requestUri);
        SetOriginAndReferrer(req, origin, referrer);
        ConfigureHttpRequest(req);
        using HttpResponseMessage? fr = await HttpClient.SendAsync(req, cancellationToken).ConfigureAwait(false);
        fr.EnsureSuccessStatusCode();
        await using Stream stream = await CreateOutputStreamAsync(key, cancellationToken).ConfigureAwait(false);
        await fr.Content.CopyToAsync(stream, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Downloads a resource.
    /// </summary>
    /// <param name="requestUri">Uri to download from.</param>
    /// <param name="file">Target filename.</param>
    /// <param name="key">Artifact key.</param>
    /// <param name="path">File path to prepend.</param>
    /// <param name="inArtifactFolder">If false, place artifact under common root.</param>
    /// <param name="origin">Request origin.</param>
    /// <param name="referrer">Request referrer.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public Task DownloadResourceAsync(string requestUri, string file, ArtifactKey key, string? path = null, bool inArtifactFolder = true, string? origin = null, string? referrer = null, CancellationToken cancellationToken = default)
        => DownloadResourceAsync(requestUri, ArtifactResourceKey.Create(key, file, path, inArtifactFolder), cancellationToken: cancellationToken);

    /// <summary>
    /// Downloads a resource.
    /// </summary>
    /// <param name="requestUri"><see cref="Uri"/> to download from.</param>
    /// <param name="stream">Target stream.</param>
    /// <param name="origin">Request origin.</param>
    /// <param name="referrer">Request referrer.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public async Task DownloadResourceAsync(Uri requestUri, Stream stream, string? origin = null, string? referrer = null, CancellationToken cancellationToken = default)
    {
        NotDisposed();
        HttpRequestMessage req = new(HttpMethod.Get, requestUri);
        SetOriginAndReferrer(req, origin, referrer);
        ConfigureHttpRequest(req);
        using HttpResponseMessage? fr = await HttpClient.SendAsync(req, cancellationToken).ConfigureAwait(false);
        fr.EnsureSuccessStatusCode();
        await fr.Content.CopyToAsync(stream, cancellationToken).ConfigureAwait(false);
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
    public async Task DownloadResourceAsync(Uri requestUri, ArtifactResourceKey key, string? origin = null, string? referrer = null, CancellationToken cancellationToken = default)
    {
        NotDisposed();
        HttpRequestMessage req = new(HttpMethod.Get, requestUri);
        SetOriginAndReferrer(req, origin, referrer);
        ConfigureHttpRequest(req);
        using HttpResponseMessage? fr = await HttpClient.SendAsync(req, cancellationToken).ConfigureAwait(false);
        fr.EnsureSuccessStatusCode();
        await using Stream stream = await CreateOutputStreamAsync(key, cancellationToken).ConfigureAwait(false);
        await fr.Content.CopyToAsync(stream, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Downloads a resource.
    /// </summary>
    /// <param name="requestUri"><see cref="Uri"/> to download from.</param>
    /// <param name="file">Target filename.</param>
    /// <param name="key">Artifact key.</param>
    /// <param name="path">File path to prepend.</param>
    /// <param name="inArtifactFolder">If false, place artifact under common root.</param>
    /// <param name="origin">Request origin.</param>
    /// <param name="referrer">Request referrer.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public Task DownloadResourceAsync(Uri requestUri, string file, ArtifactKey key, string? path = null, bool inArtifactFolder = true, string? origin = null, string? referrer = null, CancellationToken cancellationToken = default)
        => DownloadResourceAsync(requestUri, ArtifactResourceKey.Create(key, file, path, inArtifactFolder), origin, referrer, cancellationToken);

    /// <summary>
    /// Downloads a resource.
    /// </summary>
    /// <param name="requestMessage">Request to send.</param>
    /// <param name="stream">Target stream.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public async Task DownloadResourceAsync(HttpRequestMessage requestMessage, Stream stream, CancellationToken cancellationToken = default)
    {
        NotDisposed();
        using HttpResponseMessage? fr = await HttpClient.SendAsync(requestMessage, cancellationToken).ConfigureAwait(false);
        fr.EnsureSuccessStatusCode();
        await fr.Content.CopyToAsync(stream, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Downloads a resource.
    /// </summary>
    /// <param name="requestMessage">Request to send.</param>
    /// <param name="key">Resource key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public async Task DownloadResourceAsync(HttpRequestMessage requestMessage, ArtifactResourceKey key, CancellationToken cancellationToken = default)
    {
        NotDisposed();
        using HttpResponseMessage? fr = await HttpClient.SendAsync(requestMessage, cancellationToken).ConfigureAwait(false);
        fr.EnsureSuccessStatusCode();
        await using Stream stream = await CreateOutputStreamAsync(key, cancellationToken).ConfigureAwait(false);
        await fr.Content.CopyToAsync(stream, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Downloads a resource.
    /// </summary>
    /// <param name="requestMessage">Request to send.</param>
    /// <param name="file">Target filename.</param>
    /// <param name="key">Artifact key.</param>
    /// <param name="path">File path to prepend.</param>
    /// <param name="inArtifactFolder">If false, place artifact under common root.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public Task DownloadResourceAsync(HttpRequestMessage requestMessage, string file, ArtifactKey key, string? path = null, bool inArtifactFolder = true, CancellationToken cancellationToken = default)
        => DownloadResourceAsync(requestMessage, ArtifactResourceKey.Create(key, file, path, inArtifactFolder), cancellationToken);

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
                _httpClient?.Dispose();
                _httpClientHandler?.Dispose();
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
