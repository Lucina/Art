using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Art.Common;

/// <summary>
/// Base type for artifact data managers.
/// </summary>
public abstract class ArtifactDataManager : IArtifactDataManager
{
    /// <inheritdoc />
    public abstract ValueTask<CommittableStream> CreateOutputStreamAsync(ArtifactResourceKey key, OutputStreamOptions? options = null, CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract ValueTask<bool> ExistsAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract ValueTask<bool> DeleteAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract ValueTask<Stream> OpenInputStreamAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public async ValueTask OutputTextAsync(string text, ArtifactResourceKey key, OutputStreamOptions? options = null, CancellationToken cancellationToken = default)
    {
        UpdateOptionsTextual(ref options);
        await using CommittableStream stream = await CreateOutputStreamAsync(key, options, cancellationToken).ConfigureAwait(false);
        await using var sw = new StreamWriter(stream);
        await sw.WriteAsync(text).ConfigureAwait(false);
        stream.ShouldCommit = true;
    }

    /// <inheritdoc />
    [RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
    public async ValueTask OutputJsonAsync<T>(T data, ArtifactResourceKey key, OutputStreamOptions? options = null, CancellationToken cancellationToken = default)
    {
        UpdateOptionsTextual(ref options);
        await using CommittableStream stream = await CreateOutputStreamAsync(key, options, cancellationToken).ConfigureAwait(false);
        await JsonSerializer.SerializeAsync(stream, data, cancellationToken: cancellationToken).ConfigureAwait(false);
        stream.ShouldCommit = true;
    }

    /// <inheritdoc />
    public async ValueTask OutputJsonAsync<T>(T data, JsonTypeInfo<T> jsonTypeInfo, ArtifactResourceKey key, OutputStreamOptions? options = null, CancellationToken cancellationToken = default)
    {
        UpdateOptionsTextual(ref options);
        await using CommittableStream stream = await CreateOutputStreamAsync(key, options, cancellationToken).ConfigureAwait(false);
        await JsonSerializer.SerializeAsync(stream, data, jsonTypeInfo, cancellationToken: cancellationToken).ConfigureAwait(false);
        stream.ShouldCommit = true;
    }

    /// <inheritdoc />
    [RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
    public async ValueTask OutputJsonAsync<T>(T data, JsonSerializerOptions jsonSerializerOptions, ArtifactResourceKey key, OutputStreamOptions? options = null, CancellationToken cancellationToken = default)
    {
        UpdateOptionsTextual(ref options);
        await using CommittableStream stream = await CreateOutputStreamAsync(key, options, cancellationToken).ConfigureAwait(false);
        await JsonSerializer.SerializeAsync(stream, data, jsonSerializerOptions, cancellationToken).ConfigureAwait(false);
        stream.ShouldCommit = true;
    }

    /// <inheritdoc />
    public virtual ValueTask<Checksum> ComputeChecksumAsync(ArtifactResourceKey key, string checksumId, CancellationToken cancellationToken = default)
    {
        return ChecksumUtility.ComputeChecksumAsync(this, key, checksumId, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async ValueTask<Checksum?> GetChecksumAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default)
    {
        if (!await ExistsAsync(key, cancellationToken).ConfigureAwait(false)) throw new KeyNotFoundException();
        return null;
    }

    private static void UpdateOptionsTextual(ref OutputStreamOptions? options)
    {
        if (options is { } optionsActual) options = optionsActual with { PreallocationSize = 0 };
    }
}
