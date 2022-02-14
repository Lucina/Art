namespace Art;

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
    public override ValueTask<Stream> CreateOutputStreamAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default)
    {
        if (_entries.TryGetValue(key, out Stream? s) && s is ResultStream stream)
        {
            stream.SetLength(0);
            return new ValueTask<Stream>(stream);
        }
        stream = new ResultStream();
        ArtifactKey ak = key.Artifact;
        if (!_artifacts.TryGetValue(ak, out List<ArtifactResourceInfo>? list))
            _artifacts.Add(ak, list = new List<ArtifactResourceInfo>());
        list.Add(new ResultStreamArtifactResourceInfo(stream, key, null, null, null));
        _entries[key] = stream;
        return new ValueTask<Stream>(stream);
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
    public override async ValueTask<Stream> OpenInputStreamAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default)
    {
        if (!_entries.TryGetValue(key, out Stream? stream)) throw new KeyNotFoundException();
        stream.Position = 0;
        MemoryStream ms = new();
        try
        {
            await stream.CopyToAsync(ms, cancellationToken);
        }
        finally
        {
            stream.Position = 0;
        }
        return ms;
    }

    private record ResultStreamArtifactResourceInfo(Stream Resource, ArtifactResourceKey Key, string? ContentType, DateTimeOffset? Updated, string? Version)
        : StreamArtifactResourceInfo(Resource, Key, ContentType, Updated, Version);

    private class ResultStream : Stream
    {
        private readonly MemoryStream _baseStream = new();

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => true;

        public override long Length => _baseStream.Length;

        public override long Position
        {
            get => _baseStream.Position;
            set => _baseStream.Position = value;
        }

        public override void Flush() => _baseStream.Flush();
        public override int Read(byte[] buffer, int offset, int count) => _baseStream.Read(buffer, offset, count);
        public override long Seek(long offset, SeekOrigin origin) => _baseStream.Seek(offset, origin);
        public override void SetLength(long value) => _baseStream.SetLength(value);
        public override void Write(byte[] buffer, int offset, int count) => _baseStream.Write(buffer, offset, count);
    }
}
