namespace Art;

/// <summary>
/// Proxy to run artifact tool as a dump tool.
/// </summary>
/// <param name="ArtifactTool">Artifact tool.</param>
/// <param name="Options">Dump options.</param>
public record ArtifactToolDumpProxy(ArtifactTool ArtifactTool, ArtifactToolDumpOptions Options)
{
    /// <summary>
    /// Dumps artifacts.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public async ValueTask DumpAsync(CancellationToken cancellationToken = default)
    {
        if (ArtifactTool is IArtifactToolDump dumpTool)
        {
            await dumpTool.DumpAsync(cancellationToken).ConfigureAwait(false);
            return;
        }
        if (ArtifactTool is IArtifactToolList listTool)
        {
            await foreach (ArtifactData data in listTool.ListAsync(cancellationToken).ConfigureAwait(false))
            {
                switch (Options.SkipMode)
                {
                    case ArtifactSkipMode.NoSkip:
                        break;
                    case ArtifactSkipMode.SkipAllFromFirstKnown:
                        {
                            ArtifactInfo? info = await ArtifactTool.TryGetArtifactAsync(data.Info.Key.Id, cancellationToken).ConfigureAwait(false);
                            if (info != null)
                                return;
                            break;
                        }
                    case ArtifactSkipMode.SkipKnown:
                        {
                            ArtifactInfo? info = await ArtifactTool.TryGetArtifactAsync(data.Info.Key.Id, cancellationToken).ConfigureAwait(false);
                            if (info != null)
                                continue;
                            break;
                        }
                    default:
                        throw new IndexOutOfRangeException($"Invalid {nameof(ArtifactSkipMode)}");
                }
                if (!data.Info.Full && !Options.IncludeNonFull) continue;
                bool isNewArtifact = await ArtifactTool.IsNewArtifactAsync(data.Info, cancellationToken).ConfigureAwait(false);
                foreach (ArtifactResourceInfo resource in data.Values)
                {
                    switch (Options.ResourceUpdate)
                    {
                        case ResourceUpdateMode.ArtifactSoft:
                            {
                                if (!isNewArtifact) continue;
                                (ArtifactResourceInfo versionedResource, bool isNewResource) = await ArtifactTool.DetermineUpdatedResourceAsync(resource, Options.ResourceUpdate, cancellationToken).ConfigureAwait(false);
                                if (!isNewResource) continue;
                                if (versionedResource.Exportable)
                                {
                                    await using Stream stream = await ArtifactTool.CreateOutputStreamAsync(versionedResource.Key, cancellationToken).ConfigureAwait(false);
                                    await versionedResource.ExportAsync(stream, cancellationToken).ConfigureAwait(false);
                                }
                                await ArtifactTool.AddResourceAsync(versionedResource, cancellationToken).ConfigureAwait(false);
                                break;
                            }
                        case ResourceUpdateMode.ArtifactHard:
                            {
                                if (!isNewArtifact) continue;
                                (ArtifactResourceInfo versionedResource, _) = await ArtifactTool.DetermineUpdatedResourceAsync(resource, Options.ResourceUpdate, cancellationToken).ConfigureAwait(false);
                                if (versionedResource.Exportable)
                                {
                                    await using Stream stream = await ArtifactTool.CreateOutputStreamAsync(versionedResource.Key, cancellationToken).ConfigureAwait(false);
                                    await versionedResource.ExportAsync(stream, cancellationToken).ConfigureAwait(false);
                                }
                                await ArtifactTool.AddResourceAsync(versionedResource, cancellationToken).ConfigureAwait(false);
                                break;
                            }
                        case ResourceUpdateMode.Populate:
                            {
                                (ArtifactResourceInfo versionedResource, _) = await ArtifactTool.DetermineUpdatedResourceAsync(resource, Options.ResourceUpdate, cancellationToken).ConfigureAwait(false);
                                await ArtifactTool.AddResourceAsync(versionedResource, cancellationToken).ConfigureAwait(false);
                                break;
                            }
                        case ResourceUpdateMode.Soft:
                            {
                                (ArtifactResourceInfo versionedResource, bool isNewResource) = await ArtifactTool.DetermineUpdatedResourceAsync(resource, Options.ResourceUpdate, cancellationToken).ConfigureAwait(false);
                                if (!isNewResource) continue;
                                if (versionedResource.Exportable)
                                {
                                    await using Stream stream = await ArtifactTool.CreateOutputStreamAsync(versionedResource.Key, cancellationToken).ConfigureAwait(false);
                                    await versionedResource.ExportAsync(stream, cancellationToken).ConfigureAwait(false);
                                }
                                await ArtifactTool.AddResourceAsync(versionedResource, cancellationToken).ConfigureAwait(false);
                                break;
                            }
                        case ResourceUpdateMode.Hard:
                            {
                                (ArtifactResourceInfo versionedResource, _) = await ArtifactTool.DetermineUpdatedResourceAsync(resource, Options.ResourceUpdate, cancellationToken).ConfigureAwait(false);
                                if (versionedResource.Exportable)
                                {
                                    await using Stream stream = await ArtifactTool.CreateOutputStreamAsync(versionedResource.Key, cancellationToken).ConfigureAwait(false);
                                    await versionedResource.ExportAsync(stream, cancellationToken).ConfigureAwait(false);
                                }
                                await ArtifactTool.AddResourceAsync(versionedResource, cancellationToken).ConfigureAwait(false);
                                break;
                            }
                    }
                }
                if (isNewArtifact)
                    await ArtifactTool.AddArtifactAsync(data.Info, cancellationToken).ConfigureAwait(false);
            }
            return;
        }
        throw new NotSupportedException("Artifact tool is not a supported type");
    }
}
