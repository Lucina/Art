using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Art;

/// <summary>
/// Represents an instance of an artifact tool.
/// </summary>
public abstract partial class ArtifactTool : IDisposable, IAsyncFinder<ArtifactData?>
{
    #region Fields

    /// <summary>
    /// Log handler for this tool.
    /// </summary>
    public ILogHandler? LogHandler;

    /// <summary>
    /// Option used to check if currently in debug mode.
    /// </summary>
    public const string OptDebugMode = "debugMode";

    /// <summary>
    /// Option used to check if currently using force (invalidating previous artifacts).
    /// </summary>
    public const string OptForce = "force";

    /// <summary>
    /// JSON serialization defaults.
    /// </summary>
    protected internal JsonSerializerOptions JsonOptions { get => _jsonOptions ??= new JsonSerializerOptions(); set => _jsonOptions = value; }

    /// <summary>
    /// Origin tool profile.
    /// </summary>
    protected ArtifactToolProfile Profile { get; private set; }

    /// <summary>
    /// True if this tool is in debug mode.
    /// </summary>
    protected bool DebugMode { get; set; }

    /// <summary>
    /// True if all previous artifacts should be ignored (e.g. modifies <see cref="IsNewArtifactAsync(ArtifactInfo)"/>).
    /// </summary>
    protected bool Force { get; set; }

    #endregion

    #region Private fields

    /// <summary>
    /// Registration manager used by this instance.
    /// </summary>
    private ArtifactRegistrationManager _registrationManager;

    /// <summary>
    /// Data manager used by this instance.
    /// </summary>
    private ArtifactDataManager _dataManager;

    private JsonSerializerOptions _jsonOptions = new();

    private bool _configured;

    private bool _disposed;

    private bool _runOverridden = true;

    private bool _runDataOverridden = true;

    #endregion

    #region Constructor

    /// <summary>
    /// Creates a new instance of <see cref="ArtifactTool"/>.
    /// </summary>
    protected ArtifactTool()
    {
        _registrationManager = null!;
        _dataManager = null!;
        Profile = null!;
    }

    #endregion

    #region Setup

    /// <summary>
    /// Initializes this tool with the specified runtime configuration.
    /// </summary>
    /// <param name="runtimeConfig">Runtime configuration.</param>
    public virtual Task ConfigureAsync(ArtifactToolRuntimeConfig runtimeConfig)
    {
        EnsureNotDisposed();
        if (runtimeConfig == null) throw new ArgumentNullException(nameof(runtimeConfig));
        if (runtimeConfig.RegistrationManager == null) throw new ArgumentException("Cannot configure with null registration manager");
        if (runtimeConfig.DataManager == null) throw new ArgumentException("Cannot configure with null data manager");
        if (runtimeConfig.Profile == null) throw new ArgumentException("Cannot configure with null profile");
        _registrationManager = runtimeConfig.RegistrationManager;
        _dataManager = runtimeConfig.DataManager;
        Profile = runtimeConfig.Profile;
        DebugMode = GetFlagTrue(OptDebugMode);
        Force = GetFlagTrue(OptForce);
        _configured = true;
        return Task.CompletedTask;
    }

    #endregion

    #region Options

    /// <summary>
    /// Attempt to get option or throw exception if not found or if null.
    /// </summary>
    /// <typeparam name="T">Value type.</typeparam>
    /// <param name="optKey">Key to search.</param>
    /// <param name="value">Value, if located and nonnull.</param>
    /// <exception cref="ArtifactToolOptionNotFoundException">Thrown when option is not found.</exception>
    /// <exception cref="JsonException">Thrown when conversion failed.</exception>
    /// <exception cref="NotSupportedException">Thrown when type not supported.</exception>
    protected void GetOptionOrExcept<T>(string optKey, out T value)
    {
        if (!(Profile.Options?.TryGetValue(optKey, out JsonElement vv) ?? false)) throw new ArtifactToolOptionNotFoundException(optKey);
        value = vv.Deserialize<T>(ArtJsonOptions.JsonOptions)!;
    }

    /// <summary>
    /// Attempt to get option.
    /// </summary>
    /// <typeparam name="T">Value type.</typeparam>
    /// <param name="optKey">Key to search.</param>
    /// <param name="value">Value, if located and nonnull.</param>
    /// <param name="throwIfIncorrectType">If true, throw a <see cref="JsonException"/> if type is wrong.</param>
    /// <returns>True if value is located and of the right type.</returns>
    protected bool TryGetOption<T>(string optKey, [NotNullWhen(true)] out T? value, bool throwIfIncorrectType = false)
    {
        if (Profile.Options?.TryGetValue(optKey, out JsonElement vv) ?? false)
        {
            try
            {
                value = vv.Deserialize<T>(ArtJsonOptions.JsonOptions)!;
                return true;
            }
            catch (JsonException)
            {
                if (throwIfIncorrectType) throw;
            }
        }
        value = default;
        return false;
    }

    /// <summary>
    /// Checks if a flag is true.
    /// </summary>
    /// <param name="optKey">Key to search.</param>
    /// <param name="throwIfIncorrectType">If true, throw a <see cref="JsonException"/> or <see cref="NotSupportedException"/> if type is wrong.</param>
    /// <returns>True if flag is set to true.</returns>
    protected bool GetFlagTrue(string optKey, bool throwIfIncorrectType = false)
        => TryGetOption(optKey, out bool? value, throwIfIncorrectType) && value.Value;

    #endregion

    #region Artifact management

    /// <summary>
    /// Creates a new instance of <see cref="ArtifactData"/>.
    /// </summary>
    /// <param name="id">Artifact ID.</param>
    /// <param name="date">Artifact creation date.</param>
    /// <param name="updateDate">Artifact update date.</param>
    /// <param name="properties">Artifact properties.</param>
    /// <param name="full">True if this is a full artifact.</param>
    public ArtifactData CreateData(string id, DateTimeOffset? date = null, DateTimeOffset? updateDate = null, IReadOnlyDictionary<string, JsonElement>? properties = null, bool full = true)
        => new(this, Profile.Tool, Profile.Group, id, date, updateDate, properties, full);

    /// <summary>
    /// Registers artifact as known.
    /// </summary>
    /// <param name="artifactInfo">Artifact to register.</param>
    /// <returns>Task.</returns>
    protected async ValueTask AddArtifactAsync(ArtifactInfo artifactInfo)
        => await _registrationManager.AddArtifactAsync(artifactInfo).ConfigureAwait(false);

    /// <summary>
    /// Registers artifact resource as known.
    /// </summary>
    /// <param name="artifactResourceInfo">Artifact resource to register.</param>
    /// <returns>Task.</returns>
    protected async ValueTask AddResourceAsync(ArtifactResourceInfo artifactResourceInfo)
        => await _registrationManager.AddResourceAsync(artifactResourceInfo).ConfigureAwait(false);

    /// <summary>
    /// Attempts to get info for the artifact with the specified ID.
    /// </summary>
    /// <param name="id">Artifact ID.</param>
    /// <returns>Task returning retrieved artifact, if it exists.</returns>
    protected async ValueTask<ArtifactInfo?> TryGetArtifactAsync(string id)
        => await _registrationManager.TryGetArtifactAsync(new ArtifactKey(Profile.Tool, Profile.Group, id)).ConfigureAwait(false);

    /// <summary>
    /// Tests if artifact is recognizably new.
    /// </summary>
    /// <param name="artifactInfo">Artifact to check.</param>
    /// <returns>Task returning true if this is a new artifact (newer than whatever exists with the same ID).</returns>
    protected async ValueTask<bool> IsNewArtifactAsync(ArtifactInfo artifactInfo)
        => await _registrationManager.IsNewArtifactAsync(artifactInfo).ConfigureAwait(false) || Force; // Forward to RegistrationManager even if forcing

    #endregion

    #region Outputs

    /// <summary>
    /// Outputs a text file for the specified artifact.
    /// </summary>
    /// <param name="text">Text to output.</param>
    /// <param name="file">Target filename.</param>
    /// <param name="artifactInfo">Artifact target.</param>
    /// <param name="path">File path to prepend.</param>
    /// <param name="inArtifactFolder">If false, place artifact under common root.</param>
    /// <returns>Task.</returns>
    protected async ValueTask OutputTextAsync(string text, string file, ArtifactInfo artifactInfo, string? path = null, bool inArtifactFolder = true)
    => await _dataManager.OutputTextAsync(text, file, artifactInfo, path, inArtifactFolder).ConfigureAwait(false);

    /// <summary>
    /// Outputs a JSON-serialized file for the specified artifact.
    /// </summary>
    /// <param name="data">Data to output.</param>
    /// <param name="file">Target filename.</param>
    /// <param name="artifactInfo">Artifact target.</param>
    /// <param name="path">File path to prepend.</param>
    /// <param name="inArtifactFolder">If false, place artifact under common root.</param>
    /// <returns>Task.</returns>
    protected async ValueTask OutputJsonAsync<T>(T data, string file, ArtifactInfo artifactInfo, string? path = null, bool inArtifactFolder = true)
        => await _dataManager.OutputJsonAsync<T>(data, JsonOptions, file, artifactInfo, path, inArtifactFolder).ConfigureAwait(false);

    /// <summary>
    /// Outputs a JSON-serialized file for the specified artifact.
    /// </summary>
    /// <param name="data">Data to output.</param>
    /// <param name="jsonSerializerOptions">Serialization options.</param>
    /// <param name="file">Target filename.</param>
    /// <param name="artifactInfo">Artifact target.</param>
    /// <param name="path">File path to prepend.</param>
    /// <param name="inArtifactFolder">If false, place artifact under common root.</param>
    /// <returns>Task.</returns>
    protected async ValueTask OutputJsonAsync<T>(T data, JsonSerializerOptions jsonSerializerOptions, string file, ArtifactInfo artifactInfo, string? path = null, bool inArtifactFolder = true)
        => await _dataManager.OutputJsonAsync<T>(data, jsonSerializerOptions, file, artifactInfo, path, inArtifactFolder).ConfigureAwait(false);

    /// <summary>
    /// Creates an output stream for a file for the specified artifact.
    /// </summary>
    /// <param name="file">Target filename.</param>
    /// <param name="artifactInfo">Artifact target.</param>
    /// <param name="path">File path to prepend.</param>
    /// <param name="inArtifactFolder">If false, place artifact under common root.</param>
    /// <returns>Task returning a writeable stream to write an output to.</returns>
    protected ValueTask<Stream> CreateOutputStreamAsync(string file, ArtifactInfo artifactInfo, string? path = null, bool inArtifactFolder = true)
        => _dataManager.CreateOutputStreamAsync(file, artifactInfo, path, inArtifactFolder);

    #endregion

    #region JSON



    /// <summary>
    /// Deserialize JSON from a UTF-8 stream.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="utf8Stream">UTF-8 encoded stream.</param>
    /// <returns>Value returning deserialized data.</returns>
    /// <remarks>
    /// This overload usees <see cref="JsonOptions"/> member automatically.
    /// </remarks>
    protected async ValueTask<T> DeserializeJsonAsync<T>(Stream utf8Stream)
        => (await JsonSerializer.DeserializeAsync<T>(utf8Stream, JsonOptions).ConfigureAwait(false))!;

    /// <summary>
    /// Deserialize JSON from a UTF-8 stream.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="utf8Stream">UTF-8 encoded stream.</param>
    /// <param name="jsonSerializerOptions">Optional deserialization options.</param>
    /// <returns>Value returning deserialized data.</returns>
    protected static async ValueTask<T> DeserializeJsonAsync<T>(Stream utf8Stream, JsonSerializerOptions? jsonSerializerOptions)
        => (await JsonSerializer.DeserializeAsync<T>(utf8Stream, jsonSerializerOptions).ConfigureAwait(false))!;

    /// <summary>
    /// Deserialize JSON from a string.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="str">String.</param>
    /// <returns>Value returning deserialized data.</returns>
    /// <remarks>
    /// This overload usees <see cref="JsonOptions"/> member automatically.
    /// </remarks>
    protected T DeserializeJson<T>(string str)
        => JsonSerializer.Deserialize<T>(str, JsonOptions)!;

    /// <summary>
    /// Deserialize JSON from a string.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="str">String.</param>
    /// <param name="jsonSerializerOptions">Optional deserialization options.</param>
    /// <returns>Value returning deserialized data.</returns>
    protected static T DeserializeJson<T>(string str, JsonSerializerOptions? jsonSerializerOptions)
        => JsonSerializer.Deserialize<T>(str, jsonSerializerOptions)!;

    #endregion

    #region Logging

    /// <summary>
    /// Logs information log to logger.
    /// </summary>
    /// <param name="title">Title.</param>
    /// <param name="body">Body.</param>
    protected void LogInformation(string? title, string? body = null) => LogHandler?.Log(title, body, LogLevel.Information);

    /// <summary>
    /// Logs title log to logger.
    /// </summary>
    /// <param name="title">Title.</param>
    /// <param name="body">Body.</param>
    protected void LogTitle(string? title, string? body = null) => LogHandler?.Log(title, body, LogLevel.Title);

    /// <summary>
    /// Logs warning log to logger.
    /// </summary>
    /// <param name="title">Title.</param>
    /// <param name="body">Body.</param>
    protected void LogWarning(string? title, string? body = null) => LogHandler?.Log(title, body, LogLevel.Warning);

    /// <summary>
    /// Logs error log to logger.
    /// </summary>
    /// <param name="title">Title.</param>
    /// <param name="body">Body.</param>
    protected void LogError(string? title, string? body = null) => LogHandler?.Log(title, body, LogLevel.Error);

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
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void EnsureState()
    {
        EnsureNotDisposed();
        if (!_configured) throw new InvalidOperationException("Tool has not been initialized");
    }

    private void EnsureNotDisposed()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(ArtifactTool));
    }

    #endregion
}
