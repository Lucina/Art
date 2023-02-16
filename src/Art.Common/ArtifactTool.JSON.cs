using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Art.Common;

public partial class ArtifactTool
{
    #region JSON

    /// <summary>
    /// Deserializes JSON from a UTF-8 stream.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="utf8Stream">UTF-8 encoded stream.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Value returning deserialized data.</returns>
    /// <remarks>
    /// This overload uses <see cref="IArtifactTool.JsonOptions"/> member automatically.
    /// </remarks>
    [RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
    public async Task<T?> DeserializeJsonAsync<T>(Stream utf8Stream, CancellationToken cancellationToken = default)
        => await JsonSerializer.DeserializeAsync<T>(utf8Stream, JsonOptions, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Deserializes JSON from a UTF-8 stream.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="utf8Stream">UTF-8 encoded stream.</param>
    /// <param name="jsonTypeInfo">JSON type info.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Value returning deserialized data.</returns>
    public static async Task<T?> DeserializeJsonAsync<T>(Stream utf8Stream, JsonTypeInfo<T> jsonTypeInfo, CancellationToken cancellationToken = default)
        => await JsonSerializer.DeserializeAsync(utf8Stream, jsonTypeInfo, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Deserializes JSON from a UTF-8 stream.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="utf8Stream">UTF-8 encoded stream.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Value returning deserialized data.</returns>
    /// <remarks>
    /// This overload uses <see cref="IArtifactTool.JsonOptions"/> member automatically.
    /// </remarks>
    [RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
    public async Task<T> DeserializeRequiredJsonAsync<T>(Stream utf8Stream, CancellationToken cancellationToken = default)
        => await JsonSerializer.DeserializeAsync<T>(utf8Stream, JsonOptions, cancellationToken).ConfigureAwait(false) ?? throw new NullJsonDataException();

    /// <summary>
    /// Deserializes JSON from a UTF-8 stream.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="utf8Stream">UTF-8 encoded stream.</param>
    /// <param name="jsonTypeInfo">JSON type info.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Value returning deserialized data.</returns>
    public static async Task<T> DeserializeRequiredJsonAsync<T>(Stream utf8Stream, JsonTypeInfo<T> jsonTypeInfo, CancellationToken cancellationToken = default)
        => await JsonSerializer.DeserializeAsync(utf8Stream, jsonTypeInfo, cancellationToken).ConfigureAwait(false) ?? throw new NullJsonDataException();

    /// <summary>
    /// Deserializes JSON from a UTF-8 stream.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="utf8Stream">UTF-8 encoded stream.</param>
    /// <param name="jsonSerializerOptions">Optional deserialization options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Value returning deserialized data.</returns>
    [RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
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
    [RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
    public static async Task<T> DeserializeRequiredJsonAsync<T>(Stream utf8Stream, JsonSerializerOptions? jsonSerializerOptions, CancellationToken cancellationToken = default)
        => await JsonSerializer.DeserializeAsync<T>(utf8Stream, jsonSerializerOptions, cancellationToken).ConfigureAwait(false) ?? throw new NullJsonDataException();

    /// <summary>
    /// Deserializes JSON from a string.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="str">String.</param>
    /// <returns>Value returning deserialized data.</returns>
    /// <remarks>
    /// This overload uses <see cref="IArtifactTool.JsonOptions"/> member automatically.
    /// </remarks>
    [RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
    public T? DeserializeJson<T>(string str)
        => JsonSerializer.Deserialize<T>(str, JsonOptions);

    /// <summary>
    /// Deserializes JSON from a string.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="str">String.</param>
    /// <param name="jsonTypeInfo">JSON type info.</param>
    /// <returns>Value returning deserialized data.</returns>
    public static T? DeserializeJson<T>(string str, JsonTypeInfo<T> jsonTypeInfo)
        => JsonSerializer.Deserialize(str, jsonTypeInfo);

    /// <summary>
    /// Deserializes JSON from a string.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="str">String.</param>
    /// <returns>Value returning deserialized data.</returns>
    /// <remarks>
    /// This overload uses <see cref="IArtifactTool.JsonOptions"/> member automatically.
    /// </remarks>
    [RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
    public T DeserializeRequiredJson<T>(string str)
        => JsonSerializer.Deserialize<T>(str, JsonOptions) ?? throw new NullJsonDataException();

    /// <summary>
    /// Deserializes JSON from a string.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="str">String.</param>
    /// <param name="jsonTypeInfo">JSON type info.</param>
    /// <returns>Value returning deserialized data.</returns>
    public static T DeserializeRequiredJson<T>(string str, JsonTypeInfo<T> jsonTypeInfo)
        => JsonSerializer.Deserialize(str, jsonTypeInfo) ?? throw new NullJsonDataException();

    /// <summary>
    /// Deserializes JSON from a string.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="str">String.</param>
    /// <param name="jsonSerializerOptions">Optional deserialization options.</param>
    /// <returns>Value returning deserialized data.</returns>
    [RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
    public static T? DeserializeJson<T>(string str, JsonSerializerOptions? jsonSerializerOptions)
        => JsonSerializer.Deserialize<T>(str, jsonSerializerOptions);

    /// <summary>
    /// Deserializes JSON from a string.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="str">String.</param>
    /// <param name="jsonSerializerOptions">Optional deserialization options.</param>
    /// <returns>Value returning deserialized data.</returns>
    [RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
    public static T DeserializeRequiredJson<T>(string str, JsonSerializerOptions? jsonSerializerOptions)
        => JsonSerializer.Deserialize<T>(str, jsonSerializerOptions) ?? throw new NullJsonDataException();

    #endregion
}
