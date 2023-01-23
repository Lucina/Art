﻿using System.Text.Json;

namespace Art;

/// <summary>
/// Represents a manager for artifact data.
/// </summary>
public interface IArtifactDataManager
{
    /// <summary>
    /// Creates an output stream for the specified resource.
    /// </summary>
    /// <param name="key">Resource key.</param>
    /// <param name="options">Creation options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning a writeable stream to write output to.</returns>
    ValueTask<CommittableStreamBase> CreateOutputStreamAsync(ArtifactResourceKey key, OutputStreamOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if data for the specified resource exists.
    /// </summary>
    /// <param name="key">Resource key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning true if resource exists.</returns>
    ValueTask<bool> ExistsAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes data for the specified resource.
    /// </summary>
    /// <param name="key">Resource key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning true if the resource was deleted.</returns>
    ValueTask<bool> DeleteAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a read-only stream for the specified resource.
    /// </summary>
    /// <param name="key">Resource key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning a read-only stream.</returns>
    /// <exception cref="KeyNotFoundException">Thrown for missing resource.</exception>
    ValueTask<Stream> OpenInputStreamAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Outputs a text file for the specified artifact.
    /// </summary>
    /// <param name="text">Text to output.</param>
    /// <param name="key">Resource key.</param>
    /// <param name="options">Output options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    ValueTask OutputTextAsync(string text, ArtifactResourceKey key, OutputStreamOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Outputs a JSON-serialized file for the specified artifact.
    /// </summary>
    /// <param name="data">Data to output.</param>
    /// <param name="key">Resource key.</param>
    /// <param name="options">Output options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    ValueTask OutputJsonAsync<T>(T data, ArtifactResourceKey key, OutputStreamOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Outputs a JSON-serialized file for the specified artifact.
    /// </summary>
    /// <param name="data">Data to output.</param>
    /// <param name="jsonSerializerOptions">Serialization options.</param>
    /// <param name="key">Resource key.</param>
    /// <param name="options">Output options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    ValueTask OutputJsonAsync<T>(T data, JsonSerializerOptions jsonSerializerOptions, ArtifactResourceKey key, OutputStreamOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets checksum of a resource.
    /// </summary>
    /// <param name="key">Resource key.</param>
    /// <param name="checksumId">Checksum algorithm ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Checksum for resource.</returns>
    /// <exception cref="KeyNotFoundException">Thrown for missing resource.</exception>
    /// <exception cref="ArgumentException">Thrown for a bad <paramref name="checksumId"/> value.</exception>
    ValueTask<Checksum> GetChecksumAsync(ArtifactResourceKey key, string checksumId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a checksum for a given resource.
    /// </summary>
    /// <param name="key">Resource key.</param>
    /// <param name="checksum">Checksum to validate.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if checksum is validated.</returns>
    /// <exception cref="KeyNotFoundException">Thrown for missing resource.</exception>
    ValueTask<bool> ValidateChecksumAsync(ArtifactResourceKey key, Checksum checksum, CancellationToken cancellationToken = default);

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
    ValueTask<Checksum?> GetChecksumAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default);
}
