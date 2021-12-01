namespace Art;

public partial class ArtifactTool
{
    #region Fields

    /// <summary>
    /// True if this instance can find artifacts.
    /// </summary>
    public virtual bool CanFind => false;

    #endregion

    #region API

    /// <summary>
    /// Finds an artifact with the specified id.
    /// </summary>
    /// <param name="id">Artifact id.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning found artifact or null.</returns>
    public ValueTask<ArtifactData?> FindAsync(string id, CancellationToken cancellationToken = default)
    {
        EnsureState();
        return DoFindAsync(id, cancellationToken);
    }

    /// <summary>
    /// Finds an artifact with the specified id.
    /// </summary>
    /// <param name="id">Artifact id.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning found artifact or null.</returns>
    protected virtual ValueTask<ArtifactData?> DoFindAsync(string id, CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult<ArtifactData?>(null);
    }

    #endregion
}
