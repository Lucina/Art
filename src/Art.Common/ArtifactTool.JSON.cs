using System.Text.Json;

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
}
