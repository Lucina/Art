namespace Art;

/// <summary>
/// Modes for determining how resources are updated.
/// </summary>
public enum ResourceUpdateMode
{
    /// <summary>
    /// Update resource iff an artifact has been updated and a new version is detected.
    /// </summary>
    ArtifactSoft,
    /// <summary>
    /// Update resource iff an artifact has been updated.
    /// </summary>
    ArtifactHard,
    /// <summary>
    /// Always update resource information but do not retrieve resource itself (unless new artifact).
    /// </summary>
    Populate,
    /// <summary>
    /// Update resource if new version is detected.
    /// </summary>
    Soft,
    /// <summary>
    /// Always retrieve resource.
    /// </summary>
    Hard
}
