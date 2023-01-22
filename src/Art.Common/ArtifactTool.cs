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

    /// <summary>
    /// Prepares a tool for the specified profile.
    /// </summary>
    /// <param name="artifactToolProfile">Tool profile.</param>
    /// <param name="artifactRegistrationManager">Registration manager.</param>
    /// <param name="artifactDataManager">Data manager.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    /// <exception cref="ArgumentException">Thrown when an invalid profile is provided.</exception>
    /// <exception cref="ArtifactToolNotFoundException">Thrown when tool is not found.</exception>
    public static async Task<ArtifactToolBase> PrepareToolAsync(ArtifactToolProfile artifactToolProfile, ArtifactRegistrationManagerBase artifactRegistrationManager, ArtifactDataManagerBase artifactDataManager, CancellationToken cancellationToken = default)
    {
        if (artifactToolProfile.Group == null) throw new ArgumentException("Group not specified in profile");
        if (!ArtifactToolLoader.TryLoad(artifactToolProfile, out ArtifactToolBase? t))
            throw new ArtifactToolNotFoundException(artifactToolProfile.Tool);
        ArtifactToolConfig config = new(artifactRegistrationManager, artifactDataManager);
        artifactToolProfile = artifactToolProfile.WithCoreTool(t);
        await t.InitializeAsync(config, artifactToolProfile, cancellationToken).ConfigureAwait(false);
        return t;
    }
}
