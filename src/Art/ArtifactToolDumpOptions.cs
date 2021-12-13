namespace Art;

/// <summary>
/// Represents runtime options for dump operation.
/// </summary>
/// <param name="ResourceUpdate">Resource update mode.</param>
/// <param name="IncludeNonFull">Overwrite full entries with non-full if newer.</param>
/// <param name="SkipMode">Skip mode.</param>
public record ArtifactToolDumpOptions(
    ResourceUpdateMode ResourceUpdate = ResourceUpdateMode.ArtifactHard,
    bool IncludeNonFull = true,
    ArtifactSkipMode SkipMode = ArtifactSkipMode.None)
{
    /// <summary>
    /// Default options.
    /// </summary>
    public static readonly ArtifactToolDumpOptions Default = new();
}
