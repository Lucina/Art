namespace Art.Http;

/// <summary>
/// Provides utility functions.
/// </summary>
public static class ArtHttpExtensions
{
    /// <summary>
    /// Downloads a resource from a <see cref="HttpClient"/>.
    /// </summary>
    /// <param name="client">Web client instance.</param>
    /// <param name="url">URL to download from.</param>
    /// <param name="file">File path.</param>
    /// <param name="lengthCheck">Optional length check to skip download.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    /// <exception cref="HttpRequestException">Thrown for issues with request excluding non-success server responses.</exception>
    /// <exception cref="ArtHttpResponseMessageException">Thrown on HTTP response indicating non-successful response.</exception>
    public static async ValueTask DownloadResourceToFileAsync(this HttpClient client, string url, string file, long? lengthCheck = null, CancellationToken cancellationToken = default)
    {
        if (lengthCheck != null && File.Exists(file) && new FileInfo(file).Length == lengthCheck) return;
        using HttpResponseMessage fr = await client.GetAsync(url, cancellationToken);
        ArtHttpResponseMessageException.EnsureSuccessStatusCode(fr);
        await using FileStream fs = File.Create(file);
        await fr.Content.CopyToAsync(fs, cancellationToken);
    }

    /// <summary>
    /// Sets origin and referrer on a request.
    /// </summary>
    /// <param name="request">Request to configure.</param>
    /// <param name="origin">Request origin.</param>
    /// <param name="referrer">Request referrer.</param>
    public static void SetOriginAndReferrer(this HttpRequestMessage request, string? origin, string? referrer)
    {
        if (referrer != null)
            SetOriginAndReferrer(request, new Uri(referrer));
        else if (origin != null) SetOriginAndReferrer(request, new Uri(origin));
    }

    private static void SetOriginAndReferrer(HttpRequestMessage request, Uri uri)
    {
        request.Headers.Add("origin", uri.GetComponents(UriComponents.SchemeAndServer, UriFormat.UriEscaped));
        request.Headers.Referrer = uri;
    }
}
