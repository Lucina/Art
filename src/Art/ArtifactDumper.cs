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
    /// <param name="dataManager">Data manager to use for this instance.</param>
    /// <param name="artifactDumpingProfile">Origin dumping profile.</param>
    protected ArtifactDumper(ArtifactDataManager dataManager, ArtifactDumpingProfile artifactDumpingProfile)
    {
        DataManager = dataManager;
        Profile = artifactDumpingProfile;
        DebugMode = TryGetOption(OptDebugMode, out bool debugMode) && debugMode;
    }

    /// <summary>
    /// Dump artifacts.
    /// </summary>
    /// <returns>Task.</returns>
    public abstract Task DumpAsync();

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
        if (!Profile.Options.TryGetValue(optKey, out JsonElement vv)) throw new ArtifactDumperOptionNotFoundException(optKey);
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
        if (Profile.Options.TryGetValue(optKey, out JsonElement vv))
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
    /// Tests if artifact is recognizably new.
    /// </summary>
    /// <param name="artifactInfo">Artifact to check.</param>
    /// <returns>Task returning true if this is a new artifact (newer than whatever exists with the same ID).</returns>
    protected async Task<bool> IsNewArtifactAsync(ArtifactInfo artifactInfo)
        => await DataManager.IsNewArtifactAsync(artifactInfo).ConfigureAwait(false);

    /// <summary>
    /// Registers artifact as known.
    /// </summary>
    /// <param name="artifactInfo">Artifact to register.</param>
    /// <returns>Task.</returns>
    protected async Task AddInfoAsync(ArtifactInfo artifactInfo)
        => await DataManager.AddInfoAsync(artifactInfo).ConfigureAwait(false);

    /// <summary>
    /// Outputs a text file for the specified artifact.
    /// </summary>
    /// <param name="artifactInfo">Artifact target.</param>
    /// <param name="text">Text to output.</param>
    /// <param name="file">Target filename.</param>
    /// <param name="path">File path to prepend.</param>
    /// <returns>Task.</returns>
    protected async Task OutputTextAsync(ArtifactInfo artifactInfo, string text, string file, string? path = null)
    => await DataManager.OutputTextAsync(artifactInfo, text, file, path).ConfigureAwait(false);

    /// <summary>
    /// Outputs a JSON-serialized file for the specified artifact.
    /// </summary>
    /// <param name="artifactInfo">Artifact target.</param>
    /// <param name="data">Data to output.</param>
    /// <param name="file">Target filename.</param>
    /// <param name="path">File path to prepend.</param>
    /// <returns>Task.</returns>
    protected async Task OutputJsonAsync<T>(ArtifactInfo artifactInfo, T data, string file, string? path = null)
        => await DataManager.OutputJsonAsync<T>(artifactInfo, data, file, path).ConfigureAwait(false);

    /// <summary>
    /// Outputs a JSON-serialized file for the specified artifact.
    /// </summary>
    /// <param name="artifactInfo">Artifact target.</param>
    /// <param name="data">Data to output.</param>
    /// <param name="jsonSerializerOptions">Serialization options.</param>
    /// <param name="file">Target filename.</param>
    /// <param name="path">File path to prepend.</param>
    /// <returns>Task.</returns>
    protected async Task OutputJsonAsync<T>(ArtifactInfo artifactInfo, T data, JsonSerializerOptions jsonSerializerOptions, string file, string? path = null)
        => await DataManager.OutputJsonAsync<T>(artifactInfo, data, jsonSerializerOptions, file, path).ConfigureAwait(false);
}
