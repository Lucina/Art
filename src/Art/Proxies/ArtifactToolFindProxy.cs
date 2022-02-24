namespace Art.Proxies;

/// <summary>
/// Proxy to run artifact tool as a find tool.
/// </summary>
/// <param name="ArtifactTool">Artifact tool.</param>
public record ArtifactToolFindProxy(ArtifactTool ArtifactTool)
{
    #region API

    /// <summary>
    /// Finds artifacts.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Async-enumerable artifacts.</returns>
    public async Task<ArtifactData?> FindAsync(string id, CancellationToken cancellationToken = default)
    {
        if (ArtifactTool is IArtifactToolFind findTool)
            return await findTool.FindAsync(id, cancellationToken).ConfigureAwait(false);
        throw new NotSupportedException("Artifact tool is not a supported type");
    }

    #endregion
}
