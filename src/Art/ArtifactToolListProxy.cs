using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Art;

/// <summary>
/// Proxy to run artifact tool as a list tool.
/// </summary>
/// <param name="ArtifactTool">Artifact tool.</param>
public record ArtifactToolListProxy(ArtifactTool ArtifactTool)
{
    #region API

    /// <summary>
    /// Lists artifacts.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Async-enumerable artifacts.</returns>
    public async IAsyncEnumerable<ArtifactData> ListAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (ArtifactTool is IArtifactToolList listTool)
        {
            await foreach (ArtifactData res in listTool.ListAsync(cancellationToken).ConfigureAwait(false))
                yield return res;
            yield break;
        }
        if (ArtifactTool is IArtifactToolDump dumpTool)
        {
            ArtifactDataManager previous = ArtifactTool.DataManager;
            try
            {
                InMemoryArtifactDataManager im = new();
                await dumpTool.DumpAsync(cancellationToken).ConfigureAwait(false);
                foreach ((ArtifactKey ak, List<ArtifactResourceInfo> resources) in im.Artifacts)
                {
                    if (await ArtifactTool.TryGetArtifactAsync(ak, cancellationToken).ConfigureAwait(false) is not { } info) continue;
                    ArtifactData data = new(info);
                    data.AddRange(resources);
                    yield return data;
                }
            }
            finally
            {
                ArtifactTool.DataManager = previous;
            }
            yield break;
        }
        throw new NotSupportedException("Artifact tool is not a supported type");
    }

    private class InMemoryArtifactDataManager : ArtifactDataManager
    {
        public IReadOnlyDictionary<ArtifactKey, List<ArtifactResourceInfo>> Artifacts => _artifacts;

        private readonly Dictionary<ArtifactKey, List<ArtifactResourceInfo>> _artifacts = new();
        public IReadOnlyDictionary<ArtifactResourceKey, ResultStream> Entries => _entries;

        private readonly Dictionary<ArtifactResourceKey, ResultStream> _entries = new();

        public override ValueTask<Stream> CreateOutputStreamAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default)
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

        public override long Position { get => BaseStream.Position; set => BaseStream.Position = value; }

        public override void Flush() => BaseStream.Flush();
        public override int Read(byte[] buffer, int offset, int count) => BaseStream.Read(buffer, offset, count);
        public override long Seek(long offset, SeekOrigin origin) => BaseStream.Seek(offset, origin);
        public override void SetLength(long value) => BaseStream.SetLength(value);
        public override void Write(byte[] buffer, int offset, int count) => BaseStream.Write(buffer, offset, count);
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
