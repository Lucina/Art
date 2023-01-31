namespace Art.Http;

/// <summary>
/// Configuration for a <see cref="HttpRequestMessage"/> sent via <see cref="HttpClient"/>.
/// </summary>
/// <param name="RequestAction">Custom configuration callback for the <see cref="HttpRequestMessage"/> created.</param>
/// <param name="HttpCompletionOption">Custom <see cref="System.Net.Http.HttpCompletionOption"/>.</param>
/// <param name="Timeout">Custom timeout to apply to request.</param>
public record HttpRequestConfig(
    string? Origin = null,
    string? Referrer = null,
    Action<HttpRequestMessage>? RequestAction = null,
    HttpCompletionOption? HttpCompletionOption = null,
    TimeSpan? Timeout = null)
{
    internal static async Task<HttpResponseMessage> SendConfiguredAsync(HttpRequestConfig? httpRequestConfig, HttpClient httpClient, HttpRequestMessage httpRequestMessage, HttpCompletionOption defaultCompletionOption, CancellationToken cancellationToken = default)
    {
        if (httpRequestConfig != null)
        {
            httpRequestMessage.SetOriginAndReferrer(httpRequestConfig.Origin, httpRequestConfig.Referrer);
            httpRequestConfig.RequestAction?.Invoke(httpRequestMessage);
            if (httpRequestConfig.Timeout is { } timeout)
            {
                using var cts = new CancellationTokenSource(timeout);
                using var lcts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                var localCancellationToken = lcts.Token;
                try
                {
                    return await httpClient.SendAsync(httpRequestMessage, httpRequestConfig.HttpCompletionOption ?? defaultCompletionOption, localCancellationToken).ConfigureAwait(false);
                }
                catch (TaskCanceledException)
                {
                    // Prioritize the passed cancellation token
                    cancellationToken.ThrowIfCancellationRequested();
                    // Cancel from local timeout if necessary, use same TaskCanceledException->TimeoutException semantics as HttpClient
                    if (localCancellationToken.IsCancellationRequested)
                    {
                        var timeoutException = new TimeoutException($"An HTTP request timed out based on a {nameof(HttpRequestConfig)} timeout of {timeout}.");
                        throw new TaskCanceledException("An HTTP request timed out.", timeoutException);
                    }
                    // Fallback to the other cancellation (could be HttpClient timeout with InnerException TimeoutException on .NET 5)
                    throw;
                }
            }
            return await httpClient.SendAsync(httpRequestMessage, httpRequestConfig.HttpCompletionOption ?? defaultCompletionOption, cancellationToken).ConfigureAwait(false);
        }
        return await httpClient.SendAsync(httpRequestMessage, defaultCompletionOption, cancellationToken).ConfigureAwait(false);
    }
}
