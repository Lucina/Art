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
        list.Add(new ResultStreamArtifactResourceInfo(stream, key, null, null));
        _entries[key] = stream;
        return new ValueTask<Stream>(stream);
    }

    private record ResultStreamArtifactResourceInfo(Stream Resource, ArtifactResourceKey Key, DateTimeOffset? Updated, string? Version)
        : StreamArtifactResourceInfo(Resource, Key, Updated, Version)
    {
        public override async ValueTask ExportAsync(Stream stream, CancellationToken cancellationToken = default)
        {
            stream.Seek(0, SeekOrigin.Begin);
            await base.ExportAsync(stream, cancellationToken).ConfigureAwait(false);
        }
    }

    private class ResultStream : Stream
    {
        public readonly MemoryStream BaseStream = new();

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => true;

        public override long Length => BaseStream.Length;

        public override long Position
        {
            get => BaseStream.Position;
            set => BaseStream.Position = value;
        }

        public override void Flush() => BaseStream.Flush();
        public override int Read(byte[] buffer, int offset, int count) => BaseStream.Read(buffer, offset, count);
        public override long Seek(long offset, SeekOrigin origin) => BaseStream.Seek(offset, origin);
        public override void SetLength(long value) => BaseStream.SetLength(value);
        public override void Write(byte[] buffer, int offset, int count) => BaseStream.Write(buffer, offset, count);
    }
}
