namespace Art.M3U;

/// <summary>
/// Settings per segment.
/// </summary>
/// <param name="Skip"></param>
/// <param name="DisableDecryption"></param>
public record SegmentSettings(bool Skip, bool DisableDecryption);
