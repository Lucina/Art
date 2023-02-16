using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Art.Common.IO;

namespace Art.Common.Management;

/// <summary>
/// Represents an artifact data manager that does not preserve data.
/// </summary>
public class NullArtifactDataManager : IArtifactDataManager
{
    /// <summary>
    /// Shared instance.
    /// </summary>
    public static readonly NullArtifactDataManager Instance = new();

    /// <inheritdoc />
    public ValueTask<CommittableStream> CreateOutputStreamAsync(ArtifactResourceKey key, OutputStreamOptions? options = null, CancellationToken cancellationToken = default) => new(new CommittableSinkStream());

    /// <inheritdoc />
    public ValueTask<bool> ExistsAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default) => new(false);

    /// <inheritdoc />
    public ValueTask<bool> DeleteAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default) => new(true);

    /// <inheritdoc />
    public ValueTask<Stream> OpenInputStreamAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default) => throw new KeyNotFoundException();

    /// <inheritdoc />
    public async ValueTask OutputTextAsync(string text, ArtifactResourceKey key, OutputStreamOptions? options = null, CancellationToken cancellationToken = default)
    {
        // Write to stream to at least catch any encoding issues...
        await using var sw = new StreamWriter(new SinkStream());
        await sw.WriteAsync(text).ConfigureAwait(false);
    }

    /// <inheritdoc />
    [RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
    public async ValueTask OutputJsonAsync<T>(T data, ArtifactResourceKey key, OutputStreamOptions? options = null, CancellationToken cancellationToken = default)
    {
        // Write to stream to at least catch any serialization issues...
        await JsonSerializer.SerializeAsync(new SinkStream(), data, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask OutputJsonAsync<T>(T data, JsonTypeInfo<T> jsonTypeInfo, ArtifactResourceKey key, OutputStreamOptions? options = null, CancellationToken cancellationToken = default)
    {
        // Write to stream to at least catch any serialization issues...
        await JsonSerializer.SerializeAsync(new SinkStream(), data, jsonTypeInfo, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    [RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
    public async ValueTask OutputJsonAsync<T>(T data, JsonSerializerOptions jsonSerializerOptions, ArtifactResourceKey key, OutputStreamOptions? options = null, CancellationToken cancellationToken = default)
    {
        // Write to stream to at least catch any serialization issues...
        await JsonSerializer.SerializeAsync(new SinkStream(), data, jsonSerializerOptions, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public ValueTask<Checksum> ComputeChecksumAsync(ArtifactResourceKey key, string checksumId, CancellationToken cancellationToken = default)
    {
        throw new KeyNotFoundException();
    }

    /// <inheritdoc />
    public async ValueTask<Checksum?> GetChecksumAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default)
    {
        if (!await ExistsAsync(key, cancellationToken).ConfigureAwait(false)) throw new KeyNotFoundException();
        return null;
    }
}
