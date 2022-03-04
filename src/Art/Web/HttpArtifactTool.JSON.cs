using System.Net.Http.Headers;
using System.Text.Json;

namespace Art.Web;

public partial class HttpArtifactTool
{
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
    /// <exception cref="ArtHttpResponseMessageException">Thrown on HTTP response indicating non-successful response.</exception>
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
        ArtHttpResponseMessageException.EnsureSuccessStatusCode(res);
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
    /// <exception cref="ArtHttpResponseMessageException">Thrown on HTTP response indicating non-successful response.</exception>
    /// <exception cref="NullJsonDataException">Thrown for null JSON value.</exception>
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
    /// <exception cref="ArtHttpResponseMessageException">Thrown on HTTP response indicating non-successful response.</exception>
    public async Task<T?> GetDeserializedJsonAsync<T>(string requestUri, JsonSerializerOptions? jsonSerializerOptions, string? origin = null, string? referrer = null, CancellationToken cancellationToken = default)
    {
        NotDisposed();
        HttpRequestMessage req = new(HttpMethod.Get, requestUri);
        SetOriginAndReferrer(req, origin, referrer);
        ConfigureJsonRequest(req);
        using HttpResponseMessage res = await HttpClient.SendAsync(req, JsonCompletionOption, cancellationToken).ConfigureAwait(false);
        ArtHttpResponseMessageException.EnsureSuccessStatusCode(res);
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
    /// <exception cref="ArtHttpResponseMessageException">Thrown on HTTP response indicating non-successful response.</exception>
    /// <exception cref="NullJsonDataException">Thrown for null JSON value.</exception>
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
    /// <exception cref="ArtHttpResponseMessageException">Thrown on HTTP response indicating non-successful response.</exception>
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
        ArtHttpResponseMessageException.EnsureSuccessStatusCode(res);
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
    /// <exception cref="ArtHttpResponseMessageException">Thrown on HTTP response indicating non-successful response.</exception>
    /// <exception cref="NullJsonDataException">Thrown for null JSON value.</exception>
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
    /// <exception cref="ArtHttpResponseMessageException">Thrown on HTTP response indicating non-successful response.</exception>
    public async Task<T?> GetDeserializedJsonAsync<T>(Uri requestUri, JsonSerializerOptions? jsonSerializerOptions, string? origin = null, string? referrer = null, CancellationToken cancellationToken = default)
    {
        NotDisposed();
        HttpRequestMessage req = new(HttpMethod.Get, requestUri);
        SetOriginAndReferrer(req, origin, referrer);
        ConfigureJsonRequest(req);
        using HttpResponseMessage res = await HttpClient.SendAsync(req, JsonCompletionOption, cancellationToken).ConfigureAwait(false);
        ArtHttpResponseMessageException.EnsureSuccessStatusCode(res);
        return await DeserializeJsonWithDebugAsync<T>(res, jsonSerializerOptions, cancellationToken).ConfigureAwait(false);
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
    /// <exception cref="ArtHttpResponseMessageException">Thrown on HTTP response indicating non-successful response.</exception>
    /// <exception cref="NullJsonDataException">Thrown for null JSON value.</exception>
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
    /// <exception cref="ArtHttpResponseMessageException">Thrown on HTTP response indicating non-successful response.</exception>
    /// <remarks>
    /// This overload uses <see cref="ArtifactTool.JsonOptions"/> member automatically.
    /// </remarks>
    public async Task<T?> RetrieveDeserializedJsonAsync<T>(HttpRequestMessage requestMessage, CancellationToken cancellationToken = default)
    {
        NotDisposed();
        using HttpResponseMessage res = await HttpClient.SendAsync(requestMessage, JsonCompletionOption, cancellationToken).ConfigureAwait(false);
        ArtHttpResponseMessageException.EnsureSuccessStatusCode(res);
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
    /// <exception cref="ArtHttpResponseMessageException">Thrown on HTTP response indicating non-successful response.</exception>
    /// <exception cref="NullJsonDataException">Thrown for null JSON value.</exception>
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
    /// <exception cref="ArtHttpResponseMessageException">Thrown on HTTP response indicating non-successful response.</exception>
    public async Task<T?> RetrieveDeserializedJsonAsync<T>(HttpRequestMessage requestMessage, JsonSerializerOptions? jsonSerializerOptions, CancellationToken cancellationToken = default)
    {
        NotDisposed();
        using HttpResponseMessage res = await HttpClient.SendAsync(requestMessage, JsonCompletionOption, cancellationToken).ConfigureAwait(false);
        ArtHttpResponseMessageException.EnsureSuccessStatusCode(res);
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
    /// <exception cref="ArtHttpResponseMessageException">Thrown on HTTP response indicating non-successful response.</exception>
    /// <exception cref="NullJsonDataException">Thrown for null JSON value.</exception>
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
    /// <exception cref="ArtHttpResponseMessageException">Thrown on HTTP response indicating non-successful response.</exception>
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
    /// <exception cref="ArtHttpResponseMessageException">Thrown on HTTP response indicating non-successful response.</exception>
    /// <exception cref="NullJsonDataException">Thrown for null JSON value.</exception>
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
    /// <exception cref="ArtHttpResponseMessageException">Thrown on HTTP response indicating non-successful response.</exception>
    public async Task<T?> DeserializeJsonWithDebugAsync<T>(HttpResponseMessage response, JsonSerializerOptions? jsonSerializerOptions, CancellationToken cancellationToken = default)
    {
        ArtHttpResponseMessageException.EnsureSuccessStatusCode(response);
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
    /// <exception cref="ArtHttpResponseMessageException">Thrown on HTTP response indicating non-successful response.</exception>
    /// <exception cref="NullJsonDataException">Thrown for null JSON value.</exception>
    public async Task<T> DeserializeRequiredJsonWithDebugAsync<T>(HttpResponseMessage response, JsonSerializerOptions? jsonSerializerOptions, CancellationToken cancellationToken = default)
    {
        ArtHttpResponseMessageException.EnsureSuccessStatusCode(response);
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
}
