using System.Security.Cryptography;
using Art.Common.Management;
using Art.Common.Proxies;

namespace Art.Common;

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
        foreach (ArtifactToolProfile profile in ArtifactToolProfileLoader.DeserializeProfilesFromFile(artifactToolProfilePath))
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
    /// <exception cref="ArgumentException">Thrown when an invalid profile is provided.</exception>
    /// <exception cref="ArtifactToolNotFoundException">Thrown when tool is not found.</exception>
    public static async Task DumpAsync(ArtifactToolProfile artifactToolProfile, ArtifactRegistrationManager artifactRegistrationManager, ArtifactDataManager artifactDataManager, ArtifactToolDumpOptions? dumpOptions = null, IToolLogHandler? toolLogHandler = null, CancellationToken cancellationToken = default)
    {
        if (artifactToolProfile.Group == null) throw new ArgumentException("Group not specified in profile");
        using ArtifactToolBase tool = await ArtifactToolBase.PrepareToolAsync(artifactToolProfile, artifactRegistrationManager, artifactDataManager, cancellationToken);
        await new ArtifactToolDumpProxy(tool, dumpOptions ?? new ArtifactToolDumpOptions(), toolLogHandler).DumpAsync(cancellationToken);
    }

    /// <summary>
    /// Dumps an artifact asynchronously.
    /// </summary>
    /// <param name="artifactTool">Origin artifact tool.</param>
    /// <param name="artifactData">Artifact data to dump.</param>
    /// <param name="resourceUpdate">Resource update mode.</param>
    /// <param name="checksumId">Checksum algorithm ID.</param>
    /// <param name="eagerFlags">Eager flags.</param>
    /// <param name="logHandler">Log handler.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="resourceUpdate"/> is invalid.</exception>
    public static async Task DumpArtifactAsync(this ArtifactToolBase artifactTool, ArtifactData artifactData, ResourceUpdateMode resourceUpdate = ResourceUpdateMode.Soft, string? checksumId = null, EagerFlags eagerFlags = EagerFlags.None, IToolLogHandler? logHandler = null, CancellationToken cancellationToken = default)
    {
        switch (resourceUpdate)
        {
            case ResourceUpdateMode.ArtifactSoft:
            case ResourceUpdateMode.ArtifactHard:
            case ResourceUpdateMode.Soft:
            case ResourceUpdateMode.Hard:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(resourceUpdate));
        }
        ItemStateFlags iF = await artifactTool.CompareArtifactAsync(artifactData.Info, cancellationToken).ConfigureAwait(false);
        logHandler?.Log(artifactTool.Profile.Tool, artifactTool.Profile.Group, $"{((iF & ItemStateFlags.NewerIdentityMask) != 0 ? "[NEW] " : "")}{artifactData.Info.GetInfoTitleString()}", artifactData.Info.GetInfoString(), LogLevel.Entry);
        if ((iF & ItemStateFlags.NewerIdentityMask) != 0)
            await artifactTool.RegistrationManager.AddArtifactAsync(artifactData.Info with { Full = false }, cancellationToken).ConfigureAwait(false);
        switch (resourceUpdate)
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
                        await UpdateResourceAsync(artifactTool, await artifactTool.DetermineUpdatedResourceAsync(v, resourceUpdate, cancellationToken), logHandler, checksumId, cancellationToken)).ToArray();
                    await Task.WhenAll(tasks);
                    break;
                }
            case EagerFlags.ResourceMetadata:
                Task<ArtifactResourceInfoWithState>[] updateTasks = artifactData.Values.Select(v => artifactTool.DetermineUpdatedResourceAsync(v, resourceUpdate, cancellationToken)).ToArray();
                ArtifactResourceInfoWithState[] items = await Task.WhenAll(updateTasks);
                foreach (ArtifactResourceInfoWithState aris in items)
                    await UpdateResourceAsync(artifactTool, aris, logHandler, checksumId, cancellationToken);
                break;
            case EagerFlags.ResourceObtain:
                {
                    List<Task> tasks = new();
                    foreach (ArtifactResourceInfo resource in artifactData.Values)
                    {
                        ArtifactResourceInfoWithState aris = await artifactTool.DetermineUpdatedResourceAsync(resource, resourceUpdate, cancellationToken).ConfigureAwait(false);
                        tasks.Add(UpdateResourceAsync(artifactTool, aris, logHandler, checksumId, cancellationToken));
                    }
                    await Task.WhenAll(tasks);
                    break;
                }
            default:
                {
                    foreach (ArtifactResourceInfo resource in artifactData.Values)
                        await UpdateResourceAsync(artifactTool, await artifactTool.DetermineUpdatedResourceAsync(resource, resourceUpdate, cancellationToken).ConfigureAwait(false), logHandler, checksumId, cancellationToken);
                    break;
                }
        }
        SaveArtifact:
        if ((iF & ItemStateFlags.NewerIdentityMask) != 0)
            await artifactTool.RegistrationManager.AddArtifactAsync(artifactData.Info, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Dumps a resource asynchronously.
    /// </summary>
    /// <param name="artifactTool">Artifact tool.</param>
    /// <param name="resource">Resource to check.</param>
    /// <param name="resourceUpdate">Resource update mode.</param>
    /// <param name="logHandler">Log handler.</param>
    /// <param name="checksumId">Checksum algorithm ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public static async Task DumpResourceAsync(this ArtifactToolBase artifactTool, ArtifactResourceInfo resource, ResourceUpdateMode resourceUpdate, IToolLogHandler? logHandler, string? checksumId, CancellationToken cancellationToken = default)
    {
        await UpdateResourceAsync(artifactTool, await artifactTool.DetermineUpdatedResourceAsync(resource, resourceUpdate, cancellationToken).ConfigureAwait(false), logHandler, checksumId, cancellationToken);
    }

    private static async Task UpdateResourceAsync(ArtifactToolBase artifactTool, ArtifactResourceInfoWithState aris, IToolLogHandler? logHandler, string? checksumId, CancellationToken cancellationToken)
    {
        (ArtifactResourceInfo versionedResource, ItemStateFlags rF) = aris;
        if ((rF & ItemStateFlags.NewerIdentityMask) != 0 && versionedResource.Exportable)
        {
            OutputStreamOptions options = OutputStreamOptions.Default;
            versionedResource.AugmentOutputStreamOptions(ref options);
            await using CommittableStream stream = await artifactTool.DataManager.CreateOutputStreamAsync(versionedResource.Key, options, cancellationToken).ConfigureAwait(false);
            if (checksumId != null && ChecksumSource.TryGetHashAlgorithm(checksumId, out HashAlgorithm? algorithm))
            {
                // Take this opportunity to hash the resource.
                await using HashProxyStream hps = new(stream, algorithm, true);
                await versionedResource.ExportStreamAsync(hps, cancellationToken).ConfigureAwait(false);
                stream.ShouldCommit = true;
                Checksum newChecksum = new(checksumId, hps.GetHash());
                if (!Checksum.DatawiseEquals(newChecksum, versionedResource.Checksum))
                {
                    rF |= ItemStateFlags.NewChecksum;
                    versionedResource = versionedResource with { Checksum = newChecksum };
                }
            }
            else if (stream is not CommittableSinkStream) // if target output were a sink stream and hash isn't needed, then just don't bother exporting
            {
                await versionedResource.ExportStreamAsync(stream, cancellationToken).ConfigureAwait(false);
                stream.ShouldCommit = true;
            }
        }
        logHandler?.Log(artifactTool.Profile.Tool, artifactTool.Profile.Group, $"-- {((rF & ItemStateFlags.NewerIdentityMask) != 0 ? "[NEW] " : "")}{versionedResource.GetInfoPathString()}", versionedResource.GetInfoString(), LogLevel.Entry);
        if ((rF & ItemStateFlags.DifferentMask) != 0)
            await artifactTool.RegistrationManager.AddResourceAsync(versionedResource, cancellationToken).ConfigureAwait(false);
    }
}
