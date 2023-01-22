using System.Security.Cryptography;
using System.Text.Json;

namespace Art.Common;

/// <summary>
/// Base type for artifact data managers.
/// </summary>
public abstract class ArtifactDataManager : ArtifactDataManagerBase
{
    /// <inheritdoc />
    public override async ValueTask OutputTextAsync(string text, ArtifactResourceKey key, OutputStreamOptions? options = null, CancellationToken cancellationToken = default)
    {
        UpdateOptionsTextual(ref options);
        await using CommittableStreamBase stream = await CreateOutputStreamAsync(key, options, cancellationToken).ConfigureAwait(false);
        await using var sw = new StreamWriter(stream);
        await sw.WriteAsync(text).ConfigureAwait(false);
        stream.ShouldCommit = true;
    }

    /// <inheritdoc />
    public override async ValueTask OutputJsonAsync<T>(T data, ArtifactResourceKey key, OutputStreamOptions? options = null, CancellationToken cancellationToken = default)
    {
        UpdateOptionsTextual(ref options);
        await using CommittableStreamBase stream = await CreateOutputStreamAsync(key, options, cancellationToken).ConfigureAwait(false);
        await JsonSerializer.SerializeAsync(stream, data, cancellationToken: cancellationToken).ConfigureAwait(false);
        stream.ShouldCommit = true;
    }

    /// <inheritdoc />
    public override async ValueTask OutputJsonAsync<T>(T data, JsonSerializerOptions jsonSerializerOptions, ArtifactResourceKey key, OutputStreamOptions? options = null, CancellationToken cancellationToken = default)
    {
        UpdateOptionsTextual(ref options);
        await using CommittableStreamBase stream = await CreateOutputStreamAsync(key, options, cancellationToken).ConfigureAwait(false);
        await JsonSerializer.SerializeAsync(stream, data, jsonSerializerOptions, cancellationToken).ConfigureAwait(false);
        stream.ShouldCommit = true;
    }

    /// <inheritdoc />
    public override async ValueTask<Checksum> GetChecksumAsync(ArtifactResourceKey key, string checksumId, CancellationToken cancellationToken = default)
    {
        if (!ChecksumSource.TryGetHashAlgorithm(checksumId, out HashAlgorithm? hashAlgorithm))
            throw new ArgumentException("Unknown checksum ID", nameof(checksumId));
        await using Stream sourceStream = await OpenInputStreamAsync(key, cancellationToken);
        await using HashProxyStream hps = new(sourceStream, hashAlgorithm, true);
        await using MemoryStream ms = new();
        await hps.CopyToAsync(ms, cancellationToken);
        return new Checksum(checksumId, hps.GetHash());
    }

    /// <inheritdoc />
    public override async ValueTask<bool> ValidateChecksumAsync(ArtifactResourceKey key, Checksum checksum, CancellationToken cancellationToken = default)
        => ChecksumUtility.DatawiseEquals(await GetChecksumAsync(key, checksum.Id, cancellationToken), checksum);

    /// <inheritdoc />
    public override async ValueTask<Checksum?> GetChecksumAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default)
    {
        if (!await ExistsAsync(key, cancellationToken)) throw new KeyNotFoundException();
        return null;
    }

    private static void UpdateOptionsTextual(ref OutputStreamOptions? options)
    {
        if (options is { } optionsActual) options = optionsActual with { PreallocationSize = 0 };
    }
}
