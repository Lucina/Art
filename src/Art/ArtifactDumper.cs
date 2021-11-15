using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Art;

/// <summary>
/// Represents an instance of an artifact dumper.
/// </summary>
public abstract class ArtifactDumper
{
    /// <summary>
    /// Option used to check if currently in debug mode.
    /// </summary>
    public const string OptDebugMode = "debugMode";

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

    /// <summary>
    /// Dump artifacts.
    /// </summary>
    /// <returns>Task.</returns>
    public abstract ValueTask DumpAsync();

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

    /// <summary>
    /// Outputs a text file for the specified artifact.
    /// </summary>
    /// <param name="text">Text to output.</param>
    /// <param name="file">Target filename.</param>
    /// <param name="artifactInfo">Artifact target.</param>
    /// <param name="path">File path to prepend.</param>
    /// <returns>Task.</returns>
    protected async ValueTask OutputTextAsync(string text, string file, ArtifactInfo? artifactInfo = null, string? path = null)
    => await DataManager.OutputTextAsync(text, file, artifactInfo, path).ConfigureAwait(false);

    /// <summary>
    /// Outputs a JSON-serialized file for the specified artifact.
    /// </summary>
    /// <param name="data">Data to output.</param>
    /// <param name="file">Target filename.</param>
    /// <param name="artifactInfo">Artifact target.</param>
    /// <param name="path">File path to prepend.</param>
    /// <returns>Task.</returns>
    protected async ValueTask OutputJsonAsync<T>(T data, string file, ArtifactInfo? artifactInfo = null, string? path = null)
        => await DataManager.OutputJsonAsync<T>(data, file, artifactInfo, path).ConfigureAwait(false);

    /// <summary>
    /// Outputs a JSON-serialized file for the specified artifact.
    /// </summary>
    /// <param name="data">Data to output.</param>
    /// <param name="jsonSerializerOptions">Serialization options.</param>
    /// <param name="file">Target filename.</param>
    /// <param name="artifactInfo">Artifact target.</param>
    /// <param name="path">File path to prepend.</param>
    /// <returns>Task.</returns>
    protected async ValueTask OutputJsonAsync<T>(T data, JsonSerializerOptions jsonSerializerOptions, string file, ArtifactInfo? artifactInfo = null, string? path = null)
        => await DataManager.OutputJsonAsync<T>(data, jsonSerializerOptions, file, artifactInfo, path).ConfigureAwait(false);
}
