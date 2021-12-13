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
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    /// <exception cref="ArtifactToolNotFoundException"></exception>
    public static async Task DumpAsync(string artifactToolProfilePath, string targetDirectory, ArtifactToolDumpOptions? dumpOptions = null, CancellationToken cancellationToken = default)
    {
        dumpOptions ??= new ArtifactToolDumpOptions();
        foreach (ArtifactToolProfile profile in ArtifactToolProfile.DeserializeProfilesFromFile(artifactToolProfilePath))
            await DumpAsync(profile, targetDirectory, dumpOptions, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Directly dumps to disk using a tool profile.
    /// </summary>
    /// <param name="artifactToolProfile">Tool profile.</param>
    /// <param name="targetDirectory">Base directory.</param>
    /// <param name="dumpOptions">Dump options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    /// <exception cref="ArtifactToolNotFoundException"></exception>
    public static async Task DumpAsync(ArtifactToolProfile artifactToolProfile, string targetDirectory, ArtifactToolDumpOptions? dumpOptions = null, CancellationToken cancellationToken = default)
    {
        if (artifactToolProfile.Group == null) throw new IOException("Group not specified in profile");
        if (!ArtifactToolLoader.TryLoad(artifactToolProfile, out ArtifactTool? t))
            throw new ArtifactToolNotFoundException(artifactToolProfile.Tool);
        var srm = new DiskArtifactRegistrationManager(targetDirectory);
        var sdm = new DiskArtifactDataManager(targetDirectory);
        ArtifactToolConfig config = new(srm, sdm);
        using ArtifactTool tool = t;
        tool.LogHandler = ConsoleLogHandler.Default;
        artifactToolProfile = artifactToolProfile with { Tool = ArtifactTool.CreateCoreToolString(t.GetType()) };
        await tool.InitializeAsync(config, artifactToolProfile, cancellationToken).ConfigureAwait(false);
        await new ArtifactToolDumpProxy(tool, dumpOptions ?? new ArtifactToolDumpOptions()).DumpAsync(cancellationToken);
    }
}
