namespace Art;

/// <summary>
/// Pending <see cref="ArtifactResourceInfo"/> relative to a <see cref="ArtifactData"/>.
/// </summary>
public readonly record struct ArtifactDataResource(ArtifactData Data, ArtifactResourceInfo Info)
{
    /// <summary>
    /// Adds this resource.
    /// </summary>
    public void Commit() => Data.Add(Info);
}
