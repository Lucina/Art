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
            foreach ((ArtifactKey ak, List<ArtifactResourceInfo> resources) in im.Artifacts)
            {
                if (await TryGetArtifactAsync(ak).ConfigureAwait(false) is not { } info) continue;
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
        public IReadOnlyDictionary<ArtifactKey, List<ArtifactResourceInfo>> Artifacts => _artifacts;

        private readonly Dictionary<ArtifactKey, List<ArtifactResourceInfo>> _artifacts = new();
        public IReadOnlyDictionary<ArtifactResourceKey, ResultStream> Entries => _entries;

        private readonly Dictionary<ArtifactResourceKey, ResultStream> _entries = new();

        public override ValueTask<Stream> CreateOutputStreamAsync(ArtifactResourceKey key)
        {
            if (_entries.TryGetValue(key, out ResultStream? stream))
            {
                stream.SetLength(0);
                return new(stream);
            }
            stream = new ResultStream();
            ArtifactKey ak = key.Artifact;
            if (!_artifacts.TryGetValue(ak, out List<ArtifactResourceInfo>? list))
                _artifacts.Add(ak, list = new List<ArtifactResourceInfo>());
            list.Add(new ResultStreamArtifactResourceInfo(stream, key, null, ArtifactResourceInfo.EmptyProperties));
            _entries.Add(key, stream);
            return new(stream);
        }
    }

    private record ResultStreamArtifactResourceInfo(Stream Resource, ArtifactResourceKey Key, string? Version, IReadOnlyDictionary<string, JsonElement> Properties)
        : StreamArtifactResourceInfo(Resource, Key, Version, Properties)
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
