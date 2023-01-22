namespace Art;

/// <summary>
/// Runtime configuration for a <see cref="ArtifactToolBase"/>.
/// </summary>
/// <param name="RegistrationManager">Registration manager.</param>
/// <param name="DataManager">Data manager.</param>
public record ArtifactToolConfig(ArtifactRegistrationManager RegistrationManager, ArtifactDataManager DataManager)
{
    /// <summary>
    /// Shared default instance.
    /// </summary>
    public static readonly ArtifactToolConfig Default = new(HiddenNullArtifactRegistrationManager.Instance, HiddenNullArtifactDataManager.Instance);
}
