namespace Art;

/// <summary>
/// Represents a factory for <see cref="ArtifactDumper"/> instances.
/// </summary>
public abstract class ArtifactDumperFactory
{
    /// <summary>
    /// Creates a new <see cref="ArtifactDumper"/> based on the specified data manager and dumper profile.
    /// </summary>
    /// <param name="artifactRegistrationManager">Registration manager.</param>
    /// <param name="artifactDataManager">Data manager.</param>
    /// <param name="artifactDumpingProfile">Profile to use with the created artifact dumper.</param>
    /// <returns>Task that returns the created dumper.</returns>
    public abstract Task<ArtifactDumper> Create(ArtifactRegistrationManager artifactRegistrationManager, ArtifactDataManager artifactDataManager, ArtifactDumpingProfile artifactDumpingProfile);
}
