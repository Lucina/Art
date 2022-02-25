using Art.Management;

namespace Art;

/// <summary>
/// Runtime configuration for a <see cref="ArtifactTool"/>.
/// </summary>
/// <param name="RegistrationManager">Registration manager.</param>
/// <param name="DataManager">Data manager.</param>
/// <param name="FailureFlags">Flags indicating errors to ignore.</param>
public record ArtifactToolConfig(ArtifactRegistrationManager RegistrationManager, ArtifactDataManager DataManager, FailureFlags FailureFlags)
{
    /// <summary>
    /// Shared default instance.
    /// </summary>
    public static readonly ArtifactToolConfig Default = new(NullArtifactRegistrationManager.Instance, NullArtifactDataManager.Instance, FailureFlags.None);
}
