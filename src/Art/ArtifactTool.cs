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
    public IToolLogHandler? LogHandler;

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
    /// True if all previous artifacts should be ignored (e.g. modifies <see cref="IsNewArtifactAsync(ArtifactInfo, CancellationToken)"/>).
    /// </summary>
    protected bool Force { get; set; }

    /// <summary>
    /// Configuration
    /// </summary>
    protected ArtifactToolRuntimeConfig Config { get; private set; }

    /// <summary>
    /// Default delay time in seconds.
    /// </summary>
    protected virtual double DelaySeconds => 0.25;

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

    private bool _delayFirstCalled;

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
        Config = null!;
    }

    #endregion

    #region Setup

    /// <summary>
    /// Initializes this tool with the specified runtime configuration.
    /// </summary>
    /// <param name="runtimeConfig">Runtime configuration.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public virtual Task ConfigureAsync(ArtifactToolRuntimeConfig runtimeConfig, CancellationToken cancellationToken = default)
    {
        EnsureNotDisposed();
        if (runtimeConfig == null) throw new ArgumentNullException(nameof(runtimeConfig));
        if (runtimeConfig.RegistrationManager == null) throw new ArgumentException("Cannot configure with null registration manager");
        if (runtimeConfig.DataManager == null) throw new ArgumentException("Cannot configure with null data manager");
        if (runtimeConfig.Profile == null) throw new ArgumentException("Cannot configure with null profile");
        Config = runtimeConfig;
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
        value = vv.Deserialize<T>(ArtJsonOptions.s_jsonOptions)!;
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
                value = vv.Deserialize<T>(ArtJsonOptions.s_jsonOptions)!;
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

    /// <summary>
    /// Gets an string option from a string value, or take value from <see cref="ArtifactToolProfile.Group"/>.
    /// </summary>
    /// <param name="optKey">Key to search.</param>
    /// <returns>Option value.</returns>
    protected string GetStringOptionOrGroup(string optKey)
    {
        return TryGetOption(optKey, out string? optValue) ? optValue : Profile.Group;
    }

    /// <summary>
    /// Gets an Int64 option from a string or literal value, or parses value from <see cref="ArtifactToolProfile.Group"/>.
    /// </summary>
    /// <param name="optKey">Key to search.</param>
    /// <returns>Option value.</returns>
    protected long GetInt64OptionOrGroup(string optKey)
    {
        return TryGetInt64Option(optKey, out long? optValue) ? optValue.Value : long.Parse(Profile.Group);
    }

    /// <summary>
    /// Gets an UInt64 option from a string or literal value, or parses value from <see cref="ArtifactToolProfile.Group"/>.
    /// </summary>
    /// <param name="optKey">Key to search.</param>
    /// <returns>Option value.</returns>
    protected ulong GetUInt64OptionOrGroup(string optKey)
    {
        return TryGetUInt64Option(optKey, out ulong? optValue) ? optValue.Value : ulong.Parse(Profile.Group);
    }

    /// <summary>
    /// Attempts to get an Int64 option from a string or literal value.
    /// </summary>
    /// <param name="optKey">Key to search.</param>
    /// <param name="value">Value, if located and nonnull.</param>
    /// <returns>True if found.</returns>
    protected bool TryGetInt64Option(string optKey, [NotNullWhen(true)] out long? value)
    {
        if (TryGetOption(optKey, out value)) return true;
        if (TryGetOption(optKey, out string? valueStr) && long.TryParse(valueStr, out long valueParsed))
        {
            value = valueParsed;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Attempts to get an UInt64 option from a string or literal value.
    /// </summary>
    /// <param name="optKey">Key to search.</param>
    /// <param name="value">Value, if located and nonnull.</param>
    /// <returns>True if found.</returns>
    protected bool TryGetUInt64Option(string optKey, [NotNullWhen(true)] out ulong? value)
    {
        if (TryGetOption(optKey, out value)) return true;
        if (TryGetOption(optKey, out string? valueStr) && ulong.TryParse(valueStr, out ulong valueParsed))
        {
            value = valueParsed;
            return true;
        }
        return false;
    }

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
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    protected async ValueTask AddArtifactAsync(ArtifactInfo artifactInfo, CancellationToken cancellationToken = default)
        => await _registrationManager.AddArtifactAsync(artifactInfo, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Registers artifact resource as known.
    /// </summary>
    /// <param name="artifactResourceInfo">Artifact resource to register.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    protected async ValueTask AddResourceAsync(ArtifactResourceInfo artifactResourceInfo, CancellationToken cancellationToken = default)
        => await _registrationManager.AddResourceAsync(artifactResourceInfo, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Attempts to get info for the artifact with the specified ID.
    /// </summary>
    /// <param name="id">Artifact ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning retrieved artifact, if it exists.</returns>
    protected async ValueTask<ArtifactInfo?> TryGetArtifactAsync(string id, CancellationToken cancellationToken = default)
        => await _registrationManager.TryGetArtifactAsync(new ArtifactKey(Profile.Tool, Profile.Group, id), cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Attempts to get info for the artifact with the specified ID.
    /// </summary>
    /// <param name="key">Artifact key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning retrieved artifact, if it exists.</returns>
    protected async ValueTask<ArtifactInfo?> TryGetArtifactAsync(ArtifactKey key, CancellationToken cancellationToken = default)
        => await _registrationManager.TryGetArtifactAsync(key, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Attempts to get info for the resource with the specified key.
    /// </summary>
    /// <param name="key">Resource key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning retrieved resource, if it exists.</returns>
    protected async ValueTask<ArtifactResourceInfo?> TryGetResourceAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default)
        => await _registrationManager.TryGetResourceAsync(key, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Tests if artifact is recognizably new.
    /// </summary>
    /// <param name="artifactInfo">Artifact to check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning true if this is a new artifact (newer than whatever exists with the same ID).</returns>
    protected async ValueTask<bool> IsNewArtifactAsync(ArtifactInfo artifactInfo, CancellationToken cancellationToken = default)
        => await _registrationManager.IsNewArtifactAsync(artifactInfo, cancellationToken).ConfigureAwait(false) || Force; // Forward to RegistrationManager even if forcing

    #endregion

    #region Outputs

    /// <summary>
    /// Outputs a text file for the specified artifact.
    /// </summary>
    /// <param name="text">Text to output.</param>
    /// <param name="key">Resource key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    protected async ValueTask OutputTextAsync(string text, ArtifactResourceKey key, CancellationToken cancellationToken = default)
    => await _dataManager.OutputTextAsync(text, key, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Outputs a text file for the specified artifact.
    /// </summary>
    /// <param name="text">Text to output.</param>
    /// <param name="key">Artifact key.</param>
    /// <param name="file">Target filename.</param>
    /// <param name="path">File path to prepend.</param>
    /// <param name="inArtifactFolder">If false, place artifact under common root.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    protected async ValueTask OutputTextAsync(string text, ArtifactKey key, string file, string? path = null, bool inArtifactFolder = true, CancellationToken cancellationToken = default)
    => await OutputTextAsync(text, ArtifactResourceKey.Create(key, file, path, inArtifactFolder), cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Outputs a JSON-serialized file for the specified artifact.
    /// </summary>
    /// <param name="data">Data to output.</param>
    /// <param name="key">Resource key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    protected async ValueTask OutputJsonAsync<T>(T data, ArtifactResourceKey key, CancellationToken cancellationToken = default)
        => await _dataManager.OutputJsonAsync<T>(data, JsonOptions, key, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Outputs a JSON-serialized file for the specified artifact.
    /// </summary>
    /// <param name="data">Data to output.</param>
    /// <param name="key">Artifact key.</param>
    /// <param name="file">Target filename.</param>
    /// <param name="path">File path to prepend.</param>
    /// <param name="inArtifactFolder">If false, place artifact under common root.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    protected async ValueTask OutputJsonAsync<T>(T data, ArtifactKey key, string file, string? path = null, bool inArtifactFolder = true, CancellationToken cancellationToken = default)
        => await OutputJsonAsync<T>(data, JsonOptions, ArtifactResourceKey.Create(key, file, path, inArtifactFolder), cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Outputs a JSON-serialized file for the specified artifact.
    /// </summary>
    /// <param name="data">Data to output.</param>
    /// <param name="jsonSerializerOptions">Serialization options.</param>
    /// <param name="key">Resource key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    protected async ValueTask OutputJsonAsync<T>(T data, JsonSerializerOptions jsonSerializerOptions, ArtifactResourceKey key, CancellationToken cancellationToken = default)
        => await _dataManager.OutputJsonAsync<T>(data, jsonSerializerOptions, key, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Outputs a JSON-serialized file for the specified artifact.
    /// </summary>
    /// <param name="data">Data to output.</param>
    /// <param name="jsonSerializerOptions">Serialization options.</param>
    /// <param name="key">Artifact key.</param>
    /// <param name="file">Target filename.</param>
    /// <param name="path">File path to prepend.</param>
    /// <param name="inArtifactFolder">If false, place artifact under common root.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    protected async ValueTask OutputJsonAsync<T>(T data, JsonSerializerOptions jsonSerializerOptions, ArtifactKey key, string file, string? path = null, bool inArtifactFolder = true, CancellationToken cancellationToken = default)
        => await OutputJsonAsync<T>(data, jsonSerializerOptions, ArtifactResourceKey.Create(key, file, path, inArtifactFolder), cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Creates an output stream for a file for the specified artifact.
    /// </summary>
    /// <param name="key">Resource key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning a writeable stream to write an output to.</returns>
    protected ValueTask<Stream> CreateOutputStreamAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default)
        => _dataManager.CreateOutputStreamAsync(key, cancellationToken);

    /// <summary>
    /// Creates an output stream for a file for the specified artifact.
    /// </summary>
    /// <param name="key">Artifact key.</param>
    /// <param name="file">Target filename.</param>
    /// <param name="path">File path to prepend.</param>
    /// <param name="inArtifactFolder">If false, place artifact under common root.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning a writeable stream to write an output to.</returns>
    protected ValueTask<Stream> CreateOutputStreamAsync(ArtifactKey key, string file, string? path = null, bool inArtifactFolder = true, CancellationToken cancellationToken = default)
        => CreateOutputStreamAsync(ArtifactResourceKey.Create(key, file, path, inArtifactFolder), cancellationToken);

    #endregion

    #region JSON

    /// <summary>
    /// Deserialize JSON from a UTF-8 stream.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="utf8Stream">UTF-8 encoded stream.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Value returning deserialized data.</returns>
    /// <remarks>
    /// This overload usees <see cref="JsonOptions"/> member automatically.
    /// </remarks>
    protected async ValueTask<T> DeserializeJsonAsync<T>(Stream utf8Stream, CancellationToken cancellationToken = default)
        => (await JsonSerializer.DeserializeAsync<T>(utf8Stream, JsonOptions, cancellationToken).ConfigureAwait(false))!;

    /// <summary>
    /// Deserialize JSON from a UTF-8 stream.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="utf8Stream">UTF-8 encoded stream.</param>
    /// <param name="jsonSerializerOptions">Optional deserialization options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Value returning deserialized data.</returns>
    protected static async ValueTask<T> DeserializeJsonAsync<T>(Stream utf8Stream, JsonSerializerOptions? jsonSerializerOptions, CancellationToken cancellationToken = default)
        => (await JsonSerializer.DeserializeAsync<T>(utf8Stream, jsonSerializerOptions, cancellationToken).ConfigureAwait(false))!;

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

    #region Delays

    /// <summary>
    /// Delays this operation for the specified amount of time.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    protected Task DelayAsync(CancellationToken cancellationToken = default)
        => DelayAsync(DelaySeconds, cancellationToken);

    /// <summary>
    /// Delays this operation for <see cref="DelaySeconds"/> seconds.
    /// </summary>
    /// <param name="delaySeconds">Delay in seconds.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    protected static Task DelayAsync(double delaySeconds, CancellationToken cancellationToken = default)
        => Task.Delay(TimeSpan.FromSeconds(delaySeconds), cancellationToken);

    /// <summary>
    /// Delays this operation for <see cref="DelaySeconds"/> seconds, after the first call to this method.
    /// </summary>
    /// <param name="delaySeconds">Delay in seconds.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    protected Task DelayAfterFirstAsync(double? delaySeconds = null, CancellationToken? cancellationToken = null)
        => DelayAfterFirstAsync(DelaySeconds, cancellationToken);

    /// <summary>
    /// Delays this operation for the specified amount of time, after the first call to this method.
    /// </summary>
    /// <param name="delaySeconds">Delay in seconds.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    protected Task DelayAfterFirstAsync(double delaySeconds, CancellationToken cancellationToken = default)
    {
        if (_delayFirstCalled)
            return Task.Delay(TimeSpan.FromSeconds(delaySeconds), cancellationToken);
        _delayFirstCalled = true;
        return Task.CompletedTask;
    }

    #endregion

    #region Logging

    /// <summary>
    /// Logs information log to logger.
    /// </summary>
    /// <param name="title">Title.</param>
    /// <param name="body">Body.</param>
    protected void LogInformation(string? title, string? body = null)
        => LogHandler?.Log(Profile.Tool, Profile.Group, title, body, LogLevel.Information);

    /// <summary>
    /// Logs title log to logger.
    /// </summary>
    /// <param name="title">Title.</param>
    /// <param name="body">Body.</param>
    protected void LogTitle(string? title, string? body = null)
        => LogHandler?.Log(Profile.Tool, Profile.Group, title, body, LogLevel.Title);

    /// <summary>
    /// Logs warning log to logger.
    /// </summary>
    /// <param name="title">Title.</param>
    /// <param name="body">Body.</param>
    protected void LogWarning(string? title, string? body = null)
        => LogHandler?.Log(Profile.Tool, Profile.Group, title, body, LogLevel.Warning);

    /// <summary>
    /// Logs error log to logger.
    /// </summary>
    /// <param name="title">Title.</param>
    /// <param name="body">Body.</param>
    protected void LogError(string? title, string? body = null)
        => LogHandler?.Log(Profile.Tool, Profile.Group, title, body, LogLevel.Error);

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
