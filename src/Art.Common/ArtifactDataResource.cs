namespace Art.Common;

/// <summary>
/// Pending <see cref="ArtifactResourceInfo"/> relative to a <see cref="ArtifactData"/>.
/// </summary>
public readonly record struct ArtifactDataResource(ArtifactData Data, ArtifactResourceInfo Info)
{
    /// <summary>
    /// Adds this resource.
    /// </summary>
    /// <param name="primary">If true, overwrite the primary resource on <see cref="Data"/>.</param>
    /// <seealso cref="IArtifactData.PrimaryResource">IArtifactData.PrimaryResource</seealso>
    public void Commit(bool primary = false)
    {
        Data.Add(Info);
        if (primary)
        {
            Data.PrimaryResource = Info;
        }
    }
}
