namespace Art;

/// <summary>
/// Runtime configuration for a <see cref="IArtifactTool"/>.
/// </summary>
/// <param name="RegistrationManager">Registration manager.</param>
/// <param name="DataManager">Data manager.</param>
/// <param name="TimeProvider">Time provider.</param>
public record ArtifactToolConfig(IArtifactRegistrationManager RegistrationManager, IArtifactDataManager DataManager, TimeProvider TimeProvider);
