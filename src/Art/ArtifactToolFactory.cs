namespace Art;

/// <summary>
/// Represents a factory for <see cref="ArtifactTool"/> instances.
/// </summary>
public abstract class ArtifactToolFactory
{
    /// <summary>
    /// Creates a new <see cref="ArtifactTool"/> based on the specified data manager and tool profile.
    /// </summary>
    /// <param name="artifactRegistrationManager">Registration manager.</param>
    /// <param name="artifactDataManager">Data manager.</param>
    /// <param name="artifactToolProfile">Profile to use with the created artifact tool.</param>
    /// <returns>Task that returns the created tool.</returns>
    public abstract ValueTask<ArtifactTool> CreateAsync(ArtifactRegistrationManager artifactRegistrationManager, ArtifactDataManager artifactDataManager, ArtifactToolProfile artifactToolProfile);
}
