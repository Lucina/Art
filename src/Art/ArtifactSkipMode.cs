namespace Art;

/// <summary>
/// Represents artifact skip mode.
/// </summary>
public enum ArtifactSkipMode
{
    /// <summary>
    /// Never skip.
    /// </summary>
    NoSkip,
    /// <summary>
    /// Skip all artifacts starting from first known artifact.
    /// </summary>
    SkipAllFromFirstKnown,
    /// <summary>
    /// Skip only known artifacts.
    /// </summary>
    SkipKnown
}
