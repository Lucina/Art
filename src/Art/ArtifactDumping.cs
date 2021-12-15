namespace Art;

/// <summary>
/// Simple dumping process.
/// </summary>
public static class ArtifactDumping
{
    /// <summary>
    /// Directly dumps using a tool profile stored on disk.
    /// </summary>
    /// <param name="artifactToolProfilePath">Path to tool profile.</param>
    /// <param name="targetDirectory">Base directory.</param>
    /// <param name="dumpOptions">Dump options.</param>
    /// <param name="toolLogHandler">Tool log handler.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    /// <exception cref="ArtifactToolNotFoundException"></exception>
    public static async Task DumpAsync(string artifactToolProfilePath, string targetDirectory, ArtifactToolDumpOptions? dumpOptions = null, IToolLogHandler? toolLogHandler = null, CancellationToken cancellationToken = default)
    {
        dumpOptions ??= new ArtifactToolDumpOptions();
        var srm = new DiskArtifactRegistrationManager(targetDirectory);
        var sdm = new DiskArtifactDataManager(targetDirectory);
        foreach (ArtifactToolProfile profile in ArtifactToolProfile.DeserializeProfilesFromFile(artifactToolProfilePath))
            await DumpAsync(profile, srm, sdm, dumpOptions, toolLogHandler, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Directly dumps to disk using a tool profile.
    /// </summary>
    /// <param name="artifactToolProfile">Tool profile.</param>
    /// <param name="artifactRegistrationManager">Registration manager.</param>
    /// <param name="artifactDataManager">Data manager.</param>
    /// <param name="dumpOptions">Dump options.</param>
    /// <param name="toolLogHandler">Tool log handler.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    /// <exception cref="ArtifactToolNotFoundException"></exception>
    public static async Task DumpAsync(ArtifactToolProfile artifactToolProfile, ArtifactRegistrationManager artifactRegistrationManager, ArtifactDataManager artifactDataManager, ArtifactToolDumpOptions? dumpOptions = null, IToolLogHandler? toolLogHandler = null, CancellationToken cancellationToken = default)
    {
        if (artifactToolProfile.Group == null) throw new IOException("Group not specified in profile");
        if (!ArtifactToolLoader.TryLoad(artifactToolProfile, out ArtifactTool? t))
            throw new ArtifactToolNotFoundException(artifactToolProfile.Tool);
        ArtifactToolConfig config = new(artifactRegistrationManager, artifactDataManager);
        using ArtifactTool tool = t;
        artifactToolProfile = artifactToolProfile.WithCoreTool(t);
        await tool.InitializeAsync(config, artifactToolProfile, cancellationToken).ConfigureAwait(false);
        await new ArtifactToolDumpProxy(tool, dumpOptions ?? new ArtifactToolDumpOptions(), toolLogHandler).DumpAsync(cancellationToken);
    }


    /// <summary>
    /// Dumps an artifact asynchronously.
    /// </summary>
    /// <param name="artifactTool">Origin artifact tool.</param>
    /// <param name="artifactData">Artifact data to dump.</param>
    /// <param name="resourceUpdateMode">Resource update mode.</param>
    /// <param name="eagerFlags">Eager flags.</param>
    /// <param name="logHandler">Log handler.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="resourceUpdateMode"/> is invalid.</exception>
    public static async Task DumpArtifactAsync(this ArtifactTool artifactTool, ArtifactData artifactData, ResourceUpdateMode resourceUpdateMode = ResourceUpdateMode.Soft, EagerFlags eagerFlags = EagerFlags.None, IToolLogHandler? logHandler = null, CancellationToken cancellationToken = default)
    {
        switch (resourceUpdateMode)
        {
            case ResourceUpdateMode.ArtifactSoft:
            case ResourceUpdateMode.ArtifactHard:
            case ResourceUpdateMode.Soft:
            case ResourceUpdateMode.Hard:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(resourceUpdateMode));
        }
        ItemStateFlags iF = await artifactTool.CompareArtifactAsync(artifactData.Info, cancellationToken).ConfigureAwait(false);
        logHandler?.Log(artifactTool.Profile.Tool, artifactTool.Profile.Group, $"{((iF & ItemStateFlags.NewerIdentityMask) != 0 ? "[NEW] " : "")}{artifactData.Info.GetInfoString()}", null, LogLevel.Entry);
        if ((iF & ItemStateFlags.NewerIdentityMask) != 0)
            await artifactTool.AddArtifactAsync(artifactData.Info with { Full = false }, cancellationToken).ConfigureAwait(false);
        switch (resourceUpdateMode)
        {
            case ResourceUpdateMode.ArtifactSoft:
            case ResourceUpdateMode.ArtifactHard:
                if ((iF & ItemStateFlags.NewerIdentityMask) == 0) goto SaveArtifact;
                break;
            case ResourceUpdateMode.Soft:
            case ResourceUpdateMode.Hard:
                break;
        }
        switch (eagerFlags & artifactTool.AllowedEagerModes & (EagerFlags.ResourceMetadata | EagerFlags.ResourceObtain))
        {
            case EagerFlags.ResourceMetadata | EagerFlags.ResourceObtain:
                {
                    Task[] tasks = artifactData.Values.Select(async v =>
                        await UpdateResourceAsync(artifactTool, await artifactTool.DetermineUpdatedResourceAsync(v, resourceUpdateMode, cancellationToken), logHandler, cancellationToken)).ToArray();
                    await Task.WhenAll(tasks);
                    break;
                }
            case EagerFlags.ResourceMetadata:
                Task<ArtifactResourceInfoWithState>[] updateTasks = artifactData.Values.Select(v => artifactTool.DetermineUpdatedResourceAsync(v, resourceUpdateMode, cancellationToken)).ToArray();
                ArtifactResourceInfoWithState[] items = await Task.WhenAll(updateTasks);
                foreach (ArtifactResourceInfoWithState aris in items)
                    await UpdateResourceAsync(artifactTool, aris, logHandler, cancellationToken);
                break;
            case EagerFlags.ResourceObtain:
                {
                    List<ArtifactResourceInfoWithState> values = new();
                    foreach (ArtifactResourceInfo resource in artifactData.Values)
                        values.Add(await artifactTool.DetermineUpdatedResourceAsync(resource, resourceUpdateMode, cancellationToken).ConfigureAwait(false));
                    Task[] tasks = values.Select(v => UpdateResourceAsync(artifactTool, v, logHandler, cancellationToken)).ToArray();
                    await Task.WhenAll(tasks);
                    break;
                }
            default:
                {
                    foreach (ArtifactResourceInfo resource in artifactData.Values)
                        await UpdateResourceAsync(artifactTool, await artifactTool.DetermineUpdatedResourceAsync(resource, resourceUpdateMode, cancellationToken).ConfigureAwait(false), logHandler, cancellationToken);
                    break;
                }
        }
        SaveArtifact:
        if ((iF & ItemStateFlags.NewerIdentityMask) != 0)
            await artifactTool.AddArtifactAsync(artifactData.Info, cancellationToken).ConfigureAwait(false);
    }

    private static async Task UpdateResourceAsync(ArtifactTool artifactTool, ArtifactResourceInfoWithState aris, IToolLogHandler? logHandler, CancellationToken cancellationToken)
    {
        (ArtifactResourceInfo versionedResource, ItemStateFlags rF) = aris;
        logHandler?.Log(artifactTool.Profile.Tool, artifactTool.Profile.Group, $"-- {((rF & ItemStateFlags.NewerIdentityMask) != 0 ? "[NEW] " : "")}{versionedResource.GetInfoString()}", null, LogLevel.Entry);
        if ((rF & ItemStateFlags.NewerIdentityMask) != 0 && versionedResource.Exportable)
        {
            await using Stream stream = await artifactTool.CreateOutputStreamAsync(versionedResource.Key, cancellationToken).ConfigureAwait(false);
            await using Stream dataStream = await versionedResource.ExportStreamAsync(cancellationToken).ConfigureAwait(false);
            await dataStream.CopyToAsync(stream, cancellationToken).ConfigureAwait(false);
        }
        if ((rF & ItemStateFlags.DifferentMask) != 0)
            await artifactTool.AddResourceAsync(versionedResource, cancellationToken).ConfigureAwait(false);
    }
}
