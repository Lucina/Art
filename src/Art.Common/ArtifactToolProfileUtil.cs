using System.Text.Json;
using System.Text.RegularExpressions;

namespace Art.Common;

/// <summary>
/// Utility for <see cref="ArtifactToolProfile"/>.
/// </summary>
public static class ArtifactToolProfileUtil
{
    /// <summary>
    /// Deserializes profiles.
    /// </summary>
    /// <param name="path">Path to file containing profile or profile array.</param>
    /// <returns>Array of profiles.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="path"/> is null.</exception>
    /// <exception cref="InvalidDataException">Thrown if null value encountered.</exception>
    public static ArtifactToolProfile[] DeserializeProfilesFromFile(string path)
    {
        if (path == null) throw new ArgumentNullException(nameof(path));
        return DeserializeProfiles(JsonSerializer.Deserialize<JsonElement>(File.ReadAllText(path)));
    }

    /// <summary>
    /// Deserializes profiles.
    /// </summary>
    /// <param name="path">Path to file containing profile or profile array.</param>
    /// <param name="options">Custom serializer options.</param>
    /// <returns>Array of profiles.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="path"/> or <paramref name="options"/> are null.</exception>
    /// <exception cref="InvalidDataException">Thrown if null value encountered.</exception>
    public static ArtifactToolProfile[] DeserializeProfilesFromFile(string path, JsonSerializerOptions options)
    {
        if (path == null) throw new ArgumentNullException(nameof(path));
        if (options == null) throw new ArgumentNullException(nameof(options));
        return DeserializeProfiles(JsonSerializer.Deserialize<JsonElement>(File.ReadAllText(path)), options);
    }

    /// <summary>
    /// Serializes profiles.
    /// </summary>
    /// <param name="path">Path to write profiles to.</param>
    /// <param name="profiles">Array of profiles.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="profiles"/> is null.</exception>
    /// <exception cref="InvalidDataException">Thrown if null value encountered.</exception>
    public static void SerializeProfilesToFile(string path, params ArtifactToolProfile[] profiles) => SerializeProfilesToFile(path, ArtJsonSerializerOptions.s_jsonOptions, profiles);

    /// <summary>
    /// Serializes profiles.
    /// </summary>
    /// <param name="path">Path to write profiles to.</param>
    /// <param name="options">Custom serializer options.</param>
    /// <param name="profiles">Array of profiles.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="options"/> or <paramref name="profiles"/> are null.</exception>
    /// <exception cref="InvalidDataException">Thrown if null value encountered.</exception>
    public static void SerializeProfilesToFile(string path, JsonSerializerOptions options, params ArtifactToolProfile[] profiles)
    {
        if (options == null) throw new ArgumentNullException(nameof(options));
        if (profiles == null) throw new ArgumentNullException(nameof(profiles));
        using var fs = File.Create(path);
        JsonSerializer.Serialize(fs, profiles, options);
    }

    /// <summary>
    /// Deserializes profiles.
    /// </summary>
    /// <param name="utf8Stream">UTF-8 stream containing profile or profile array.</param>
    /// <returns>Array of profiles.</returns>
    /// <exception cref="InvalidDataException">Thrown if null value encountered.</exception>
    public static ArtifactToolProfile[] DeserializeProfiles(Stream utf8Stream)
    {
        return DeserializeProfiles(JsonSerializer.Deserialize<JsonElement>(utf8Stream));
    }

    /// <summary>
    /// Deserializes profiles.
    /// </summary>
    /// <param name="utf8Stream">UTF-8 stream containing profile or profile array.</param>
    /// <param name="options">Custom serializer options.</param>
    /// <returns>Array of profiles.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="options"/> is null.</exception>
    /// <exception cref="InvalidDataException">Thrown if null value encountered.</exception>
    public static ArtifactToolProfile[] DeserializeProfiles(Stream utf8Stream, JsonSerializerOptions options)
    {
        if (options == null) throw new ArgumentNullException(nameof(options));
        return DeserializeProfiles(JsonSerializer.Deserialize<JsonElement>(utf8Stream), options);
    }

    /// <summary>
    /// Serializes profiles.
    /// </summary>
    /// <param name="utf8Stream">UTF-8 stream to output to.</param>
    /// <param name="profiles">Array of profiles.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="profiles"/> is null.</exception>
    /// <exception cref="InvalidDataException">Thrown if null value encountered.</exception>
    public static void SerializeProfiles(Stream utf8Stream, params ArtifactToolProfile[] profiles) => SerializeProfiles(utf8Stream, ArtJsonSerializerOptions.s_jsonOptions, profiles);

    /// <summary>
    /// Serializes profiles.
    /// </summary>
    /// <param name="utf8Stream">UTF-8 stream to output to.</param>
    /// <param name="options">Custom serializer options.</param>
    /// <param name="profiles">Array of profiles.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="options"/> or <paramref name="profiles"/> are null.</exception>
    /// <exception cref="InvalidDataException">Thrown if null value encountered.</exception>
    public static void SerializeProfiles(Stream utf8Stream, JsonSerializerOptions options, params ArtifactToolProfile[] profiles)
    {
        if (options == null) throw new ArgumentNullException(nameof(options));
        if (profiles == null) throw new ArgumentNullException(nameof(profiles));
        JsonSerializer.Serialize(utf8Stream, profiles, options);
    }

    /// <summary>
    /// Deserializes profiles.
    /// </summary>
    /// <param name="element">Element containing profile or profile array.</param>
    /// <returns>Array of profiles.</returns>
    /// <exception cref="InvalidDataException">Thrown if null value encountered.</exception>
    public static ArtifactToolProfile[] DeserializeProfiles(JsonElement element) => DeserializeProfiles(element, ArtJsonSerializerOptions.s_jsonOptions);

    /// <summary>
    /// Deserializes profiles.
    /// </summary>
    /// <param name="element">Element containing profile or profile array.</param>
    /// <param name="options">Custom serializer options.</param>
    /// <returns>Array of profiles.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="options"/> is null.</exception>
    /// <exception cref="InvalidDataException">Thrown if null value encountered.</exception>
    public static ArtifactToolProfile[] DeserializeProfiles(JsonElement element, JsonSerializerOptions options)
    {
        if (options == null) throw new ArgumentNullException(nameof(options));
        if (element.ValueKind == JsonValueKind.Object)
            return new[] { element.Deserialize<ArtifactToolProfile>(options) ?? throw new InvalidDataException() };
        return element.Deserialize<ArtifactToolProfile[]>(options) ?? throw new InvalidDataException();
    }

    /// <summary>
    /// Serializes profiles.
    /// </summary>
    /// <param name="profiles">Array of profiles.</param>
    /// <returns>Serialized profiles.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="profiles"/> is null.</exception>
    public static JsonElement SerializeProfiles(params ArtifactToolProfile[] profiles) => SerializeProfiles(ArtJsonSerializerOptions.s_jsonOptions, profiles);

    /// <summary>
    /// Serializes profiles.
    /// </summary>
    /// <param name="options">Custom serializer options.</param>
    /// <param name="profiles">Array of profiles.</param>
    /// <returns>Serialized profiles.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="options"/> or <paramref name="profiles"/> are null.</exception>
    public static JsonElement SerializeProfiles(JsonSerializerOptions options, params ArtifactToolProfile[] profiles)
    {
        if (options == null) throw new ArgumentNullException(nameof(options));
        if (profiles == null) throw new ArgumentNullException(nameof(profiles));
        return JsonSerializer.SerializeToElement(profiles, options);
    }

    private static readonly Regex s_toolRegex = new(@"^([\S\s]+)::([\S\s]+)$");

    /// <summary>
    /// Separates assembly and type name from <see cref="ArtifactToolProfile.Tool"/>.
    /// </summary>
    /// <returns>Separated assembly and type name.</returns>
    /// <exception cref="ArgumentException">Thrown if this instance has an invalid <see cref="ArtifactToolProfile.Tool"/> value.</exception>
    public static ArtifactToolID GetID(this ArtifactToolProfile profile) => GetID(profile.Tool);

    /// <summary>
    /// Separates assembly and type name from <see cref="ArtifactToolProfile.Tool"/>.
    /// </summary>
    /// <param name="tool">Artifact tool target string(assembly::toolType)</param>
    /// <returns>Separated assembly and type name.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="tool"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if this instance has an invalid <paramref name="tool"/> value.</exception>
    public static ArtifactToolID GetID(string tool)
    {
        if (tool == null) throw new ArgumentNullException(nameof(tool));
        if (s_toolRegex.Match(tool) is not { Success: true } match)
            throw new ArgumentException("Tool string is in invalid format, must be \"<assembly>::<toolType>\"", nameof(tool));
        return new ArtifactToolID(match.Groups[1].Value, match.Groups[2].Value);
    }

    /// <summary>
    /// Creates a tool profile for the specified tool.
    /// </summary>
    /// <param name="tool">Target tool string.</param>
    /// <param name="group">Target group.</param>
    /// <param name="options">Options.</param>
    /// <returns>Profile.</returns>
    public static ArtifactToolProfile Create(string tool, string group, params (string, JsonElement)[] options)
        => new(tool, group, options.ToDictionary(v => v.Item1, v => v.Item2));

    /// <summary>
    /// Creates a tool profile for the specified tool.
    /// </summary>
    /// <typeparam name="TTool">Tool type.</typeparam>
    /// <param name="group">Target group.</param>
    /// <param name="options">Options.</param>
    /// <returns>Profile.</returns>
    public static ArtifactToolProfile Create<TTool>(string group, params (string, JsonElement)[] options) where TTool : ArtifactToolBase
        => new(ArtifactToolBase.CreateToolString<TTool>(), group, options.ToDictionary(v => v.Item1, v => v.Item2));

    /// <summary>
    /// Creates a tool profile for the specified tool.
    /// </summary>
    /// <param name="toolType">Tool type.</param>
    /// <param name="group">Target group.</param>
    /// <param name="options">Options.</param>
    /// <returns>Profile.</returns>
    /// <remarks>
    /// This overload sets <see cref="ArtifactToolProfile.Options"/> to null if no options are specified.
    /// </remarks>
    public static ArtifactToolProfile Create(Type toolType, string group, params (string, JsonElement)[] options)
        => CreateInternal(toolType, group, options, false);

    /// <summary>
    /// Creates a tool profile for the specified tool.
    /// </summary>
    /// <param name="toolType">Tool type.</param>
    /// <param name="group">Target group.</param>
    /// <param name="options">Options.</param>
    /// <returns>Profile.</returns>
    /// <remarks>
    /// This overload sets <see cref="ArtifactToolProfile.Options"/> to a valid dictionary even if no options are specified.
    /// </remarks>
    public static ArtifactToolProfile CreateWithOptions(Type toolType, string group, params (string, JsonElement)[] options)
        => CreateInternal(toolType, group, options, true);

    private static ArtifactToolProfile CreateInternal(Type toolType, string group, (string, JsonElement)[] options, bool alwaysOptions)
        => new(ArtifactToolBase.CreateToolString(toolType), group, options.Length == 0 ? alwaysOptions ? new Dictionary<string, JsonElement>() : null : options.ToDictionary(v => v.Item1, v => v.Item2));

    /// <summary>
    /// Creates an instance of this profile with most derived core type of instance or instance's type.
    /// </summary>
    /// <param name="profile">Profile.</param>
    /// <param name="instance">Instance to derive tool type from.</param>
    /// <returns>Profile.</returns>
    public static ArtifactToolProfile WithCoreTool(this ArtifactToolProfile profile, object instance) => profile.WithCoreTool(instance.GetType());

    /// <summary>
    /// Creates an instance of this profile with most derived core type or given type.
    /// </summary>
    /// <param name="profile">Profile.</param>
    /// <param name="type">Tool type.</param>
    /// <returns>Profile.</returns>
    public static ArtifactToolProfile WithCoreTool(this ArtifactToolProfile profile, Type type) => profile with { Tool = ArtifactToolBase.CreateCoreToolString(type) };

    /// <summary>
    /// Creates an instance of this profile with specified comparer.
    /// </summary>
    /// <param name="profile">Profile.</param>
    /// <param name="comparer">Comparer to use.</param>
    /// <returns>Profile</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="comparer"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if duplicate keys are encountered using the specified comparer.</exception>
    public static ArtifactToolProfile WithOptionsComparer(this ArtifactToolProfile profile, StringComparer comparer)
    {
        if (comparer == null) throw new ArgumentNullException(nameof(comparer));
        return profile.Options == null ? profile : profile with { Options = new Dictionary<string, JsonElement>(profile.Options, comparer) };
    }

    /// <summary>
    /// Creates a new options dictionary for this profile with the specified comparer.
    /// </summary>
    /// <param name="profile">Profile.</param>
    /// <param name="comparer">Comparer to use.</param>
    /// <returns>Profile</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="comparer"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if duplicate keys are encountered using the specified comparer.</exception>
    public static Dictionary<string, JsonElement> GetOptionsWithOptionsComparer(this ArtifactToolProfile profile, StringComparer comparer)
    {
        if (comparer == null) throw new ArgumentNullException(nameof(comparer));
        return profile.Options == null ? new Dictionary<string, JsonElement>() : new Dictionary<string, JsonElement>(profile.Options, comparer);
    }
}
