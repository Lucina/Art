using System.Text.Json;

namespace Art;

/// <summary>
/// Represents a manager for artifact data.
/// </summary>
public abstract class ArtifactDataManager
{
    /// <summary>
    /// Creates an output stream for the specified resource.
    /// </summary>
    /// <param name="key">Resource key.</param>
    /// <param name="options">Creation options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning a writeable stream to write output to.</returns>
    public abstract ValueTask<CommittableStream> CreateOutputStreamAsync(ArtifactResourceKey key, OutputStreamOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if data for the specified resource exists.
    /// </summary>
    /// <param name="key">Resource key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning true if resource exists.</returns>
    public abstract ValueTask<bool> ExistsAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes data for the specified resource.
    /// </summary>
    /// <param name="key">Resource key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning true if the resource was deleted.</returns>
    public abstract ValueTask<bool> DeleteAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a read-only stream for the specified resource.
    /// </summary>
    /// <param name="key">Resource key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning a read-only stream.</returns>
    public abstract ValueTask<Stream> OpenInputStreamAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Outputs a text file for the specified artifact.
    /// </summary>
    /// <param name="text">Text to output.</param>
    /// <param name="key">Resource key.</param>
    /// <param name="options">Output options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public virtual async ValueTask OutputTextAsync(string text, ArtifactResourceKey key, OutputStreamOptions? options = null, CancellationToken cancellationToken = default)
    {
        UpdateOptionsTextual(ref options);
        await using CommittableStream stream = await CreateOutputStreamAsync(key, options, cancellationToken).ConfigureAwait(false);
        await using var sw = new StreamWriter(stream);
        await sw.WriteAsync(text).ConfigureAwait(false);
        stream.ShouldCommit = true;
    }

    /// <summary>
    /// Outputs a JSON-serialized file for the specified artifact.
    /// </summary>
    /// <param name="data">Data to output.</param>
    /// <param name="key">Resource key.</param>
    /// <param name="options">Output options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public virtual async ValueTask OutputJsonAsync<T>(T data, ArtifactResourceKey key, OutputStreamOptions? options = null, CancellationToken cancellationToken = default)
    {
        UpdateOptionsTextual(ref options);
        await using CommittableStream stream = await CreateOutputStreamAsync(key, options, cancellationToken).ConfigureAwait(false);
        await JsonSerializer.SerializeAsync(stream, data, cancellationToken: cancellationToken).ConfigureAwait(false);
        stream.ShouldCommit = true;
    }

    /// <summary>
    /// Outputs a JSON-serialized file for the specified artifact.
    /// </summary>
    /// <param name="data">Data to output.</param>
    /// <param name="jsonSerializerOptions">Serialization options.</param>
    /// <param name="key">Resource key.</param>
    /// <param name="options">Output options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public virtual async ValueTask OutputJsonAsync<T>(T data, JsonSerializerOptions jsonSerializerOptions, ArtifactResourceKey key, OutputStreamOptions? options = null, CancellationToken cancellationToken = default)
    {
        UpdateOptionsTextual(ref options);
        await using CommittableStream stream = await CreateOutputStreamAsync(key, options, cancellationToken).ConfigureAwait(false);
        await JsonSerializer.SerializeAsync(stream, data, jsonSerializerOptions, cancellationToken).ConfigureAwait(false);
        stream.ShouldCommit = true;
    }

    private static void UpdateOptionsTextual(ref OutputStreamOptions? options)
    {
        if (options is { } optionsActual) options = optionsActual with { PreallocationSize = 0 };
    }
}
