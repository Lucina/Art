using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json;

namespace Art;

/// <summary>
/// Represents an instance of an artifact dumper that depends on an <see cref="System.Net.Http.HttpClient"/>.
/// </summary>
public abstract class HttpArtifactDumper : ArtifactDumper
{
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

    private HttpClient _httpClient;

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

    private HttpClientHandler _httpClientHandler;

    private bool _disposed;

    /// <summary>
    /// Creates a new instance of <see cref="HttpArtifactDumper"/>.
    /// </summary>
    /// <param name="registrationManager">Registration manager to use for this instance.</param>
    /// <param name="dataManager">Data manager to use for this instance.</param>
    /// <param name="artifactDumpingProfile">Origin dumping profile.</param>
    /// <param name="httpClientInstance">Optional existing http client instance to use.</param>
    /// <remarks>
    /// No configuration will be performed on the <see cref="System.Net.Http.HttpClient"/> if provided. However, derived constructors can access the <see cref="HttpClient"/> member for configuration.
    /// </remarks>
    protected HttpArtifactDumper(ArtifactRegistrationManager registrationManager, ArtifactDataManager dataManager, ArtifactDumpingProfile artifactDumpingProfile, HttpClientInstance? httpClientInstance = null)
        : base(registrationManager, dataManager, artifactDumpingProfile)
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

    /// <summary>
    /// Retrieve deserialized JSON using a uri.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="requestUri">Request URI.</param>
    /// <returns>Task.</returns>
    /// <remarks>
    /// This overload usees <see cref="JsonOptions"/> member automatically.
    /// </remarks>
    protected async ValueTask<T> GetDeserializedJsonAsync<T>(string requestUri)
    {
        NotDisposed();
        using HttpResponseMessage res = await HttpClient.GetAsync(requestUri).ConfigureAwait(false);
        res.EnsureSuccessStatusCode();
        return (await JsonSerializer.DeserializeAsync<T>(await res.Content.ReadAsStreamAsync().ConfigureAwait(false), JsonOptions).ConfigureAwait(false))!;
    }

    /// <summary>
    /// Retrieve deserialized JSON using a uri and <see cref="JsonSerializerOptions"/>.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="requestUri">Request URI.</param>
    /// <param name="jsonSerializerOptions">Deserialization options.</param>
    /// <returns>Task.</returns>
    protected async ValueTask<T> GetDeserializedJsonAsync<T>(string requestUri, JsonSerializerOptions jsonSerializerOptions)
    {
        NotDisposed();
        using HttpResponseMessage res = await HttpClient.GetAsync(requestUri).ConfigureAwait(false);
        res.EnsureSuccessStatusCode();
        return (await JsonSerializer.DeserializeAsync<T>(await res.Content.ReadAsStreamAsync().ConfigureAwait(false), jsonSerializerOptions).ConfigureAwait(false))!;
    }

    /// <summary>
    /// Retrieve deserialized JSON using a <see cref="Uri"/>.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="requestUri">Request URI.</param>
    /// <returns>Task.</returns>
    /// <remarks>
    /// This overload usees <see cref="JsonOptions"/> member automatically.
    /// </remarks>
    protected async ValueTask<T> GetDeserializedJsonAsync<T>(Uri requestUri)
    {
        NotDisposed();
        using HttpResponseMessage res = await HttpClient.GetAsync(requestUri).ConfigureAwait(false);
        res.EnsureSuccessStatusCode();
        return (await JsonSerializer.DeserializeAsync<T>(await res.Content.ReadAsStreamAsync().ConfigureAwait(false), JsonOptions).ConfigureAwait(false))!;
    }

    /// <summary>
    /// Retrieve deserialized JSON using a <see cref="Uri"/> and <see cref="JsonSerializerOptions"/>.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="requestUri">Request URI.</param>
    /// <param name="jsonSerializerOptions">Deserialization options.</param>
    /// <returns>Task.</returns>
    protected async ValueTask<T> GetDeserializedJsonAsync<T>(Uri requestUri, JsonSerializerOptions jsonSerializerOptions)
    {
        NotDisposed();
        using HttpResponseMessage res = await HttpClient.GetAsync(requestUri).ConfigureAwait(false);
        res.EnsureSuccessStatusCode();
        return (await JsonSerializer.DeserializeAsync<T>(await res.Content.ReadAsStreamAsync().ConfigureAwait(false), jsonSerializerOptions).ConfigureAwait(false))!;
    }

    /// <summary>
    /// Retrieve deserialized JSON using a <see cref="HttpRequestMessage"/>.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="requestMessage">Request to send.</param>
    /// <returns>Task.</returns>
    /// <remarks>
    /// This overload usees <see cref="JsonOptions"/> member automatically.
    /// </remarks>
    protected async ValueTask<T> RetrieveDeserializedJsonAsync<T>(HttpRequestMessage requestMessage)
    {
        NotDisposed();
        using HttpResponseMessage res = await HttpClient.SendAsync(requestMessage).ConfigureAwait(false);
        res.EnsureSuccessStatusCode();
        return (await JsonSerializer.DeserializeAsync<T>(await res.Content.ReadAsStreamAsync().ConfigureAwait(false), JsonOptions).ConfigureAwait(false))!;
    }

    /// <summary>
    /// Retrieve deserialized JSON using a <see cref="HttpRequestMessage"/> and <see cref="JsonSerializerOptions"/>.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="requestMessage">Request to send.</param>
    /// <param name="jsonSerializerOptions">Deserialization options.</param>
    /// <returns>Task.</returns>
    protected async ValueTask<T> RetrieveDeserializedJsonAsync<T>(HttpRequestMessage requestMessage, JsonSerializerOptions jsonSerializerOptions)
    {
        NotDisposed();
        using HttpResponseMessage res = await HttpClient.SendAsync(requestMessage).ConfigureAwait(false);
        res.EnsureSuccessStatusCode();
        return (await JsonSerializer.DeserializeAsync<T>(await res.Content.ReadAsStreamAsync().ConfigureAwait(false), jsonSerializerOptions).ConfigureAwait(false))!;
    }

    /// <summary>
    /// Downloads a resource.
    /// </summary>
    /// <param name="requestUri">Uri to download from.</param>
    /// <param name="file">Target filename.</param>
    /// <param name="artifactInfo">Target artifact.</param>
    /// <param name="path">File path to prepend.</param>
    /// <returns>Task.</returns>
    protected async ValueTask DownloadResourceAsync(string requestUri, string file, ArtifactInfo? artifactInfo = null, string? path = null)
    {
        NotDisposed();
        using HttpResponseMessage? fr = await HttpClient.GetAsync(requestUri).ConfigureAwait(false);
        fr.EnsureSuccessStatusCode();
        await using Stream stream = await DataManager.CreateOutputStreamAsync(file, artifactInfo, path).ConfigureAwait(false);
        await fr.Content.CopyToAsync(stream).ConfigureAwait(false);
    }

    /// <summary>
    /// Downloads a resource.
    /// </summary>
    /// <param name="requestUri"><see cref="Uri"/> to download from.</param>
    /// <param name="file">Target filename.</param>
    /// <param name="artifactInfo">Target artifact.</param>
    /// <param name="path">File path to prepend.</param>
    /// <returns>Task.</returns>
    protected async ValueTask DownloadResourceAsync(Uri requestUri, string file, ArtifactInfo? artifactInfo = null, string? path = null)
    {
        NotDisposed();
        using HttpResponseMessage? fr = await HttpClient.GetAsync(requestUri).ConfigureAwait(false);
        fr.EnsureSuccessStatusCode();
        await using Stream stream = await DataManager.CreateOutputStreamAsync(file, artifactInfo, path).ConfigureAwait(false);
        await fr.Content.CopyToAsync(stream).ConfigureAwait(false);
    }

    /// <summary>
    /// Downloads a resource.
    /// </summary>
    /// <param name="requestMessage">Request to send.</param>
    /// <param name="file">Target filename.</param>
    /// <param name="artifactInfo">Target artifact.</param>
    /// <param name="path">File path to prepend.</param>
    /// <returns>Task.</returns>
    protected async ValueTask DownloadResourceAsync(HttpRequestMessage requestMessage, string file, ArtifactInfo? artifactInfo = null, string? path = null)
    {
        NotDisposed();
        using HttpResponseMessage? fr = await HttpClient.SendAsync(requestMessage).ConfigureAwait(false);
        fr.EnsureSuccessStatusCode();
        await using Stream stream = await DataManager.CreateOutputStreamAsync(file, artifactInfo, path).ConfigureAwait(false);
        await fr.Content.CopyToAsync(stream).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
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
        if (_disposed) throw new ObjectDisposedException(nameof(HttpArtifactDumper));
    }

    /// <summary>
    /// <see cref="System.Net.Http.HttpClient"/> instance with associated <see cref="System.Net.Http.HttpClientHandler"/>.
    /// </summary>
    /// <param name="HttpClient">Client.</param>
    /// <param name="HttpClientHandler">Client handler.</param>
    public record HttpClientInstance(HttpClient HttpClient, HttpClientHandler HttpClientHandler);
}
