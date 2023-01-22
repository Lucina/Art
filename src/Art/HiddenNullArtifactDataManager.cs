using System.Text.Json;

namespace Art;

internal class HiddenNullArtifactDataManager : ArtifactDataManagerBase
{
    /// <summary>
    /// Shared instance.
    /// </summary>
    public static readonly HiddenNullArtifactDataManager Instance = new();

    /// <inheritdoc />
    public override ValueTask<CommittableStream> CreateOutputStreamAsync(ArtifactResourceKey key, OutputStreamOptions? options = null, CancellationToken cancellationToken = default) => new(new HiddenCommittableSinkStream());

    /// <inheritdoc />
    public override ValueTask<bool> ExistsAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default) => new(false);

    /// <inheritdoc />
    public override ValueTask<bool> DeleteAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default) => new(true);

    /// <inheritdoc />
    public override ValueTask<Stream> OpenInputStreamAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default) => throw new KeyNotFoundException();

    /// <inheritdoc />
    public override async ValueTask OutputTextAsync(string text, ArtifactResourceKey key, OutputStreamOptions? options = null, CancellationToken cancellationToken = default)
    {
        // Write to stream to at least catch any encoding issues...
        await using var sw = new StreamWriter(new HiddenSinkStream());
        await sw.WriteAsync(text).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override async ValueTask OutputJsonAsync<T>(T data, ArtifactResourceKey key, OutputStreamOptions? options = null, CancellationToken cancellationToken = default)
    {
        // Write to stream to at least catch any serialization issues...
        await JsonSerializer.SerializeAsync(new HiddenSinkStream(), data, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override async ValueTask OutputJsonAsync<T>(T data, JsonSerializerOptions jsonSerializerOptions, ArtifactResourceKey key, OutputStreamOptions? options = null, CancellationToken cancellationToken = default)
    {
        // Write to stream to at least catch any serialization issues...
        await JsonSerializer.SerializeAsync(new HiddenSinkStream(), data, jsonSerializerOptions, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override ValueTask<Checksum> GetChecksumAsync(ArtifactResourceKey key, string checksumId, CancellationToken cancellationToken = default)
    {
        throw new KeyNotFoundException();
    }

    /// <inheritdoc />
    public override ValueTask<bool> ValidateChecksumAsync(ArtifactResourceKey key, Checksum checksum, CancellationToken cancellationToken = default)
    {
        throw new KeyNotFoundException();
    }

    /// <inheritdoc />
    public override async ValueTask<Checksum?> GetChecksumAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default)
    {
        if (!await ExistsAsync(key, cancellationToken)) throw new KeyNotFoundException();
        return null;
    }
}
