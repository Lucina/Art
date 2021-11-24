namespace Art;

/// <summary>
/// Modes for determining how resources are updated.
/// </summary>
public enum ResourceUpdateMode
{
    /// <summary>
    /// Update resource iff an artifact has been updated.
    /// </summary>
    Artifact,
    /// <summary>
    /// Update resource version but do not retrieve resource.
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
