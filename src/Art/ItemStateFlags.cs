namespace Art;

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
    EnforceNew = 1 << 1,
    /// <summary>
    /// Completely new item.
    /// </summary>
    New = 1 << 2,
    /// <summary>
    /// Older date than other.
    /// </summary>
    OlderDate = 1 << 3,
    /// <summary>
    /// Newer date than other.
    /// </summary>
    NewerDate = 1 << 4,
    /// <summary>
    /// Changed version marker.
    /// </summary>
    ChangedVersion = 1 << 5,
    /// <summary>
    /// Changed non-identifying metadata (e.g. content-type).
    /// </summary>
    ChangedMetadata = 1 << 6,
    /// <summary>
    /// Mask for different item identity (date, version).
    /// </summary>
    DifferentIdentityMask = EnforceNew | New | OlderDate | NewerDate | ChangedVersion,
    /// <summary>
    /// Mask for newer item identity (date, version).
    /// </summary>
    NewerIdentityMask = EnforceNew | New | NewerDate | ChangedVersion,
    /// <summary>
    /// Mask for any difference at all.
    /// </summary>
    DifferentMask = EnforceNew | New | OlderDate | NewerDate | ChangedVersion | ChangedMetadata
}
