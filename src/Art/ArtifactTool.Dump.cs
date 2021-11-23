namespace Art;

public partial class ArtifactTool
{

    #region Fields

    /// <summary>
    /// True if this instance can dump artifacts.
    /// </summary>
    public virtual bool CanDump => CanList;

    #endregion

    #region API

    /// <summary>
    /// Dumps artifacts.
    /// </summary>
    /// <param name="includeNonFull">Overwrite full entries with non-full if newer.</param>
    /// <returns>Task.</returns>
    public async ValueTask DumpAsync(bool includeNonFull = false)
    {
        EnsureState();
        await DoDumpAsync(includeNonFull).ConfigureAwait(false);
        if (_runOverridden) return;
        await foreach (ArtifactData data in DoListAsync().ConfigureAwait(false))
        {
            if (!await IsNewArtifactAsync(data.Info).ConfigureAwait(false)) continue;
            if (!data.Info.Full && !includeNonFull) continue;
            foreach (ArtifactResourceInfo resource in data.Values)
            {
                if (!resource.Exportable) continue;
                await using Stream stream = await CreateOutputStreamAsync(resource.File, data.Info, resource.Path, resource.InArtifactFolder).ConfigureAwait(false);
                await resource.ExportAsync(stream).ConfigureAwait(false);
                await AddResourceAsync(resource).ConfigureAwait(false);
            }
            await AddArtifactAsync(data.Info).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Dumps artifacts.
    /// </summary>
    /// <param name="includeNonFull">Overwrite full entries with non-full if newer.</param>
    /// <returns>Task.</returns>
    protected virtual ValueTask DoDumpAsync(bool includeNonFull = false)
    {
        _runOverridden = false;
        return ValueTask.CompletedTask;
    }

    #endregion
}
