namespace Art.Common;

/// <summary>
/// Flags for item state.
/// </summary>
[Flags]
public enum ItemStateFlags
{
    /// <summary>
    /// No flags.
    /// </summary>
    None = 0,
    /// <summary>
    /// Treat this item as different even if existing data is the same.
    /// </summary>
    EnforceNew = 1 << 0,
    /// <summary>
    /// Completely new item.
    /// </summary>
    New = 1 << 1,
    /// <summary>
    /// Older date than other.
    /// </summary>
    OlderDate = 1 << 2,
    /// <summary>
    /// Newer date than other.
    /// </summary>
    NewerDate = 1 << 3,
    /// <summary>
    /// Changed version marker.
    /// </summary>
    ChangedVersion = 1 << 4,
    /// <summary>
    /// Changed non-identifying metadata (e.g. content-type).
    /// </summary>
    ChangedMetadata = 1 << 5,
    /// <summary>
    /// New (nonnull) checksum detected.
    /// </summary>
    NewChecksum = 1 << 6,
    /// <summary>
    /// Mask for different item identity (date, version).
    /// </summary>
    DifferentIdentityMask = EnforceNew | New | OlderDate | NewerDate | ChangedVersion | NewChecksum,
    /// <summary>
    /// Mask for newer item identity (date, version).
    /// </summary>
    NewerIdentityMask = EnforceNew | New | NewerDate | ChangedVersion | NewChecksum,
    /// <summary>
    /// Mask for any difference at all.
    /// </summary>
    DifferentMask = EnforceNew | New | OlderDate | NewerDate | ChangedVersion | ChangedMetadata | NewChecksum
}
