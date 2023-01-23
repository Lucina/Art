﻿using System.Collections.Immutable;
using System.Text.Json;

namespace Art;

/// <summary>
/// Represents an instance of an artifact tool.
/// </summary>
public abstract partial class ArtifactToolBase : IArtifactTool
{
    #region Fields

    /// <inheritdoc />
    public IToolLogHandler? LogHandler { get; set; }

    /// <inheritdoc />
    public ArtifactToolProfile Profile { get; private set; }

    /// <inheritdoc />
    public ArtifactToolConfig Config { get; private set; }

    /// <inheritdoc />
    public virtual EagerFlags AllowedEagerModes => EagerFlags.None;

    /// <inheritdoc />
    public IArtifactRegistrationManager RegistrationManager { get; set; }

    /// <inheritdoc />
    public IArtifactDataManager DataManager { get; set; }

    /// <inheritdoc />
    public JsonSerializerOptions JsonOptions
    {
        get => _jsonOptions ??= new JsonSerializerOptions();
        set => _jsonOptions = value;
    }

    #endregion

    #region Private fields

    private JsonSerializerOptions? _jsonOptions;

    private bool _disposed;


    //private bool _initialized;

    #endregion

    #region Constructor

    /// <summary>
    /// Creates a new instance of <see cref="IArtifactTool"/>.
    /// </summary>
    protected ArtifactToolBase()
    {
        RegistrationManager = null!;
        DataManager = null!;
        Profile = null!;
        Config = null!;
    }

    #endregion

    #region Setup

    /// <inheritdoc />
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
        Profile = profile ?? new ArtifactToolProfile(ArtifactToolStringUtil.CreateToolString(GetType()), "default", ImmutableDictionary<string, JsonElement>.Empty);
        if (Profile.Options != null)
            ConfigureOptions();
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

    /// <summary>
    /// Configures options.
    /// </summary>
    /// <remarks>
    /// This method is called by <see cref="InitializeAsync"/> when <see cref="Profile"/>.<see cref="ArtifactToolProfile.Options"/> is not null.
    /// </remarks>
    public virtual void ConfigureOptions()
    {
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
        if (_disposed) throw new ObjectDisposedException(nameof(IArtifactTool));
    }

    #endregion
}
