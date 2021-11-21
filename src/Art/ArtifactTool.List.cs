using System.Text.Json;

namespace Art;

public partial class ArtifactTool
{
    #region Fields

    /// <summary>
    /// True if this instance can list artifacts.
    /// </summary>
    public virtual bool CanList => false;

    #endregion

    #region API

    /// <summary>
    /// Lists artifacts.
    /// </summary>
    /// <returns>Async-enumerable artifacts.</returns>
    public async IAsyncEnumerable<ArtifactData> ListAsync()
    {
        EnsureState();
        await foreach (ArtifactData res in DoListAsync().ConfigureAwait(false))
            yield return res;
        if (_runDataOverridden) yield break;
        ArtifactDataManager previous = _dataManager;
        try
        {
            InMemoryArtifactDataManager im = new();
            await DoDumpAsync().ConfigureAwait(false);
            foreach ((ArtifactInfo info, List<ArtifactResourceInfo> resources) in im.Artifacts)
            {
                ArtifactData data = new(info);
                data.AddRange(resources);
                yield return data;
            }
        }
        finally
        {
            _dataManager = previous;
        }
    }

    private class InMemoryArtifactDataManager : ArtifactDataManager
    {
        public IReadOnlyDictionary<ArtifactInfo, List<ArtifactResourceInfo>> Artifacts => _artifacts;

        private readonly Dictionary<ArtifactInfo, List<ArtifactResourceInfo>> _artifacts = new();
        public IReadOnlyDictionary<DataEntryKey, ResultStream> Entries => _entries;

        private readonly Dictionary<DataEntryKey, ResultStream> _entries = new();

        public override ValueTask<Stream> CreateOutputStreamAsync(string file, ArtifactInfo artifactInfo, string? path = null, bool inArtifactFolder = true)
        {
            DataEntryKey entry = new(file, artifactInfo, path, inArtifactFolder);
            if (_entries.TryGetValue(entry, out ResultStream? stream))
            {
                stream.SetLength(0);
                return new(stream);
            }
            stream = new ResultStream();
            if (!_artifacts.TryGetValue(artifactInfo, out List<ArtifactResourceInfo>? list))
                _artifacts.Add(artifactInfo, list = new List<ArtifactResourceInfo>());
            list.Add(new ResultStreamArtifactResourceInfo(stream, artifactInfo.Id, file, path, inArtifactFolder, ArtifactResourceInfo.EmptyProperties));
            _entries.Add(entry, stream);
            return new(stream);
        }
    }

    private record ResultStreamArtifactResourceInfo(Stream Resource, string ArtifactId, string File, string? Path, bool InArtifactFolder, IReadOnlyDictionary<string, JsonElement> Properties) : StreamArtifactResourceInfo(Resource, ArtifactId, File, Path, InArtifactFolder, Properties)
    {
        public override async ValueTask ExportAsync(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            await base.ExportAsync(stream).ConfigureAwait(false);
        }
    }

    private class ResultStream : Stream
    {
        public readonly MemoryStream BaseStream = new();

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => true;

        public override long Length => BaseStream.Length;

        public override long Position { get => BaseStream.Position; set => BaseStream.Position = value; }

        public override void Flush() => BaseStream.Flush();
        public override int Read(byte[] buffer, int offset, int count) => BaseStream.Read(buffer, offset, count);
        public override long Seek(long offset, SeekOrigin origin) => BaseStream.Seek(offset, origin);
        public override void SetLength(long value) => BaseStream.SetLength(value);
        public override void Write(byte[] buffer, int offset, int count) => BaseStream.Write(buffer, offset, count);
    }

    private record struct DataEntryKey(string File, ArtifactInfo ArtifactInfo, string? Path, bool InArtifactFolder);

    /// <summary>
    /// Lists artifacts.
    /// </summary>
    /// <returns>Async-enumerable artifacts.</returns>
    protected virtual IAsyncEnumerable<ArtifactData> DoListAsync()
    {
        _runDataOverridden = false;
        return EmptyAsyncEnumerable<ArtifactData>.Singleton;
    }

    private class EmptyAsyncEnumerable<T> : IAsyncEnumerable<T>
    {
        public static readonly EmptyAsyncEnumerable<T> Singleton = new();
        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default) => EmptyAsyncEnumerator<T>.Singleton;
    }

    private class EmptyAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        public static readonly EmptyAsyncEnumerator<T> Singleton = new();
        public T Current => default!;
        public ValueTask DisposeAsync() => default;
        public ValueTask<bool> MoveNextAsync() => new(false);
    }

    #endregion
}
