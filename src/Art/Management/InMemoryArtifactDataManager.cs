using Art.Resources;

namespace Art.Management;

/// <summary>
/// Represents an in-memory artifact data manager.
/// </summary>
public class InMemoryArtifactDataManager : ArtifactDataManager
{
    /// <summary>
    /// Mapping of artifact keys to resource info.
    /// </summary>
    public IReadOnlyDictionary<ArtifactKey, List<ArtifactResourceInfo>> Artifacts => _artifacts;

    private readonly Dictionary<ArtifactKey, List<ArtifactResourceInfo>> _artifacts = new();

    /// <summary>
    /// Mapping of resource keys to data.
    /// </summary>
    public IReadOnlyDictionary<ArtifactResourceKey, Stream> Entries => _entries;

    private readonly Dictionary<ArtifactResourceKey, Stream> _entries = new();

    /// <inheritdoc />
    public override ValueTask<CommittableStream> CreateOutputStreamAsync(ArtifactResourceKey key, OutputStreamOptions? options = null, CancellationToken cancellationToken = default)
    {
        // Create a new output stream. If one already exists for mapping, get rid of it.
        // Since everything uses CommittableMemoryStream, underlying memory stream isn't disposed, so
        // previous buffer is still accessible. Doesn't need to be, but that's how it is right now.
        if (_entries.TryGetValue(key, out Stream? s))
        {
            // Invalidate existing.
            s.Dispose();
            _entries.Remove(key);
        }
        CommittableStream stream;
        if (options is { } optionsActual)
        {
            long preallocationSize = optionsActual.PreallocationSize;
            if (preallocationSize < 0 || preallocationSize > Array.MaxLength) throw new ArgumentException($"Invalid {nameof(OutputStreamOptions.PreallocationSize)} value", nameof(options));
            stream = preallocationSize != 0 ? new CommittableMemoryStream((int)optionsActual.PreallocationSize) : new CommittableMemoryStream();
        }
        else
            stream = new CommittableMemoryStream();
        ArtifactKey ak = key.Artifact;
        if (!_artifacts.TryGetValue(ak, out List<ArtifactResourceInfo>? list))
            _artifacts.Add(ak, list = new List<ArtifactResourceInfo>());
        list.Add(new ResultStreamArtifactResourceInfo(stream, key, null, null, null));
        _entries[key] = stream;
        return new ValueTask<CommittableStream>(stream);
    }

    /// <inheritdoc />
    public override ValueTask<bool> ExistsAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult(_entries.ContainsKey(key));
    }

    /// <inheritdoc />
    public override ValueTask<bool> DeleteAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult(_entries.Remove(key));
    }

    /// <inheritdoc />
    public override ValueTask<Stream> OpenInputStreamAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default)
    {
        // Use a stream wrapping original buffer, but hide away buffer and make stream read-only
        if (!_entries.TryGetValue(key, out Stream? stream)) throw new KeyNotFoundException();
        if (stream is not CommittableMemoryStream cms)
            throw new InvalidOperationException($"Expected {nameof(CommittableMemoryStream)} but got unexpected stream type {stream.GetType()}");
        MemoryStream oms = cms.MemoryStream;
        return ValueTask.FromResult<Stream>(new MemoryStream(oms.GetBuffer(), 0, (int)oms.Length, false, false));
    }

    private record ResultStreamArtifactResourceInfo(Stream Resource, ArtifactResourceKey Key, string? ContentType, DateTimeOffset? Updated, string? Version)
        : StreamArtifactResourceInfo(Resource, Key, ContentType, Updated, Version);
}
