using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Text.Json;

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
    public bool DebugMode { get; set; }

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

    //private bool _configured;

    private bool _disposed;

    private bool _delayFirstCalled;

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
    /// Initializes and configures this tool with the specified runtime configuration.
    /// </summary>
    /// <param name="config">Configuration.</param>
    /// <param name="profile">Profile.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public Task InitializeAsync(ArtifactToolConfig config, ArtifactToolProfile profile, CancellationToken cancellationToken = default)
    {
        EnsureNotDisposed();
        if (config == null) throw new ArgumentNullException(nameof(config));
        if (config.RegistrationManager == null) throw new ArgumentException("Cannot configure with null registration manager");
        if (config.DataManager == null) throw new ArgumentException("Cannot configure with null data manager");
        if (profile == null) throw new ArgumentException("Cannot configure with null profile");
        Config = config;
        RegistrationManager = config.RegistrationManager;
        DataManager = config.DataManager;
        Profile = profile;
        DebugMode = GetFlagTrue(OptDebugMode);
        //_configured = true;
        return ConfigureAsync(cancellationToken);
    }

    /// <summary>
    /// Initializes this tool.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public virtual Task ConfigureAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    #endregion

    #region Options

    /// <summary>
    /// Attempt to get option or throw exception if not found.
    /// </summary>
    /// <param name="optKey">Key to search.</param>
    /// <returns>Value, if located.</returns>
    /// <exception cref="ArtifactToolOptionNotFoundException">Thrown when option is not found.</exception>
    public string GetStringOptionOrExcept(string optKey)
    {
        if (!(Profile.Options?.TryGetValue(optKey, out JsonElement vv) ?? false)) throw new ArtifactToolOptionNotFoundException(optKey);
        return vv.ToString();
    }

    /// <summary>
    /// Attempt to get option or throw exception if not found or if null.
    /// </summary>
    /// <typeparam name="T">Value type.</typeparam>
    /// <param name="optKey">Key to search.</param>
    /// <returns>Value, if located and nonnull.</returns>
    /// <exception cref="ArtifactToolOptionNotFoundException">Thrown when option is not found.</exception>
    /// <exception cref="JsonException">Thrown when conversion failed.</exception>
    /// <exception cref="NotSupportedException">Thrown when type not supported.</exception>
    public T GetOptionOrExcept<T>(string optKey)
    {
        if (!(Profile.Options?.TryGetValue(optKey, out JsonElement vv) ?? false)) throw new ArtifactToolOptionNotFoundException(optKey);
        return vv.Deserialize<T>(ArtJsonOptions.s_jsonOptions) ?? throw new NullJsonDataException();
    }

    /// <summary>
    /// Attempt to get option.
    /// </summary>
    /// <typeparam name="T">Value type.</typeparam>
    /// <param name="optKey">Key to search.</param>
    /// <param name="value">Value, if located and nonnull.</param>
    /// <param name="throwIfIncorrectType">If true, throw a <see cref="JsonException"/> if type is wrong.</param>
    /// <returns>True if value is located and of the right type.</returns>
    public bool TryGetOption<T>(string optKey, [NotNullWhen(true)] out T? value, bool throwIfIncorrectType = false)
    {
        if (Profile.Options?.TryGetValue(optKey, out JsonElement vv) ?? false)
        {
            try
            {
                value = vv.Deserialize<T>(ArtJsonOptions.s_jsonOptions);
                return value != null;
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
    /// Attempt to get option.
    /// </summary>
    /// <param name="optKey">Key to search.</param>
    /// <param name="value">Value, if located and nonnull.</param>
    /// <param name="throwIfIncorrectType">If true, throw a <see cref="JsonException"/> if type is wrong.</param>
    /// <returns>True if value is located and of the right type.</returns>
    public bool TryGetStringOption(string optKey, [NotNullWhen(true)] out string? value, bool throwIfIncorrectType = false)
    {
        if (Profile.Options?.TryGetValue(optKey, out JsonElement vv) ?? false)
        {
            try
            {
                value = vv.ToString();
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
    public bool GetFlagTrue(string optKey, bool throwIfIncorrectType = false)
    {
        return TryGetOption(optKey, out bool? value, throwIfIncorrectType) && value.Value
               || TryGetOption(optKey, out string? valueStr) && s_yesLower.Contains(valueStr.ToLowerInvariant());
    }

    /// <summary>
    /// Gets an string option from a string value, or take value from <see cref="ArtifactToolProfile.Group"/>.
    /// </summary>
    /// <param name="optKey">Key to search.</param>
    /// <returns>Option value.</returns>
    public string GetStringOptionOrGroup(string optKey)
    {
        return TryGetStringOption(optKey, out string? optValue) ? optValue : Profile.Group;
    }

    /// <summary>
    /// Gets an Int64 option from a string or literal value, or parses value from <see cref="ArtifactToolProfile.Group"/>.
    /// </summary>
    /// <param name="optKey">Key to search.</param>
    /// <returns>Option value.</returns>
    public long GetInt64OptionOrGroup(string optKey)
    {
        return TryGetInt64Option(optKey, out long? optValue) ? optValue.Value : long.Parse(Profile.Group, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Gets an UInt64 option from a string or literal value, or parses value from <see cref="ArtifactToolProfile.Group"/>.
    /// </summary>
    /// <param name="optKey">Key to search.</param>
    /// <returns>Option value.</returns>
    public ulong GetUInt64OptionOrGroup(string optKey)
    {
        return TryGetUInt64Option(optKey, out ulong? optValue) ? optValue.Value : ulong.Parse(Profile.Group, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Attempts to get an Int64 option from a string or literal value.
    /// </summary>
    /// <param name="optKey">Key to search.</param>
    /// <param name="value">Value, if located and nonnull.</param>
    /// <returns>True if found.</returns>
    public bool TryGetInt64Option(string optKey, [NotNullWhen(true)] out long? value)
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
    public bool TryGetUInt64Option(string optKey, [NotNullWhen(true)] out ulong? value)
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
    /// <param name="name">Name.</param>
    /// <param name="date">Artifact creation date.</param>
    /// <param name="updateDate">Artifact update date.</param>
    /// <param name="full">True if this is a full artifact.</param>
    public ArtifactData CreateData(string id, string? name = null, DateTimeOffset? date = null, DateTimeOffset? updateDate = null, bool full = true)
        => new(this, Profile.Tool, Profile.Group, id, name, date, updateDate, full);

    /// <summary>
    /// Registers artifact as known.
    /// </summary>
    /// <param name="artifactInfo">Artifact to register.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public async Task AddArtifactAsync(ArtifactInfo artifactInfo, CancellationToken cancellationToken = default)
        => await RegistrationManager.AddArtifactAsync(artifactInfo, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Registers artifact resource as known.
    /// </summary>
    /// <param name="artifactResourceInfo">Artifact resource to register.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public async Task AddResourceAsync(ArtifactResourceInfo artifactResourceInfo, CancellationToken cancellationToken = default)
        => await RegistrationManager.AddResourceAsync(artifactResourceInfo, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Attempts to get info for the artifact with the specified ID.
    /// </summary>
    /// <param name="id">Artifact ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning retrieved artifact, if it exists.</returns>
    public async Task<ArtifactInfo?> TryGetArtifactAsync(string id, CancellationToken cancellationToken = default)
        => await RegistrationManager.TryGetArtifactAsync(new ArtifactKey(Profile.Tool, Profile.Group, id), cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Attempts to get info for the artifact with the specified ID.
    /// </summary>
    /// <param name="key">Artifact key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning retrieved artifact, if it exists.</returns>
    public async Task<ArtifactInfo?> TryGetArtifactAsync(ArtifactKey key, CancellationToken cancellationToken = default)
        => await RegistrationManager.TryGetArtifactAsync(key, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Attempts to get info for the resource with the specified key.
    /// </summary>
    /// <param name="key">Resource key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning retrieved resource, if it exists.</returns>
    public async Task<ArtifactResourceInfo?> TryGetResourceAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default)
        => await RegistrationManager.TryGetResourceAsync(key, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Tests artifact state against existing.
    /// </summary>
    /// <param name="artifactInfo">Artifact to check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning comparison against whatever exists with the same ID.</returns>
    public async ValueTask<ItemStateFlags> CompareArtifactAsync(ArtifactInfo artifactInfo, CancellationToken cancellationToken = default)
    {
        ItemStateFlags state = ItemStateFlags.None;
        if (await TryGetArtifactAsync(artifactInfo.Key, cancellationToken).ConfigureAwait(false) is not { } oldArtifact)
        {
            state |= ItemStateFlags.New;
            if (artifactInfo.Date != null || artifactInfo.UpdateDate != null)
                state |= ItemStateFlags.NewerDate;
        }
        else
        {
            if (artifactInfo.UpdateDate != null && oldArtifact.UpdateDate != null)
            {
                if (artifactInfo.UpdateDate > oldArtifact.UpdateDate)
                    state |= ItemStateFlags.NewerDate;
                else if (artifactInfo.UpdateDate < oldArtifact.UpdateDate)
                    state |= ItemStateFlags.OlderDate;
            }
            else if (artifactInfo.UpdateDate != null && oldArtifact.UpdateDate == null)
                state |= ItemStateFlags.NewerDate;
            else if (artifactInfo.UpdateDate == null && oldArtifact.UpdateDate == null)
            {
                if (artifactInfo.Date != null && oldArtifact.Date != null)
                {
                    if (artifactInfo.Date > oldArtifact.Date)
                        state |= ItemStateFlags.NewerDate;
                    else if (artifactInfo.Date < oldArtifact.Date)
                        state |= ItemStateFlags.OlderDate;
                }
                else if (artifactInfo.Date != null && oldArtifact.Date == null)
                    state |= ItemStateFlags.NewerDate;
            }
            if (artifactInfo.Full && !oldArtifact.Full)
                state |= ItemStateFlags.New;
        }
        return state;
    }

    /// <summary>
    /// Attempts to get resource with populated version (if available) based on provided resource.
    /// </summary>
    /// <param name="resource">Resource to check.</param>
    /// <param name="resourceUpdate">Resource update mode.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning instance for the resource with populated version (if available), and additional state information.</returns>
    public async Task<ArtifactResourceInfoWithState> DetermineUpdatedResourceAsync(ArtifactResourceInfo resource, ResourceUpdateMode resourceUpdate, CancellationToken cancellationToken = default)
    {
        ItemStateFlags state = resourceUpdate switch
        {
            ResourceUpdateMode.ArtifactSoft => ItemStateFlags.None,
            ResourceUpdateMode.ArtifactHard => ItemStateFlags.EnforceNew,
            ResourceUpdateMode.Soft => ItemStateFlags.None,
            ResourceUpdateMode.Hard => ItemStateFlags.EnforceNew,
            _ => throw new ArgumentOutOfRangeException(nameof(resourceUpdate), resourceUpdate, null)
        };
        resource = await resource.WithMetadataAsync(cancellationToken).ConfigureAwait(false);
        if (await TryGetResourceAsync(resource.Key, cancellationToken).ConfigureAwait(false) is not { } prev)
        {
            state |= ItemStateFlags.ChangedMetadata | ItemStateFlags.New;
            if (resource.Updated != null)
                state |= ItemStateFlags.NewerDate;
            if (resource.Version != null)
                state |= ItemStateFlags.ChangedVersion;
            if (resource.Checksum != null && !Checksum.DatawiseEquals(resource.Checksum, null))
                state |= ItemStateFlags.NewChecksum;
        }
        else
        {
            if (resource.Version != prev.Version)
                state |= ItemStateFlags.ChangedVersion;
            if (resource.Updated > prev.Updated)
                state |= ItemStateFlags.NewerDate;
            else if (resource.Updated < prev.Updated)
                state |= ItemStateFlags.OlderDate;
            if (resource.IsMetadataDifferent(prev))
                state |= ItemStateFlags.ChangedMetadata;
            if (resource.Checksum != null && !Checksum.DatawiseEquals(resource.Checksum, prev.Checksum))
                state |= ItemStateFlags.NewChecksum;
        }
        return new ArtifactResourceInfoWithState(resource, state);
    }

    #endregion

    #region Outputs

    /// <summary>
    /// Outputs a text file for the specified artifact.
    /// </summary>
    /// <param name="text">Text to output.</param>
    /// <param name="key">Resource key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public async Task OutputTextAsync(string text, ArtifactResourceKey key, CancellationToken cancellationToken = default)
        => await DataManager.OutputTextAsync(text, key, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Outputs a text file for the specified artifact.
    /// </summary>
    /// <param name="text">Text to output.</param>
    /// <param name="key">Artifact key.</param>
    /// <param name="file">Target filename.</param>
    /// <param name="path">File path to prepend.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public async Task OutputTextAsync(string text, ArtifactKey key, string file, string path = "", CancellationToken cancellationToken = default)
        => await OutputTextAsync(text, new ArtifactResourceKey(key, file, path), cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Outputs a JSON-serialized file for the specified artifact.
    /// </summary>
    /// <param name="data">Data to output.</param>
    /// <param name="key">Resource key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public async Task OutputJsonAsync<T>(T data, ArtifactResourceKey key, CancellationToken cancellationToken = default)
        => await DataManager.OutputJsonAsync(data, JsonOptions, key, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Outputs a JSON-serialized file for the specified artifact.
    /// </summary>
    /// <param name="data">Data to output.</param>
    /// <param name="key">Artifact key.</param>
    /// <param name="file">Target filename.</param>
    /// <param name="path">File path to prepend.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public async Task OutputJsonAsync<T>(T data, ArtifactKey key, string file, string path = "", CancellationToken cancellationToken = default)
        => await OutputJsonAsync(data, JsonOptions, new ArtifactResourceKey(key, file, path), cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Outputs a JSON-serialized file for the specified artifact.
    /// </summary>
    /// <param name="data">Data to output.</param>
    /// <param name="jsonSerializerOptions">Serialization options.</param>
    /// <param name="key">Resource key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public async Task OutputJsonAsync<T>(T data, JsonSerializerOptions jsonSerializerOptions, ArtifactResourceKey key, CancellationToken cancellationToken = default)
        => await DataManager.OutputJsonAsync(data, jsonSerializerOptions, key, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Outputs a JSON-serialized file for the specified artifact.
    /// </summary>
    /// <param name="data">Data to output.</param>
    /// <param name="jsonSerializerOptions">Serialization options.</param>
    /// <param name="key">Artifact key.</param>
    /// <param name="file">Target filename.</param>
    /// <param name="path">File path to prepend.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public async Task OutputJsonAsync<T>(T data, JsonSerializerOptions jsonSerializerOptions, ArtifactKey key, string file, string path = "", CancellationToken cancellationToken = default)
        => await OutputJsonAsync(data, jsonSerializerOptions, new ArtifactResourceKey(key, file, path), cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Creates an output stream for a file for the specified artifact.
    /// </summary>
    /// <param name="key">Resource key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning a writeable stream to write an output to.</returns>
    public async Task<Stream> CreateOutputStreamAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default)
        => await DataManager.CreateOutputStreamAsync(key, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Creates an output stream for a file for the specified artifact.
    /// </summary>
    /// <param name="key">Artifact key.</param>
    /// <param name="file">Target filename.</param>
    /// <param name="path">File path to prepend.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning a writeable stream to write an output to.</returns>
    public Task<Stream> CreateOutputStreamAsync(ArtifactKey key, string file, string path = "", CancellationToken cancellationToken = default)
        => CreateOutputStreamAsync(new ArtifactResourceKey(key, file, path), cancellationToken);

    #endregion

    #region JSON

    /// <summary>
    /// Deserializes JSON from a UTF-8 stream.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="utf8Stream">UTF-8 encoded stream.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Value returning deserialized data.</returns>
    /// <remarks>
    /// This overload uses <see cref="JsonOptions"/> member automatically.
    /// </remarks>
    public async Task<T?> DeserializeJsonAsync<T>(Stream utf8Stream, CancellationToken cancellationToken = default)
        => await JsonSerializer.DeserializeAsync<T>(utf8Stream, JsonOptions, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Deserializes JSON from a UTF-8 stream.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="utf8Stream">UTF-8 encoded stream.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Value returning deserialized data.</returns>
    /// <remarks>
    /// This overload uses <see cref="JsonOptions"/> member automatically.
    /// </remarks>
    public async Task<T> DeserializeRequiredJsonAsync<T>(Stream utf8Stream, CancellationToken cancellationToken = default)
        => await JsonSerializer.DeserializeAsync<T>(utf8Stream, JsonOptions, cancellationToken).ConfigureAwait(false) ?? throw new NullJsonDataException();

    /// <summary>
    /// Deserializes JSON from a UTF-8 stream.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="utf8Stream">UTF-8 encoded stream.</param>
    /// <param name="jsonSerializerOptions">Optional deserialization options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Value returning deserialized data.</returns>
    public static async Task<T?> DeserializeJsonAsync<T>(Stream utf8Stream, JsonSerializerOptions? jsonSerializerOptions, CancellationToken cancellationToken = default)
        => await JsonSerializer.DeserializeAsync<T>(utf8Stream, jsonSerializerOptions, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Deserializes JSON from a UTF-8 stream.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="utf8Stream">UTF-8 encoded stream.</param>
    /// <param name="jsonSerializerOptions">Optional deserialization options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Value returning deserialized data.</returns>
    public static async Task<T> DeserializeRequiredJsonAsync<T>(Stream utf8Stream, JsonSerializerOptions? jsonSerializerOptions, CancellationToken cancellationToken = default)
        => await JsonSerializer.DeserializeAsync<T>(utf8Stream, jsonSerializerOptions, cancellationToken).ConfigureAwait(false) ?? throw new NullJsonDataException();

    /// <summary>
    /// Deserializes JSON from a string.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="str">String.</param>
    /// <returns>Value returning deserialized data.</returns>
    /// <remarks>
    /// This overload uses <see cref="JsonOptions"/> member automatically.
    /// </remarks>
    public T? DeserializeJson<T>(string str)
        => JsonSerializer.Deserialize<T>(str, JsonOptions);

    /// <summary>
    /// Deserializes JSON from a string.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="str">String.</param>
    /// <returns>Value returning deserialized data.</returns>
    /// <remarks>
    /// This overload uses <see cref="JsonOptions"/> member automatically.
    /// </remarks>
    public T DeserializeRequiredJson<T>(string str)
        => JsonSerializer.Deserialize<T>(str, JsonOptions) ?? throw new NullJsonDataException();

    /// <summary>
    /// Deserializes JSON from a string.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="str">String.</param>
    /// <param name="jsonSerializerOptions">Optional deserialization options.</param>
    /// <returns>Value returning deserialized data.</returns>
    public static T? DeserializeJson<T>(string str, JsonSerializerOptions? jsonSerializerOptions)
        => JsonSerializer.Deserialize<T>(str, jsonSerializerOptions);

    /// <summary>
    /// Deserializes JSON from a string.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="str">String.</param>
    /// <param name="jsonSerializerOptions">Optional deserialization options.</param>
    /// <returns>Value returning deserialized data.</returns>
    public static T DeserializeRequiredJson<T>(string str, JsonSerializerOptions? jsonSerializerOptions)
        => JsonSerializer.Deserialize<T>(str, jsonSerializerOptions) ?? throw new NullJsonDataException();

    #endregion

    #region Delays

    /// <summary>
    /// Delays this operation for the specified amount of time.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public Task DelayAsync(CancellationToken cancellationToken = default)
        => DelayAsync(DelaySeconds, cancellationToken);

    /// <summary>
    /// Delays this operation for <see cref="DelaySeconds"/> seconds.
    /// </summary>
    /// <param name="delaySeconds">Delay in seconds.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public static Task DelayAsync(double delaySeconds, CancellationToken cancellationToken = default)
        => Task.Delay(TimeSpan.FromSeconds(delaySeconds), cancellationToken);

    /// <summary>
    /// Delays this operation for <see cref="DelaySeconds"/> seconds, after the first call to this method.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public Task DelayAfterFirstAsync(CancellationToken cancellationToken = default)
        => DelayAfterFirstAsync(DelaySeconds, cancellationToken);

    /// <summary>
    /// Delays this operation for the specified amount of time, after the first call to this method.
    /// </summary>
    /// <param name="delaySeconds">Delay in seconds.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public Task DelayAfterFirstAsync(double delaySeconds, CancellationToken cancellationToken = default)
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
    public void LogInformation(string? title, string? body = null)
        => LogHandler?.Log(Profile.Tool, Profile.Group, title, body, LogLevel.Information);

    /// <summary>
    /// Logs entry log to logger.
    /// </summary>
    /// <param name="title">Title.</param>
    /// <param name="body">Body.</param>
    public void LogEntry(string? title, string? body = null)
        => LogHandler?.Log(Profile.Tool, Profile.Group, title, body, LogLevel.Entry);

    /// <summary>
    /// Logs title log to logger.
    /// </summary>
    /// <param name="title">Title.</param>
    /// <param name="body">Body.</param>
    public void LogTitle(string? title, string? body = null)
        => LogHandler?.Log(Profile.Tool, Profile.Group, title, body, LogLevel.Title);

    /// <summary>
    /// Logs warning log to logger.
    /// </summary>
    /// <param name="title">Title.</param>
    /// <param name="body">Body.</param>
    public void LogWarning(string? title, string? body = null)
        => LogHandler?.Log(Profile.Tool, Profile.Group, title, body, LogLevel.Warning);

    /// <summary>
    /// Logs error log to logger.
    /// </summary>
    /// <param name="title">Title.</param>
    /// <param name="body">Body.</param>
    public void LogError(string? title, string? body = null)
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
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /*private void EnsureState()
    {
        EnsureNotDisposed();
        if (!_configured) throw new InvalidOperationException("Tool has not been initialized");
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
        ArtifactToolConfig config = new(artifactRegistrationManager, artifactDataManager, FailureBypassFlags.None);
        artifactToolProfile = artifactToolProfile.WithCoreTool(t);
        await t.InitializeAsync(config, artifactToolProfile, cancellationToken).ConfigureAwait(false);
        return t;
    }
    #endregion
}
