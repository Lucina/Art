using System.Runtime.CompilerServices;

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

    #endregion
}

/// <summary>
/// Represents empty async enumerable.
/// </summary>
/// <typeparam name="T">Value type.</typeparam>
public class EmptyAsyncEnumerable<T> : IAsyncEnumerable<T>
{
    /// <summary>
    /// Singleton instance.
    /// </summary>
    public static readonly EmptyAsyncEnumerable<T> Singleton = new();

    /// <summary>
    /// Gets enumerator.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Enumerator.</returns>
    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default) => EmptyAsyncEnumerator<T>.Singleton;
}

/// <summary>
/// Represents empty async enumerator.
/// </summary>
/// <typeparam name="T">Value type.</typeparam>
public class EmptyAsyncEnumerator<T> : IAsyncEnumerator<T>
{
    /// <summary>
    /// Singleton instance.
    /// </summary>
    public static readonly EmptyAsyncEnumerator<T> Singleton = new();

    /// <summary>
    /// Current value.
    /// </summary>
    public T Current => default!;

    /// <summary>
    /// Disposes enumerator.
    /// </summary>
    /// <returns></returns>
    public ValueTask DisposeAsync() => default;

    /// <summary>
    /// Checks enumerator.
    /// </summary>
    /// <returns></returns>
    public ValueTask<bool> MoveNextAsync() => new(false);
}
