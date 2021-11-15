﻿using System.Diagnostics.CodeAnalysis;
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
    protected HttpClient HttpClient { get; set; }

    /// <summary>
    /// Creates a new instance of <see cref="HttpArtifactDumper"/>, with automatic configuration of a <see cref="System.Net.Http.HttpClient"/>.
    /// </summary>
    /// <param name="registrationManager">Registration manager to use for this instance.</param>
    /// <param name="dataManager">Data manager to use for this instance.</param>
    /// <param name="artifactDumpingProfile">Origin dumping profile.</param>
    /// <remarks>
    /// The <see cref="HttpClient"/> member will be preconfigured, including setup with a cookie file if specified and automatic response decompression.
    /// </remarks>
    protected HttpArtifactDumper(ArtifactRegistrationManager registrationManager, ArtifactDataManager dataManager, ArtifactDumpingProfile artifactDumpingProfile)
        : base(registrationManager, dataManager, artifactDumpingProfile)
    {
        CookieContainer cc = new();
        if (TryGetOption(OptCookieFile, out string? cookieFile))
            using (StreamReader? f = File.OpenText(cookieFile))
                cc.LoadCookieFile(f);
        HttpClientHandler hch = new()
        {
            AutomaticDecompression = DecompressionMethods.All,
            CookieContainer = cc
        };
        HttpClient = new(hch);
    }

    /// <summary>
    /// Creates a new instance of <see cref="HttpArtifactDumper"/>, with an existing <see cref="System.Net.Http.HttpClient"/> (no automatic configuration).
    /// </summary>
    /// <param name="registrationManager">Registration manager to use for this instance.</param>
    /// <param name="dataManager">Data manager to use for this instance.</param>
    /// <param name="artifactDumpingProfile">Origin dumping profile.</param>
    /// <param name="httpClient">Existing http client to use.</param>
    /// <remarks>
    /// No configuration will be performed on the <see cref="System.Net.Http.HttpClient"/>. However, derived constructors can access the <see cref="HttpClient"/> member for configuration.
    /// </remarks>
    protected HttpArtifactDumper(ArtifactRegistrationManager registrationManager, ArtifactDataManager dataManager, ArtifactDumpingProfile artifactDumpingProfile, HttpClient httpClient)
        : base(registrationManager, dataManager, artifactDumpingProfile)
    {
        HttpClient = httpClient;
    }

    /// <summary>
    /// Retrieve deserialized JSON using a uri.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="requestUri">Request URI.</param>
    /// <returns>Task.</returns>
    protected async ValueTask<T> GetDeserializedJsonAsync<T>(string requestUri)
    {
        using HttpResponseMessage res = await HttpClient.GetAsync(requestUri).ConfigureAwait(false);
        res.EnsureSuccessStatusCode();
        return (await JsonSerializer.DeserializeAsync<T>(await res.Content.ReadAsStreamAsync().ConfigureAwait(false)).ConfigureAwait(false))!;
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
    protected async ValueTask<T> GetDeserializedJsonAsync<T>(Uri requestUri)
    {
        using HttpResponseMessage res = await HttpClient.GetAsync(requestUri).ConfigureAwait(false);
        res.EnsureSuccessStatusCode();
        return (await JsonSerializer.DeserializeAsync<T>(await res.Content.ReadAsStreamAsync().ConfigureAwait(false)).ConfigureAwait(false))!;
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
    protected async ValueTask<T> RetrieveDeserializedJsonAsync<T>(HttpRequestMessage requestMessage)
    {
        using HttpResponseMessage res = await HttpClient.SendAsync(requestMessage).ConfigureAwait(false);
        res.EnsureSuccessStatusCode();
        return (await JsonSerializer.DeserializeAsync<T>(await res.Content.ReadAsStreamAsync().ConfigureAwait(false)).ConfigureAwait(false))!;
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
        using HttpResponseMessage? fr = await HttpClient.GetAsync(requestUri).ConfigureAwait(false);
        fr.EnsureSuccessStatusCode();
        using Stream stream = await DataManager.CreateOutputStreamAsync(file, artifactInfo, path).ConfigureAwait(false);
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
        using HttpResponseMessage? fr = await HttpClient.GetAsync(requestUri).ConfigureAwait(false);
        fr.EnsureSuccessStatusCode();
        using Stream stream = await DataManager.CreateOutputStreamAsync(file, artifactInfo, path).ConfigureAwait(false);
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
        using HttpResponseMessage? fr = await HttpClient.SendAsync(requestMessage).ConfigureAwait(false);
        fr.EnsureSuccessStatusCode();
        using Stream stream = await DataManager.CreateOutputStreamAsync(file, artifactInfo, path).ConfigureAwait(false);
        await fr.Content.CopyToAsync(stream).ConfigureAwait(false);
    }
}
