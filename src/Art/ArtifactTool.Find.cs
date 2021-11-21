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
    /// <returns>Task returning found artifact or null.</returns>
    public ValueTask<ArtifactData?> FindAsync(string id)
    {
        EnsureState();
        return DoFindAsync(id);
    }

    /// <summary>
    /// Finds an artifact with the specified id.
    /// </summary>
    /// <param name="id">Artifact id.</param>
    /// <returns>Task returning found artifact or null.</returns>
    protected virtual ValueTask<ArtifactData?> DoFindAsync(string id)
    {
        return ValueTask.FromResult<ArtifactData?>(null);
    }

    #endregion
}
