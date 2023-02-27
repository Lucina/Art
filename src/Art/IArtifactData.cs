namespace Art;

/// <summary>
/// Stores data relevant to an artifact.
/// </summary>
public interface IArtifactData : IReadOnlyDictionary<ArtifactResourceKey, ArtifactResourceInfo>
{
    /// <summary>
    /// Info for this artifact.
    /// </summary>
    ArtifactInfo Info { get; }

    /// <summary>
    /// Primary resource stream for this artifact, if one is appropriate.
    /// </summary>
    /// <remarks>
    /// This corresponds to the primary media for this artifact.
    /// The resource should be one considered significantly more important than any other resource
    /// under this artifact, such as an audiovisual stream. Note that this member precludes marking
    /// multiple resource streams as primary.
    /// <br/>
    /// The resource should have
    /// <see cref="ArtifactResourceInfo.CanGetStream"/> or <see cref="ArtifactResourceInfo.CanExportStream"/>
    /// set to true.
    /// </remarks>
    ArtifactResourceInfo? PrimaryResource { get; }
}
