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
public record M3UDownloaderConfig(string URL, ArtifactKey ArtifactKey, bool SkipExistingSegments = true, bool Decrypt = false, bool PrioritizeResolution = false, int MaxFails = 1, string? Referrer = null);
