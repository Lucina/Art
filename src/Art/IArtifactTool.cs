using System.Text.Json;

namespace Art;

/// <summary>
/// Base interface for artifact tools.
/// </summary>
public interface IArtifactTool : IDisposable
{
    /// <summary>
    /// True if this tool is in debug mode.
    /// </summary>
    bool DebugMode { get; set; }

    /// <summary>
    /// Log handler for this tool.
    /// </summary>
    IToolLogHandler? LogHandler { get; set; }

    /// <summary>
    /// Origin tool profile.
    /// </summary>
    public ArtifactToolProfile Profile { get; }

    /// <summary>
    /// Configuration.
    /// </summary>
    public ArtifactToolConfig Config { get; }

    /// <summary>
    /// Allowed eager evaluation modes for this tool.
    /// </summary>
    EagerFlags AllowedEagerModes { get; }

    /// <summary>
    /// Registration manager used by this instance.
    /// </summary>
    IArtifactRegistrationManager RegistrationManager { get; set; }

    /// <summary>
    /// Data manager used by this instance.
    /// </summary>
    IArtifactDataManager DataManager { get; set; }

    /// <summary>
    /// JSON serialization defaults.
    /// </summary>
    public JsonSerializerOptions JsonOptions { get; set; }

    /// <summary>
    /// Initializes and configures this tool with the specified runtime configuration and profile.
    /// </summary>
    /// <param name="config">Configuration.</param>
    /// <param name="profile">Profile.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    Task InitializeAsync(ArtifactToolConfig? config = null, ArtifactToolProfile? profile = null, CancellationToken cancellationToken = default);
}
