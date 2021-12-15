namespace Art;

/// <summary>
/// Flags for eager evaluation.
/// </summary>
[Flags]
public enum EagerFlags
{
    /// <summary>
    /// Disable all eager evaluations.
    /// </summary>
    None = 0,
    /// <summary>
    /// Eager artifact listing.
    /// </summary>
    ArtifactList = 1 << 0,
    /// <summary>
    /// Eager artifact dumping.
    /// </summary>
    ArtifactDump = 1 << 1,
    /// <summary>
    /// Eager resource metadata querying.
    /// </summary>
    ResourceMetadata = 1 << 2,
    /// <summary>
    /// Eager resource obtain.
    /// </summary>
    ResourceObtain = 1 << 3,
    /// <summary>
    /// All eager evaluations on.
    /// </summary>
    All = ArtifactList | ArtifactDump | ResourceMetadata | ResourceObtain
}
