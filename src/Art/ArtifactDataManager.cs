using System.Text.Json;

namespace Art;

/// <summary>
/// Represents a manager for artifact data.
/// </summary>
public abstract class ArtifactDataManager
{
    /// <summary>
    /// Creates an output stream for a file for the specified artifact.
    /// </summary>
    /// <param name="key">Resource key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning a writeable stream to write an output to.</returns>
    public abstract ValueTask<Stream> CreateOutputStreamAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Outputs a text file for the specified artifact.
    /// </summary>
    /// <param name="text">Text to output.</param>
    /// <param name="key">Resource key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public virtual async ValueTask OutputTextAsync(string text, ArtifactResourceKey key, CancellationToken cancellationToken = default)
    {
        using Stream stream = await CreateOutputStreamAsync(key, cancellationToken).ConfigureAwait(false);
        using var sw = new StreamWriter(stream);
        await sw.WriteAsync(text).ConfigureAwait(false);
    }

    /// <summary>
    /// Outputs a JSON-serialized file for the specified artifact.
    /// </summary>
    /// <param name="data">Data to output.</param>
    /// <param name="key">Resource key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public virtual async ValueTask OutputJsonAsync<T>(T data, ArtifactResourceKey key, CancellationToken cancellationToken = default)
    {
        using Stream stream = await CreateOutputStreamAsync(key, cancellationToken).ConfigureAwait(false);
        await JsonSerializer.SerializeAsync(stream, data, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Outputs a JSON-serialized file for the specified artifact.
    /// </summary>
    /// <param name="data">Data to output.</param>
    /// <param name="jsonSerializerOptions">Serialization options.</param>
    /// <param name="key">Resource key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public virtual async ValueTask OutputJsonAsync<T>(T data, JsonSerializerOptions jsonSerializerOptions, ArtifactResourceKey key, CancellationToken cancellationToken = default)
    {
        using Stream stream = await CreateOutputStreamAsync(key, cancellationToken).ConfigureAwait(false);
        await JsonSerializer.SerializeAsync(stream, data, jsonSerializerOptions, cancellationToken).ConfigureAwait(false);
    }
}
