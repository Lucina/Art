using System.Globalization;
using System.Reflection;
using System.Text.Json;
using Art.Logging;
using Art.Management;

namespace Art;

/// <summary>
/// Represents an instance of an artifact tool.
/// </summary>
public abstract partial class ArtifactTool : IDisposable
{
    #region Fields

    /// <summary>
    /// Invariant culture.
    /// </summary>
    public static readonly CultureInfo IC = CultureInfo.InvariantCulture;

    /// <summary>
    /// Log handler for this tool.
    /// </summary>
    public IToolLogHandler? LogHandler;

    /// <summary>
    /// Option used to check if currently in debug mode.
    /// </summary>
    public const string OptDebugMode = "debugMode";

    /// <summary>
    /// JSON serialization defaults.
    /// </summary>
    public JsonSerializerOptions JsonOptions
    {
        get => _jsonOptions ??= new JsonSerializerOptions();
        set => _jsonOptions = value;
    }

    /// <summary>
    /// Origin tool profile.
    /// </summary>
    public ArtifactToolProfile Profile { get; private set; }

    /// <summary>
    /// True if this tool is in debug mode.
    /// </summary>
    public bool DebugMode;

    /// <summary>
    /// Configuration
    /// </summary>
    public ArtifactToolConfig Config { get; private set; }

    /// <summary>
    /// Default delay time in seconds.
    /// </summary>
    public virtual double DelaySeconds => 0.25;

    /// <summary>
    /// Allowed eager evaluation modes for this tool.
    /// </summary>
    public virtual EagerFlags AllowedEagerModes => EagerFlags.None;

    /// <summary>
    /// Registration manager used by this instance.
    /// </summary>
    public ArtifactRegistrationManager RegistrationManager;

    /// <summary>
    /// Data manager used by this instance.
    /// </summary>
    public ArtifactDataManager DataManager;

    #endregion

    #region Private fields

    private static readonly HashSet<string> s_yesLower = new HashSet<string> { "y", "yes", "" };

    private JsonSerializerOptions? _jsonOptions;

    private bool _disposed;

    private bool _delayFirstCalled;

    //private bool _initialized;

    #endregion

    #region Constructor

    /// <summary>
    /// Creates a new instance of <see cref="ArtifactTool"/>.
    /// </summary>
    protected ArtifactTool()
    {
        RegistrationManager = null!;
        DataManager = null!;
        Profile = null!;
        Config = null!;
    }

    #endregion

    #region Setup

    /// <summary>
    /// Initializes and configures this tool with the specified runtime configuration and profile.
    /// </summary>
    /// <param name="config">Configuration.</param>
    /// <param name="profile">Profile.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public async Task InitializeAsync(ArtifactToolConfig? config = null, ArtifactToolProfile? profile = null, CancellationToken cancellationToken = default)
    {
        EnsureNotDisposed();
        InitializeCore(config, profile);
        await ConfigureAsync(cancellationToken);
        //_initialized = true;
    }

    private void InitializeCore(ArtifactToolConfig? config, ArtifactToolProfile? profile)
    {
        config ??= ArtifactToolConfig.Default;
        if (config.RegistrationManager == null) throw new ArgumentException("Cannot configure with null registration manager");
        if (config.DataManager == null) throw new ArgumentException("Cannot configure with null data manager");
        Config = config;
        RegistrationManager = config.RegistrationManager;
        DataManager = config.DataManager;
        Profile = profile ?? ArtifactToolProfile.Create(GetType(), "default");
        GetFlag(OptDebugMode, ref DebugMode);
    }

    /// <summary>
    /// Initializes this tool.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    /// <remarks>This is called implicitly by <see cref="InitializeAsync"/> which performs first initialization for a given profile / config set.</remarks>
    public virtual Task ConfigureAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    #endregion

    #region IDisposable

    /// <summary>
    /// Disposes unmanaged resources.
    /// </summary>
    /// <param name="disposing">Is disposing.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            _disposed = true;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /*private void EnsureInitialized()
    {
        if (!_initialized) throw new InvalidOperationException("Instance has not been initialized");
    }*/

    private void EnsureNotDisposed()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(ArtifactTool));
    }

    #endregion

    #region Utility

    /// <summary>
    /// Creates a tool string for the specified tool.
    /// </summary>
    /// <param name="type">Tool type.</param>
    /// <returns>Tool string.</returns>
    public static string CreateCoreToolString(Type type)
    {
        Type? coreType = type;
        while (coreType != null && coreType.GetCustomAttribute<CoreAttribute>() == null) coreType = coreType.BaseType;
        return CreateToolString(coreType ?? type);
    }

    /// <summary>
    /// Creates a tool string for the specified tool.
    /// </summary>
    /// <typeparam name="TTool">Tool type.</typeparam>
    /// <returns>Tool string.</returns>
    public static string CreateCoreToolString<TTool>() where TTool : ArtifactTool
        => CreateCoreToolString(typeof(TTool));

    /// <summary>
    /// Creates a tool string for the specified tool.
    /// </summary>
    /// <param name="type">Tool type.</param>
    /// <returns>Tool string.</returns>
    public static string CreateToolString(Type type)
    {
        string assemblyName = type.Assembly.GetName().Name ?? throw new InvalidOperationException();
        string typeName = type.FullName ?? throw new InvalidOperationException();
        return $"{assemblyName}::{typeName}";
    }

    /// <summary>
    /// Creates a tool string for the specified tool.
    /// </summary>
    /// <typeparam name="TTool">Tool type.</typeparam>
    /// <returns>Tool string.</returns>
    public static string CreateToolString<TTool>() where TTool : ArtifactTool
        => CreateToolString(typeof(TTool));

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
    public static async Task<ArtifactTool> PrepareToolAsync(ArtifactToolProfile artifactToolProfile, ArtifactRegistrationManager artifactRegistrationManager, ArtifactDataManager artifactDataManager, CancellationToken cancellationToken = default)
    {
        if (artifactToolProfile.Group == null) throw new ArgumentException("Group not specified in profile");
        if (!ArtifactToolLoader.TryLoad(artifactToolProfile, out ArtifactTool? t))
            throw new ArtifactToolNotFoundException(artifactToolProfile.Tool);
        ArtifactToolConfig config = new(artifactRegistrationManager, artifactDataManager, FailureFlags.None);
        artifactToolProfile = artifactToolProfile.WithCoreTool(t);
        await t.InitializeAsync(config, artifactToolProfile, cancellationToken).ConfigureAwait(false);
        return t;
    }

    #endregion
}
