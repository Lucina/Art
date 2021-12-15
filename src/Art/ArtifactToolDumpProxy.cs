namespace Art;

/// <summary>
/// Proxy to run artifact tool as a dump tool.
/// </summary>
/// <param name="ArtifactTool">Artifact tool.</param>
/// <param name="Options">Dump options.</param>
/// <param name="LogHandler">Log handler.</param>
public record ArtifactToolDumpProxy(ArtifactTool ArtifactTool, ArtifactToolDumpOptions Options, IToolLogHandler LogHandler)
{
    /// <summary>
    /// Dumps artifacts.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public async ValueTask DumpAsync(CancellationToken cancellationToken = default)
    {
        ArtifactTool.LogHandler = LogHandler;
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
                    case ArtifactSkipMode.None:
                        break;
                    case ArtifactSkipMode.FastExit:
                        {
                            ArtifactInfo? info = await ArtifactTool.TryGetArtifactAsync(data.Info.Key.Id, cancellationToken).ConfigureAwait(false);
                            if (info != null)
                                return;
                            break;
                        }
                    case ArtifactSkipMode.Known:
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
                ArtifactInfo i = data.Info;
                ItemStateFlags iF = await ArtifactTool.CompareArtifactAsync(data.Info, cancellationToken).ConfigureAwait(false);
                LogHandler.Log(i.Key.Tool, i.Key.Group, $"{((iF & ItemStateFlags.NewerIdentityMask) != 0 ? "[NEW] " : "")}{i.GetInfoString()}", null, LogLevel.Title);
                if ((iF & ItemStateFlags.NewerIdentityMask) != 0)
                    await ArtifactTool.AddArtifactAsync(data.Info with { Full = false }, cancellationToken).ConfigureAwait(false);
                foreach (ArtifactResourceInfo resource in data.Values)
                {
                    switch (Options.ResourceUpdate)
                    {
                        case ResourceUpdateMode.ArtifactSoft:
                        case ResourceUpdateMode.ArtifactHard:
                            if ((iF & ItemStateFlags.NewerIdentityMask) == 0) continue;
                            break;
                        case ResourceUpdateMode.Soft:
                        case ResourceUpdateMode.Hard:
                            break;
                        default:
                            throw new IndexOutOfRangeException();
                    }
                    (ArtifactResourceInfo versionedResource, ItemStateFlags rF) = await ArtifactTool.DetermineUpdatedResourceAsync(resource, Options.ResourceUpdate, cancellationToken).ConfigureAwait(false);
                    LogHandler.Log(i.Key.Tool, i.Key.Group, $"-- {((rF & ItemStateFlags.NewerIdentityMask) != 0 ? "[NEW] " : "")}{versionedResource.GetInfoString()}", null, LogLevel.Title);
                    if ((rF & ItemStateFlags.NewerIdentityMask) != 0 && versionedResource.Exportable)
                    {
                        await using Stream stream = await ArtifactTool.CreateOutputStreamAsync(versionedResource.Key, cancellationToken).ConfigureAwait(false);
                        await using Stream dataStream = await versionedResource.ExportStreamAsync(cancellationToken).ConfigureAwait(false);
                        await dataStream.CopyToAsync(stream, cancellationToken).ConfigureAwait(false);
                    }
                    if ((rF & ItemStateFlags.DifferentMask) != 0)
                        await ArtifactTool.AddResourceAsync(versionedResource, cancellationToken).ConfigureAwait(false);
                }
                if ((iF & ItemStateFlags.NewerIdentityMask) != 0)
                    await ArtifactTool.AddArtifactAsync(data.Info, cancellationToken).ConfigureAwait(false);
            }
            return;
        }
        throw new NotSupportedException("Artifact tool is not a supported type");
    }
}
