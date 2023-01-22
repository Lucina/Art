namespace Art.Common;

/// <summary>
/// Common type for artifact tools.
/// </summary>
public partial class ArtifactTool : ArtifactToolBase
{
    private static readonly HashSet<string> s_yesLower = new() { "y", "yes", "" };

    private bool _delayFirstCalled;

    /// <summary>
    /// True if this tool is in debug mode.
    /// </summary>
    public bool DebugMode;

    /// <summary>
    /// Default delay time in seconds.
    /// </summary>
    public virtual double DelaySeconds => 0.25;
}
