namespace Art.M3U;

/// <summary>
/// Represents a configuration of timeouts, delays, and retry values.
/// </summary>
/// <param name="Http500RetryDelay">Retry delay for HTTP 500 status code.</param>
/// <param name="Http503RetryDelay">Retry delay for HTTP 503 status code.</param>
/// <param name="RequestTimeout">Timeout for requests.</param>
/// <param name="RequestTimeoutRetries">Number of consecutive retries before attempting recovery.</param>
public record struct M3UTiming(
    TimeSpan? Http500RetryDelay,
    TimeSpan? Http503RetryDelay,
    TimeSpan? RequestTimeout,
    int? RequestTimeoutRetries)
{
    /// <summary>
    /// Default configuration of <see cref="M3UTiming"/>.
    /// </summary>
    public static readonly M3UTiming Default = new(
        Http500RetryDelay: TimeSpan.FromSeconds(10),
        Http503RetryDelay: TimeSpan.FromSeconds(10),
        RequestTimeout: TimeSpan.FromSeconds(5),
        RequestTimeoutRetries: 10);
}
