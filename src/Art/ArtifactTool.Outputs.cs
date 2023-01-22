using System.Text.Json;

namespace Art;

public partial class ArtifactTool
{
    #region Outputs

    /// <summary>
    /// Outputs a text file for the specified artifact.
    /// </summary>
    /// <param name="text">Text to output.</param>
    /// <param name="key">Resource key.</param>
    /// <param name="options">Output options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public async Task OutputTextAsync(string text, ArtifactResourceKey key, OutputStreamOptions? options = null, CancellationToken cancellationToken = default)
        => await DataManager.OutputTextAsync(text, key, options, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Outputs a text file for the specified artifact.
    /// </summary>
    /// <param name="text">Text to output.</param>
    /// <param name="key">Artifact key.</param>
    /// <param name="file">Target filename.</param>
    /// <param name="path">File path to prepend.</param>
    /// <param name="options">Output options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public async Task OutputTextAsync(string text, ArtifactKey key, string file, string path = "", OutputStreamOptions? options = null, CancellationToken cancellationToken = default)
        => await OutputTextAsync(text, new ArtifactResourceKey(key, file, path), options, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Outputs a JSON-serialized file for the specified artifact.
    /// </summary>
    /// <param name="data">Data to output.</param>
    /// <param name="key">Resource key.</param>
    /// <param name="options">Output options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public async Task OutputJsonAsync<T>(T data, ArtifactResourceKey key, OutputStreamOptions? options = null, CancellationToken cancellationToken = default)
        => await DataManager.OutputJsonAsync(data, JsonOptions, key, options, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Outputs a JSON-serialized file for the specified artifact.
    /// </summary>
    /// <param name="data">Data to output.</param>
    /// <param name="key">Artifact key.</param>
    /// <param name="file">Target filename.</param>
    /// <param name="path">File path to prepend.</param>
    /// <param name="options">Output options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public async Task OutputJsonAsync<T>(T data, ArtifactKey key, string file, string path = "", OutputStreamOptions? options = null, CancellationToken cancellationToken = default)
        => await OutputJsonAsync(data, JsonOptions, new ArtifactResourceKey(key, file, path), options, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Outputs a JSON-serialized file for the specified artifact.
    /// </summary>
    /// <param name="data">Data to output.</param>
    /// <param name="jsonSerializerOptions">Serialization options.</param>
    /// <param name="key">Resource key.</param>
    /// <param name="options">Output options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public async Task OutputJsonAsync<T>(T data, JsonSerializerOptions jsonSerializerOptions, ArtifactResourceKey key, OutputStreamOptions? options = null, CancellationToken cancellationToken = default)
        => await DataManager.OutputJsonAsync(data, jsonSerializerOptions, key, options, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Outputs a JSON-serialized file for the specified artifact.
    /// </summary>
    /// <param name="data">Data to output.</param>
    /// <param name="jsonSerializerOptions">Serialization options.</param>
    /// <param name="key">Artifact key.</param>
    /// <param name="file">Target filename.</param>
    /// <param name="path">File path to prepend.</param>
    /// <param name="options">Output options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public async Task OutputJsonAsync<T>(T data, JsonSerializerOptions jsonSerializerOptions, ArtifactKey key, string file, string path = "", OutputStreamOptions? options = null, CancellationToken cancellationToken = default)
        => await OutputJsonAsync(data, jsonSerializerOptions, new ArtifactResourceKey(key, file, path), options, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Creates an output stream for a file for the specified artifact.
    /// </summary>
    /// <param name="key">Resource key.</param>
    /// <param name="options">Output options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning a writeable stream to write an output to.</returns>
    public async Task<CommittableStream> CreateOutputStreamAsync(ArtifactResourceKey key, OutputStreamOptions? options = null, CancellationToken cancellationToken = default)
        => await DataManager.CreateOutputStreamAsync(key, options, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Creates an output stream for a file for the specified artifact.
    /// </summary>
    /// <param name="key">Artifact key.</param>
    /// <param name="file">Target filename.</param>
    /// <param name="path">File path to prepend.</param>
    /// <param name="options">Output options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning a writeable stream to write an output to.</returns>
    public Task<CommittableStream> CreateOutputStreamAsync(ArtifactKey key, string file, string path = "", OutputStreamOptions? options = null, CancellationToken cancellationToken = default)
        => CreateOutputStreamAsync(new ArtifactResourceKey(key, file, path), options, cancellationToken);

    #endregion
}
