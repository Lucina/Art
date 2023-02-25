using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Art.Common;

/// <summary>
/// Utility functions.
/// </summary>
public static class ArtUtils
{
    /// <summary>
    /// Creates a random path for the specified sibling path.
    /// </summary>
    /// <param name="sibling">Sibling path.</param>
    /// <param name="attempts">Maximum number of attempts to generate path.</param>
    /// <returns>Random path.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="sibling"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown for invalid <paramref name="sibling"/> (e.g. drive root) or <paramref name="attempts"/> (&lt;=0) value.</exception>
    /// <exception cref="IOException">Thrown if failed to create sibling path with specified attempts.</exception>
    public static string CreateRandomPathForSibling(string sibling, int attempts = 10)
    {
        string dir = Path.GetDirectoryName(sibling) ?? throw new ArgumentException("Sibling path cannot be a drive root", nameof(sibling));
        return CreateRandomPath(dir, attempts);
    }

    /// <summary>
    /// Creates a random path directly under the specified base directory.
    /// </summary>
    /// <param name="baseDirectory">Base directory.</param>
    /// <param name="attempts">Maximum number of attempts to generate path.</param>
    /// <returns>Random path.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="baseDirectory"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown for invalid <paramref name="attempts"/> (&lt;=0) value.</exception>
    /// <exception cref="IOException">Thrown if failed to create sibling path with specified attempts.</exception>
    public static string CreateRandomPath(string baseDirectory, int attempts = 10)
    {
        if (baseDirectory == null) throw new ArgumentNullException(nameof(baseDirectory));
        if (attempts <= 0) throw new ArgumentException("Invalid max number of attempts", nameof(attempts));
        for (int i = 0; i < attempts; i++)
        {
            string path = Path.Combine(baseDirectory, $"{Guid.NewGuid():N}.tmp");
            if (!File.Exists(path) && !Directory.Exists(path)) return path;
        }
        throw new IOException("Failed to create random path");
    }

    /// <summary>
    /// Converts a hex string (optionally including hex specifier "0x") to a byte array.
    /// </summary>
    /// <param name="hex">Hex string.</param>
    /// <returns>Value.</returns>
    /// <exception cref="FormatException">Thrown for invalid format.</exception>
    public static byte[] Dehex(ReadOnlySpan<char> hex)
    {
        if (hex.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase)) hex = hex[2..];
        return Convert.FromHexString(hex);
    }

    /// <summary>
    /// Loads an object from a UTF-8 JSON stream.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="stream">Stream to load from.</param>
    /// <returns>Read data.</returns>
    [RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
    public static T? LoadFromUtf8Stream<T>(Stream stream) => JsonSerializer.Deserialize<T>(stream, ArtJsonSerializerOptions.s_context.Options);

    /// <summary>
    /// Loads an object from a UTF-8 JSON stream.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="stream">Stream to load from.</param>
    /// <param name="jsonTypeInfo">JSON type info.</param>
    /// <returns>Read data.</returns>
    public static T? LoadFromUtf8Stream<T>(Stream stream, JsonTypeInfo<T> jsonTypeInfo) => JsonSerializer.Deserialize(stream, jsonTypeInfo);

    /// <summary>
    /// Loads an object from a UTF-8 JSON stream.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="stream">Stream to load from.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning read data.</returns>
    [RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
    public static async Task<T?> LoadFromUtf8StreamAsync<T>(Stream stream, CancellationToken cancellationToken = default) => await JsonSerializer.DeserializeAsync<T>(stream, ArtJsonSerializerOptions.s_context.Options, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Loads an object from a UTF-8 JSON stream.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="stream">Stream to load from.</param>
    /// <param name="jsonTypeInfo">JSON type info.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning read data.</returns>
    public static async Task<T?> LoadFromUtf8StreamAsync<T>(Stream stream, JsonTypeInfo<T> jsonTypeInfo, CancellationToken cancellationToken = default) => await JsonSerializer.DeserializeAsync(stream, jsonTypeInfo, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Loads an object from a JSON file.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="file">File path to load from.</param>
    /// <returns>Read data.</returns>
    [RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
    public static T? LoadFromFile<T>(string file) => JsonSerializer.Deserialize<T>(File.ReadAllText(file), ArtJsonSerializerOptions.s_context.Options);

    /// <summary>
    /// Loads an object from a JSON file.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="file">File path to load from.</param>
    /// <param name="jsonTypeInfo">JSON type info.</param>
    /// <returns>Read data.</returns>
    public static T? LoadFromFile<T>(string file, JsonTypeInfo<T> jsonTypeInfo) => JsonSerializer.Deserialize(File.ReadAllText(file), jsonTypeInfo);

    /// <summary>
    /// Loads an object from a JSON file.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="file">File path to load from.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning read data.</returns>
    [RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
    public static async Task<T?> LoadFromFileAsync<T>(string file, CancellationToken cancellationToken = default) => JsonSerializer.Deserialize<T>(await File.ReadAllTextAsync(file, cancellationToken).ConfigureAwait(false), ArtJsonSerializerOptions.s_context.Options);

    /// <summary>
    /// Loads an object from a JSON file.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="file">File path to load from.</param>
    /// <param name="jsonTypeInfo">JSON type info.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning read data.</returns>
    public static async Task<T?> LoadFromFileAsync<T>(string file, JsonTypeInfo<T> jsonTypeInfo, CancellationToken cancellationToken = default) => JsonSerializer.Deserialize(await File.ReadAllTextAsync(file, cancellationToken).ConfigureAwait(false), jsonTypeInfo);
}
