using System.Security.Cryptography;
using System.Text.Json;
using Art.Crypto;

namespace Art.Management;

/// <summary>
/// Represents a manager for artifact data.
/// </summary>
public abstract class ArtifactDataManager
{
    #region Abstract

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
    /// <exception cref="KeyNotFoundException">Thrown for missing resource.</exception>
    public abstract ValueTask<Stream> OpenInputStreamAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default);

    #endregion

    #region Virtual

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

    /// <summary>
    /// Gets checksum of a resource.
    /// </summary>
    /// <param name="key">Resource key.</param>
    /// <param name="checksumId">Checksum algorithm ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Checksum for resource.</returns>
    /// <exception cref="KeyNotFoundException">Thrown for missing resource.</exception>
    /// <exception cref="ArgumentException">Thrown for a bad <paramref name="checksumId"/> value.</exception>
    public virtual async ValueTask<Checksum> GetChecksumAsync(ArtifactResourceKey key, string checksumId, CancellationToken cancellationToken = default)
    {
        if (!ChecksumSource.TryGetHashAlgorithm(checksumId, out HashAlgorithm? hashAlgorithm))
            throw new ArgumentException("Unknown checksum ID", nameof(checksumId));
        await using Stream sourceStream = await OpenInputStreamAsync(key, cancellationToken);
        await using HashProxyStream hps = new(sourceStream, hashAlgorithm, true);
        await using MemoryStream ms = new();
        await hps.CopyToAsync(ms, cancellationToken);
        return new Checksum(checksumId, hps.GetHash());
    }

    /// <summary>
    /// Validates a checksum for a given resource.
    /// </summary>
    /// <param name="key">Resource key.</param>
    /// <param name="checksum">Checksum to validate.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if checksum is validated.</returns>
    /// <exception cref="KeyNotFoundException">Thrown for missing resource.</exception>
    public virtual async ValueTask<bool> ValidateChecksumAsync(ArtifactResourceKey key, Checksum checksum, CancellationToken cancellationToken = default)
        => Checksum.DatawiseEquals(await GetChecksumAsync(key, checksum.Id, cancellationToken), checksum);

    /// <summary>
    /// Gets checksum associated with a resource if it exists.
    /// </summary>
    /// <param name="key">Resource key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A checksum for the resource, if one exists.</returns>
    /// <exception cref="KeyNotFoundException">Thrown for missing resource.</exception>
    /// <remarks>
    /// This method should return any "primary" checksum readily available from this manager.
    /// </remarks>
    public virtual async ValueTask<Checksum?> GetChecksumAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default)
    {
        if (!await ExistsAsync(key, cancellationToken)) throw new KeyNotFoundException();
        return null;
    }

    #endregion

    #region Internal

    private static void UpdateOptionsTextual(ref OutputStreamOptions? options)
    {
        if (options is { } optionsActual) options = optionsActual with { PreallocationSize = 0 };
    }

    #endregion
}
