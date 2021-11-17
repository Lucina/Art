using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Art;

/// <summary>
/// Represents an instance of an artifact dumper.
/// </summary>
public abstract class ArtifactDumper : IDisposable
{

    #region Fields

    /// <summary>
    /// Log handler for this dumper.
    /// </summary>
    public ILogHandler? LogHandler;

    /// <summary>
    /// Option used to check if currently in debug mode.
    /// </summary>
    public const string OptDebugMode = "debugMode";

    /// <summary>
    /// JSON serialization defaults.
    /// </summary>
    protected JsonSerializerOptions JsonOptions { get => _jsonOptions ??= new JsonSerializerOptions(); set => _jsonOptions = value; }

    /// <summary>
    /// Registration manager used by this instance.
    /// </summary>
    protected ArtifactRegistrationManager RegistrationManager { get; }

    /// <summary>
    /// Data manager used by this instance.
    /// </summary>
    protected ArtifactDataManager DataManager { get; }

    /// <summary>
    /// Origin dumping profile.
    /// </summary>
    protected ArtifactDumpingProfile Profile { get; }

    /// <summary>
    /// True if this dumper is in debug mode.
    /// </summary>
    protected bool DebugMode { get; set; }

    #endregion

    #region Private fields

    private JsonSerializerOptions _jsonOptions = new();

    #endregion

    #region Constructor

    /// <summary>
    /// Creates a new instance of <see cref="ArtifactDumper"/>.
    /// </summary>
    /// <param name="registrationManager">Registration manager to use for this instance.</param>
    /// <param name="dataManager">Data manager to use for this instance.</param>
    /// <param name="artifactDumpingProfile">Origin dumping profile.</param>
    protected ArtifactDumper(ArtifactRegistrationManager registrationManager, ArtifactDataManager dataManager, ArtifactDumpingProfile artifactDumpingProfile)
    {
        RegistrationManager = registrationManager;
        DataManager = dataManager;
        Profile = artifactDumpingProfile;
        DebugMode = TryGetOption(OptDebugMode, out bool debugMode) && debugMode;
    }

    #endregion

    #region API

    /// <summary>
    /// Dump artifacts.
    /// </summary>
    /// <returns>Task.</returns>
    public abstract ValueTask DumpAsync();

    #endregion

    #region Options

    /// <summary>
    /// Attempt to get option or throw exception if not found or if null.
    /// </summary>
    /// <typeparam name="T">Value type.</typeparam>
    /// <param name="optKey">Key to search.</param>
    /// <param name="value">Value, if located and nonnull.</param>
    /// <exception cref="ArtifactDumperOptionNotFoundException">Thrown when option is not found.</exception>
    /// <exception cref="JsonException">Thrown when conversion failed.</exception>
    /// <exception cref="NotSupportedException">Thrown when type not supported.</exception>
    protected void GetOptionOrExcept<T>(string optKey, out T value)
    {
        if (!(Profile.Options?.TryGetValue(optKey, out JsonElement vv) ?? false)) throw new ArtifactDumperOptionNotFoundException(optKey);
        value = vv.Deserialize<T>(ArtJsonOptions.JsonOptions)!;
    }

    /// <summary>
    /// Attempt to get option.
    /// </summary>
    /// <typeparam name="T">Value type.</typeparam>
    /// <param name="optKey">Key to search.</param>
    /// <param name="value">Value, if located and nonnull.</param>
    /// <param name="throwIfIncorrectType">If true, throw a <see cref="JsonException"/> or <see cref="NotSupportedException"/> if type is wrong.</param>
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
            catch (NotSupportedException)
            {
                if (throwIfIncorrectType) throw;
            }
        }
        value = default;
        return false;
    }

    #endregion

    #region Artifact management

    /// <summary>
    /// Registers artifact as known.
    /// </summary>
    /// <param name="artifactInfo">Artifact to register.</param>
    /// <returns>Task.</returns>
    protected async ValueTask AddArtifactAsync(ArtifactInfo artifactInfo)
        => await RegistrationManager.AddArtifactAsync(artifactInfo).ConfigureAwait(false);

    /// <summary>
    /// Attempts to get info for the artifact with the specified ID.
    /// </summary>
    /// <param name="id">Artifact ID.</param>
    /// <returns>Task returning retrieved artifact, if it exists.</returns>
    protected async ValueTask<ArtifactInfo?> TryGetArtifactAsync(string id)
        => await RegistrationManager.TryGetArtifactAsync(id).ConfigureAwait(false);

    /// <summary>
    /// Tests if artifact is recognizably new.
    /// </summary>
    /// <param name="artifactInfo">Artifact to check.</param>
    /// <returns>Task returning true if this is a new artifact (newer than whatever exists with the same ID).</returns>
    protected async ValueTask<bool> IsNewArtifactAsync(ArtifactInfo artifactInfo)
        => await RegistrationManager.IsNewArtifactAsync(artifactInfo).ConfigureAwait(false);

    #endregion

    #region Outputs

    /// <summary>
    /// Outputs a text file for the specified artifact.
    /// </summary>
    /// <param name="text">Text to output.</param>
    /// <param name="file">Target filename.</param>
    /// <param name="artifactInfo">Artifact target.</param>
    /// <param name="inArtifactFolder">If false, place artifact under common root.</param>
    /// <param name="path">File path to prepend.</param>
    /// <returns>Task.</returns>
    protected async ValueTask OutputTextAsync(string text, string file, ArtifactInfo artifactInfo, bool inArtifactFolder = true, string? path = null)
    => await DataManager.OutputTextAsync(text, file, artifactInfo, inArtifactFolder, path).ConfigureAwait(false);

    /// <summary>
    /// Outputs a JSON-serialized file for the specified artifact.
    /// </summary>
    /// <param name="data">Data to output.</param>
    /// <param name="file">Target filename.</param>
    /// <param name="artifactInfo">Artifact target.</param>
    /// <param name="inArtifactFolder">If false, place artifact under common root.</param>
    /// <param name="path">File path to prepend.</param>
    /// <returns>Task.</returns>
    protected async ValueTask OutputJsonAsync<T>(T data, string file, ArtifactInfo artifactInfo, bool inArtifactFolder = true, string? path = null)
        => await DataManager.OutputJsonAsync<T>(data, JsonOptions, file, artifactInfo, inArtifactFolder, path).ConfigureAwait(false);

    /// <summary>
    /// Outputs a JSON-serialized file for the specified artifact.
    /// </summary>
    /// <param name="data">Data to output.</param>
    /// <param name="jsonSerializerOptions">Serialization options.</param>
    /// <param name="file">Target filename.</param>
    /// <param name="artifactInfo">Artifact target.</param>
    /// <param name="inArtifactFolder">If false, place artifact under common root.</param>
    /// <param name="path">File path to prepend.</param>
    /// <returns>Task.</returns>
    protected async ValueTask OutputJsonAsync<T>(T data, JsonSerializerOptions jsonSerializerOptions, string file, ArtifactInfo artifactInfo, bool inArtifactFolder = true, string? path = null)
        => await DataManager.OutputJsonAsync<T>(data, jsonSerializerOptions, file, artifactInfo, inArtifactFolder, path).ConfigureAwait(false);

    /// <summary>
    /// Creates an output stream for a file for the specified artifact.
    /// </summary>
    /// <param name="file">Target filename.</param>
    /// <param name="artifactInfo">Artifact target.</param>
    /// <param name="inArtifactFolder">If false, place artifact under common root.</param>
    /// <param name="path">File path to prepend.</param>
    /// <returns>Task returning a writeable stream to write an output to.</returns>
    protected ValueTask<Stream> CreateOutputStreamAsync(string file, ArtifactInfo artifactInfo, bool inArtifactFolder = true, string? path = null)
        => DataManager.CreateOutputStreamAsync(file, artifactInfo, inArtifactFolder, path);

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
    protected abstract void Dispose(bool disposing);

    /// <inheritdoc/>
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    #endregion
}
