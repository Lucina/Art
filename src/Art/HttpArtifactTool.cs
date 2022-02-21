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

    #region Http client configuraiton

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
    /// <returns>Task returning response (status left unchecked).</returns>
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
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
    /// <returns>Task returning response (status left unchecked).</returns>
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
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
    /// <returns>Task returning response (status left unchecked).</returns>
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    public async Task<HttpResponseMessage> GetAsync(string requestUri, string? origin = null, string? referrer = null, CancellationToken cancellationToken = default)
    {
        NotDisposed();
        HttpRequestMessage req = new(HttpMethod.Get, requestUri);
        SetOriginAndReferrer(req, origin, referrer);
        ConfigureHttpRequest(req);
        return await HttpClient.SendAsync(req, GenericCompletionOption, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends an HTTP GET request.
    /// </summary>
    /// <param name="requestUri">Request.</param>
    /// <param name="origin">Request origin.</param>
    /// <param name="referrer">Request referrer.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning response (status left unchecked).</returns>
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    public async Task<HttpResponseMessage> GetAsync(Uri requestUri, string? origin = null, string? referrer = null, CancellationToken cancellationToken = default)
    {
        NotDisposed();
        HttpRequestMessage req = new(HttpMethod.Get, requestUri);
        SetOriginAndReferrer(req, origin, referrer);
        ConfigureHttpRequest(req);
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
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    /// <exception cref="AggregateException">Thrown with <see cref="HttpRequestException"/> and <see cref="ExHttpResponseMessageException"/> on HTTP error.</exception>
    /// <remarks>
    /// This overload uses <see cref="ArtifactTool.JsonOptions"/> member automatically.
    /// </remarks>
    public async Task<T?> GetDeserializedJsonAsync<T>(string requestUri, string? origin = null, string? referrer = null, CancellationToken cancellationToken = default)
    {
        NotDisposed();
        HttpRequestMessage req = new(HttpMethod.Get, requestUri);
        SetOriginAndReferrer(req, origin, referrer);
        ConfigureJsonRequest(req);
        using HttpResponseMessage res = await HttpClient.SendAsync(req, JsonCompletionOption, cancellationToken).ConfigureAwait(false);
        ExHttpResponseMessageException.EnsureSuccessStatusCode(res);
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
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    /// <exception cref="AggregateException">Thrown with <see cref="HttpRequestException"/> and <see cref="ExHttpResponseMessageException"/> on HTTP error.</exception>
    /// <remarks>
    /// This overload uses <see cref="ArtifactTool.JsonOptions"/> member automatically.
    /// </remarks>
    public async Task<T> GetDeserializedRequiredJsonAsync<T>(string requestUri, string? origin = null, string? referrer = null, CancellationToken cancellationToken = default)
    {
        return await GetDeserializedJsonAsync<T>(requestUri, origin, referrer, cancellationToken).ConfigureAwait(false) ?? throw new NullJsonDataException();
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
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    /// <exception cref="AggregateException">Thrown with <see cref="HttpRequestException"/> and <see cref="ExHttpResponseMessageException"/> on HTTP error.</exception>
    public async Task<T?> GetDeserializedJsonAsync<T>(string requestUri, JsonSerializerOptions? jsonSerializerOptions, string? origin = null, string? referrer = null, CancellationToken cancellationToken = default)
    {
        NotDisposed();
        HttpRequestMessage req = new(HttpMethod.Get, requestUri);
        SetOriginAndReferrer(req, origin, referrer);
        ConfigureJsonRequest(req);
        using HttpResponseMessage res = await HttpClient.SendAsync(req, JsonCompletionOption, cancellationToken).ConfigureAwait(false);
        ExHttpResponseMessageException.EnsureSuccessStatusCode(res);
        return await DeserializeJsonWithDebugAsync<T>(res, jsonSerializerOptions, cancellationToken).ConfigureAwait(false);
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
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    /// <exception cref="AggregateException">Thrown with <see cref="HttpRequestException"/> and <see cref="ExHttpResponseMessageException"/> on HTTP error.</exception>
    public async Task<T> GetDeserializedRequiredJsonAsync<T>(string requestUri, JsonSerializerOptions? jsonSerializerOptions, string? origin = null, string? referrer = null, CancellationToken cancellationToken = default)
    {
        return await GetDeserializedJsonAsync<T>(requestUri, jsonSerializerOptions, origin, referrer, cancellationToken).ConfigureAwait(false) ?? throw new NullJsonDataException();
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
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    /// <exception cref="AggregateException">Thrown with <see cref="HttpRequestException"/> and <see cref="ExHttpResponseMessageException"/> on HTTP error.</exception>
    /// <remarks>
    /// This overload uses <see cref="ArtifactTool.JsonOptions"/> member automatically.
    /// </remarks>
    public async Task<T?> GetDeserializedJsonAsync<T>(Uri requestUri, string? origin = null, string? referrer = null, CancellationToken cancellationToken = default)
    {
        NotDisposed();
        HttpRequestMessage req = new(HttpMethod.Get, requestUri);
        SetOriginAndReferrer(req, origin, referrer);
        ConfigureJsonRequest(req);
        using HttpResponseMessage res = await HttpClient.SendAsync(req, JsonCompletionOption, cancellationToken).ConfigureAwait(false);
        ExHttpResponseMessageException.EnsureSuccessStatusCode(res);
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
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    /// <exception cref="AggregateException">Thrown with <see cref="HttpRequestException"/> and <see cref="ExHttpResponseMessageException"/> on HTTP error.</exception>
    /// <remarks>
    /// This overload uses <see cref="ArtifactTool.JsonOptions"/> member automatically.
    /// </remarks>
    public async Task<T> GetDeserializedRequiredJsonAsync<T>(Uri requestUri, string? origin = null, string? referrer = null, CancellationToken cancellationToken = default)
    {
        return await GetDeserializedJsonAsync<T>(requestUri, origin, referrer, cancellationToken).ConfigureAwait(false) ?? throw new NullJsonDataException();
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
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    /// <exception cref="AggregateException">Thrown with <see cref="HttpRequestException"/> and <see cref="ExHttpResponseMessageException"/> on HTTP error.</exception>
    public async Task<T?> GetDeserializedJsonAsync<T>(Uri requestUri, JsonSerializerOptions? jsonSerializerOptions, string? origin = null, string? referrer = null, CancellationToken cancellationToken = default)
    {
        NotDisposed();
        HttpRequestMessage req = new(HttpMethod.Get, requestUri);
        SetOriginAndReferrer(req, origin, referrer);
        ConfigureJsonRequest(req);
        using HttpResponseMessage res = await HttpClient.SendAsync(req, JsonCompletionOption, cancellationToken).ConfigureAwait(false);
        ExHttpResponseMessageException.EnsureSuccessStatusCode(res);
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
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    /// <exception cref="AggregateException">Thrown with <see cref="HttpRequestException"/> and <see cref="ExHttpResponseMessageException"/> on HTTP error.</exception>
    public async Task<T> GetDeserializedRequiredJsonAsync<T>(Uri requestUri, JsonSerializerOptions? jsonSerializerOptions, string? origin = null, string? referrer = null, CancellationToken cancellationToken = default)
    {
        return await GetDeserializedJsonAsync<T>(requestUri, jsonSerializerOptions, origin, referrer, cancellationToken).ConfigureAwait(false) ?? throw new NullJsonDataException();
    }

    /// <summary>
    /// Retrieves deserialized JSON using a <see cref="HttpRequestMessage"/>.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="requestMessage">Request to send.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning deserialized data.</returns>
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    /// <exception cref="AggregateException">Thrown with <see cref="HttpRequestException"/> and <see cref="ExHttpResponseMessageException"/> on HTTP error.</exception>
    /// <remarks>
    /// This overload uses <see cref="ArtifactTool.JsonOptions"/> member automatically.
    /// </remarks>
    public async Task<T?> RetrieveDeserializedJsonAsync<T>(HttpRequestMessage requestMessage, CancellationToken cancellationToken = default)
    {
        NotDisposed();
        using HttpResponseMessage res = await HttpClient.SendAsync(requestMessage, JsonCompletionOption, cancellationToken).ConfigureAwait(false);
        ExHttpResponseMessageException.EnsureSuccessStatusCode(res);
        return await DeserializeJsonAsync<T>(await res.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false), JsonOptions, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves deserialized JSON using a <see cref="HttpRequestMessage"/>.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="requestMessage">Request to send.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning deserialized data.</returns>
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    /// <exception cref="AggregateException">Thrown with <see cref="HttpRequestException"/> and <see cref="ExHttpResponseMessageException"/> on HTTP error.</exception>
    /// <remarks>
    /// This overload uses <see cref="ArtifactTool.JsonOptions"/> member automatically.
    /// </remarks>
    public async Task<T> RetrieveDeserializedRequiredJsonAsync<T>(HttpRequestMessage requestMessage, CancellationToken cancellationToken = default)
    {
        return await RetrieveDeserializedJsonAsync<T>(requestMessage, cancellationToken).ConfigureAwait(false) ?? throw new NullJsonDataException();
    }

    /// <summary>
    /// Retrieves deserialized JSON using a <see cref="HttpRequestMessage"/> and <see cref="JsonSerializerOptions"/>.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="requestMessage">Request to send.</param>
    /// <param name="jsonSerializerOptions">Optional deserialization options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning deserialized data.</returns>
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    /// <exception cref="AggregateException">Thrown with <see cref="HttpRequestException"/> and <see cref="ExHttpResponseMessageException"/> on HTTP error.</exception>
    public async Task<T?> RetrieveDeserializedJsonAsync<T>(HttpRequestMessage requestMessage, JsonSerializerOptions? jsonSerializerOptions, CancellationToken cancellationToken = default)
    {
        NotDisposed();
        using HttpResponseMessage res = await HttpClient.SendAsync(requestMessage, JsonCompletionOption, cancellationToken).ConfigureAwait(false);
        ExHttpResponseMessageException.EnsureSuccessStatusCode(res);
        return await DeserializeJsonAsync<T>(await res.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false), jsonSerializerOptions, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves deserialized JSON using a <see cref="HttpRequestMessage"/> and <see cref="JsonSerializerOptions"/>.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="requestMessage">Request to send.</param>
    /// <param name="jsonSerializerOptions">Optional deserialization options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning deserialized data.</returns>
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    /// <exception cref="AggregateException">Thrown with <see cref="HttpRequestException"/> and <see cref="ExHttpResponseMessageException"/> on HTTP error.</exception>
    public async Task<T> RetrieveDeserializedRequiredJsonAsync<T>(HttpRequestMessage requestMessage, JsonSerializerOptions? jsonSerializerOptions, CancellationToken cancellationToken = default)
    {
        return await RetrieveDeserializedJsonAsync<T>(requestMessage, jsonSerializerOptions, cancellationToken).ConfigureAwait(false) ?? throw new NullJsonDataException();
    }

    /// <summary>
    /// Deserialize JSON asynchronously, with debug output if <see cref="ArtifactTool.DebugMode"/> is enabled.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="response">Response to read from.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning value.</returns>
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    /// <exception cref="AggregateException">Thrown with <see cref="HttpRequestException"/> and <see cref="ExHttpResponseMessageException"/> on HTTP error.</exception>
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
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    /// <exception cref="AggregateException">Thrown with <see cref="HttpRequestException"/> and <see cref="ExHttpResponseMessageException"/> on HTTP error.</exception>
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
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    /// <exception cref="AggregateException">Thrown with <see cref="HttpRequestException"/> and <see cref="ExHttpResponseMessageException"/> on HTTP error.</exception>
    public async Task<T?> DeserializeJsonWithDebugAsync<T>(HttpResponseMessage response, JsonSerializerOptions? jsonSerializerOptions, CancellationToken cancellationToken = default)
    {
        ExHttpResponseMessageException.EnsureSuccessStatusCode(response);
        if (!DebugMode)
            return await DeserializeJsonAsync<T>(await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false), jsonSerializerOptions, cancellationToken).ConfigureAwait(false);
        string text = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        LogInformation($"JSON from {response.RequestMessage?.RequestUri?.ToString() ?? "unknown request"}", text);
        return DeserializeJson<T>(text, jsonSerializerOptions);
    }

    /// <summary>
    /// Deserialize JSON asynchronously, with debug output if <see cref="ArtifactTool.DebugMode"/> is enabled.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="response">Response to read from.</param>
    /// <param name="jsonSerializerOptions">Optional deserialization options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning value.</returns>
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    /// <exception cref="AggregateException">Thrown with <see cref="HttpRequestException"/> and <see cref="ExHttpResponseMessageException"/> on HTTP error.</exception>
    public async Task<T> DeserializeRequiredJsonWithDebugAsync<T>(HttpResponseMessage response, JsonSerializerOptions? jsonSerializerOptions, CancellationToken cancellationToken = default)
    {
        ExHttpResponseMessageException.EnsureSuccessStatusCode(response);
        if (!DebugMode)
            return await DeserializeRequiredJsonAsync<T>(await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false), jsonSerializerOptions, cancellationToken).ConfigureAwait(false);
        string text = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        LogInformation($"JSON from {response.RequestMessage?.RequestUri?.ToString() ?? "unknown request"}", text);
        return DeserializeRequiredJson<T>(text, jsonSerializerOptions);
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

    /// <summary>
    /// Default <see cref="HttpCompletionOption"/> for JSON requests.
    /// </summary>
    public virtual HttpCompletionOption JsonCompletionOption => HttpCompletionOption.ResponseContentRead;

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
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    /// <exception cref="AggregateException">Thrown with <see cref="HttpRequestException"/> and <see cref="ExHttpResponseMessageException"/> on HTTP error.</exception>
    public async Task DownloadResourceAsync(string requestUri, Stream stream, string? origin = null, string? referrer = null, CancellationToken cancellationToken = default)
    {
        NotDisposed();
        HttpRequestMessage req = new(HttpMethod.Get, requestUri);
        SetOriginAndReferrer(req, origin, referrer);
        ConfigureHttpRequest(req);
        using HttpResponseMessage res = await HttpClient.SendAsync(req, DownloadCompletionOption, cancellationToken).ConfigureAwait(false);
        ExHttpResponseMessageException.EnsureSuccessStatusCode(res);
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
    /// <exception cref="AggregateException">Thrown with <see cref="HttpRequestException"/> and <see cref="ExHttpResponseMessageException"/> on HTTP error.</exception>
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
    /// <exception cref="AggregateException">Thrown with <see cref="HttpRequestException"/> and <see cref="ExHttpResponseMessageException"/> on HTTP error.</exception>
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
    /// <exception cref="AggregateException">Thrown with <see cref="HttpRequestException"/> and <see cref="ExHttpResponseMessageException"/> on HTTP error.</exception>
    public async Task DownloadResourceAsync(Uri requestUri, Stream stream, string? origin = null, string? referrer = null, CancellationToken cancellationToken = default)
    {
        NotDisposed();
        HttpRequestMessage req = new(HttpMethod.Get, requestUri);
        SetOriginAndReferrer(req, origin, referrer);
        ConfigureHttpRequest(req);
        using HttpResponseMessage res = await HttpClient.SendAsync(req, DownloadCompletionOption, cancellationToken).ConfigureAwait(false);
        ExHttpResponseMessageException.EnsureSuccessStatusCode(res);
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
    /// <exception cref="AggregateException">Thrown with <see cref="HttpRequestException"/> and <see cref="ExHttpResponseMessageException"/> on HTTP error.</exception>
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
    /// <exception cref="AggregateException">Thrown with <see cref="HttpRequestException"/> and <see cref="ExHttpResponseMessageException"/> on HTTP error.</exception>
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
    /// <exception cref="AggregateException">Thrown with <see cref="HttpRequestException"/> and <see cref="ExHttpResponseMessageException"/> on HTTP error.</exception>
    public async Task DownloadResourceAsync(HttpRequestMessage requestMessage, Stream stream, CancellationToken cancellationToken = default)
    {
        NotDisposed();
        using HttpResponseMessage res = await HttpClient.SendAsync(requestMessage, DownloadCompletionOption, cancellationToken).ConfigureAwait(false);
        ExHttpResponseMessageException.EnsureSuccessStatusCode(res);
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
    /// <exception cref="AggregateException">Thrown with <see cref="HttpRequestException"/> and <see cref="ExHttpResponseMessageException"/> on HTTP error.</exception>
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
    /// <exception cref="AggregateException">Thrown with <see cref="HttpRequestException"/> and <see cref="ExHttpResponseMessageException"/> on HTTP error.</exception>
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
        ExHttpResponseMessageException.EnsureSuccessStatusCode(res);
        await StreamDownloadAsync(res, key, cancellationToken);
    }

    private async Task StreamDownloadAsync(HttpResponseMessage response, ArtifactResourceKey key, CancellationToken cancellationToken)
    {
        await using CommittableStream stream = await CreateOutputStreamAsync(key, cancellationToken).ConfigureAwait(false);
        await response.Content.CopyToAsync(stream, cancellationToken).ConfigureAwait(false);
        stream.ShouldCommit = true;
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
