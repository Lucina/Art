namespace Art;

/// <summary>
/// Runtime configuration for a <see cref="ArtifactTool"/>.
/// </summary>
/// <param name="RegistrationManager">Registration manager.</param>
/// <param name="DataManager">Data manager.</param>
/// <param name="FailureBypassFlags">Flags indicating errors to ignore.</param>
public record ArtifactToolConfig(ArtifactRegistrationManager RegistrationManager, ArtifactDataManager DataManager, FailureBypassFlags FailureBypassFlags);
