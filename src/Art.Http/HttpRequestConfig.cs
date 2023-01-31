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
#if NET5_0_OR_GREATER
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
                    // HttpClient can timeout, on .NET 5 this will be InnerException TimeoutException
                    // Prioritize the passed cancellation token
                    cancellationToken.ThrowIfCancellationRequested();
                    // Cancel from local timeout if applicable, using same TaskCanceledException->TimeoutException semantics as HttpClient
                    try
                    {
                        ThrowForTimeout(timeout, localCancellationToken);
                    }
                    catch (Exception e)
                    {
                        throw new TaskCanceledException("An HTTP request timed out.", e);
                    }
                    // Fallback to the inner exception
                    throw;
                }
            }
            return await httpClient.SendAsync(httpRequestMessage, httpRequestConfig.HttpCompletionOption ?? defaultCompletionOption, cancellationToken).ConfigureAwait(false);
        }
        return await httpClient.SendAsync(httpRequestMessage, defaultCompletionOption, cancellationToken).ConfigureAwait(false);
    }
#endif

    private static void ThrowForTimeout(TimeSpan timeSpan, CancellationToken cancellationToken)
    {
        // This has to throw TimeoutException, for matching TaskCanceledException->TimeoutException
        if (cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException($"A request timed out after {timeSpan.TotalSeconds} seconds.");
        }
    }
}
