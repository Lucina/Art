namespace Art.M3U;

/// <summary>
/// Represents configuration for downloader.
/// </summary>
/// <param name="URL">Base stream URL.</param>
/// <param name="ArtifactKey">Base artifact key.</param>
/// <param name="SkipExistingSegments">Skip registered segments.</param>
/// <param name="Decrypt">Decrypt data inline.</param>
/// <param name="PrioritizeResolution">Prioritize resolution in stream selection.</param>
/// <param name="MaxFails">Maximum allowed consecutive failures.</param>
/// <param name="Referrer">Stream download referrer.</param>
/// <param name="Origin">Stream download origin.</param>
/// <param name="Headers">Headers to add to each request.</param>
/// <param name="RequestTimeout">Timeout for requests, in milliseconds.</param>
/// <param name="RequestTimeoutRetries">Number of consecutive retries before attempting recovery.</param>
public record M3UDownloaderConfig(
    string URL,
    ArtifactKey ArtifactKey,
    bool SkipExistingSegments = true,
    bool Decrypt = false,
    bool PrioritizeResolution = false,
    int MaxFails = 1,
    string? Referrer = null,
    string? Origin = null,
    IReadOnlyCollection<KeyValuePair<string, string>>? Headers = null,
    int RequestTimeout = 5000,
    int RequestTimeoutRetries = 10);
