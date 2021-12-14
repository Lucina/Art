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
