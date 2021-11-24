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
    /// <returns>Task.</returns>
    public async ValueTask DumpAsync()
    {
        EnsureState();
        await DoDumpAsync().ConfigureAwait(false);
        if (_runOverridden) return;
        await foreach (ArtifactData data in DoListAsync().ConfigureAwait(false))
        {
            if (!data.Info.Full && !Config.Options.IncludeNonFull) continue;
            bool isNewArtifact = await IsNewArtifactAsync(data.Info).ConfigureAwait(false);
            foreach (ArtifactResourceInfo resource in data.Values)
            {
                switch (Config.Options.ResourceUpdate)
                {
                    case ResourceUpdateMode.ArtifactSoft:
                        {
                            if (!isNewArtifact) continue;
                            (ArtifactResourceInfo versionedResource, bool isNewResource) = await DetermineUpdatedResourceAsync(resource, Config.Options.ResourceUpdate).ConfigureAwait(false);
                            if (!isNewResource) continue;
                            if (versionedResource.Exportable)
                            {
                                await using Stream stream = await CreateOutputStreamAsync(versionedResource.Key).ConfigureAwait(false);
                                await versionedResource.ExportAsync(stream).ConfigureAwait(false);
                            }
                            await AddResourceAsync(versionedResource).ConfigureAwait(false);
                            break;
                        }
                    case ResourceUpdateMode.ArtifactHard:
                        {
                            if (!isNewArtifact) continue;
                            (ArtifactResourceInfo versionedResource, _) = await DetermineUpdatedResourceAsync(resource, Config.Options.ResourceUpdate).ConfigureAwait(false);
                            if (versionedResource.Exportable)
                            {
                                await using Stream stream = await CreateOutputStreamAsync(versionedResource.Key).ConfigureAwait(false);
                                await versionedResource.ExportAsync(stream).ConfigureAwait(false);
                            }
                            await AddResourceAsync(versionedResource).ConfigureAwait(false);
                            break;
                        }
                    case ResourceUpdateMode.Populate:
                        {
                            (ArtifactResourceInfo versionedResource, _) = await DetermineUpdatedResourceAsync(resource, Config.Options.ResourceUpdate).ConfigureAwait(false);
                            await AddResourceAsync(resource).ConfigureAwait(false);
                            break;
                        }
                    case ResourceUpdateMode.Soft:
                        {
                            (ArtifactResourceInfo versionedResource, bool isNewResource) = await DetermineUpdatedResourceAsync(resource, Config.Options.ResourceUpdate).ConfigureAwait(false);
                            if (!isNewResource) continue;
                            if (versionedResource.Exportable)
                            {
                                await using Stream stream = await CreateOutputStreamAsync(versionedResource.Key).ConfigureAwait(false);
                                await versionedResource.ExportAsync(stream).ConfigureAwait(false);
                            }
                            await AddResourceAsync(versionedResource).ConfigureAwait(false);
                            break;
                        }
                    case ResourceUpdateMode.Hard:
                        {
                            (ArtifactResourceInfo versionedResource, _) = await DetermineUpdatedResourceAsync(resource, Config.Options.ResourceUpdate).ConfigureAwait(false);
                            if (versionedResource.Exportable)
                            {
                                await using Stream stream = await CreateOutputStreamAsync(versionedResource.Key).ConfigureAwait(false);
                                await versionedResource.ExportAsync(stream).ConfigureAwait(false);
                            }
                            await AddResourceAsync(versionedResource).ConfigureAwait(false);
                            break;
                        }
                }
            }
            if (isNewArtifact)
                await AddArtifactAsync(data.Info).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Attempts to get resource with populated version (if available) based on provided resource.
    /// </summary>
    /// <param name="resource">Resource to check.</param>
    /// <param name="resourceUpdate">Resource update mode.</param>
    /// <returns>Task returning instance for the resource with populated version (if available), and whether that instance is new.</returns>
    private async ValueTask<(ArtifactResourceInfo latest, bool isNew)> DetermineUpdatedResourceAsync(ArtifactResourceInfo resource, ResourceUpdateMode resourceUpdate)
    {
        resource = await resource.GetVersionedAsync().ConfigureAwait(false);
        switch (resourceUpdate)
        {
            case ResourceUpdateMode.ArtifactSoft:
            case ResourceUpdateMode.Populate:
            case ResourceUpdateMode.Soft:
                {
                    if (resource.Version == null || (await TryGetResourceAsync(resource.Key).ConfigureAwait(false)) is not { } prev) return (resource, true);
                    bool isNew = resource.Version != prev.Version;
                    return (resource, isNew);
                }
            case ResourceUpdateMode.ArtifactHard:
            case ResourceUpdateMode.Hard:
                {
                    return (resource, true);
                }
            default: throw new ArgumentException(nameof(resourceUpdate));
        }
    }

    /// <summary>
    /// Dumps artifacts.
    /// </summary>
    /// <returns>Task.</returns>
    protected virtual ValueTask DoDumpAsync()
    {
        _runOverridden = false;
        return ValueTask.CompletedTask;
    }

    #endregion
}
