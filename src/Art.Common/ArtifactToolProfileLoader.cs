using System.Text.Json;

namespace Art.Common;

/// <summary>
/// Utility for loading <see cref="ArtifactToolProfile"/>.
/// </summary>
public static class ArtifactToolProfileLoader
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
}
