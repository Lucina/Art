namespace Art;

/// <summary>
/// Simple dumping process.
/// </summary>
public static class ArtifactDumping
{
    /// <summary>
    /// Directly dump using a dumping profile stored on disk.
    /// </summary>
    /// <param name="dumpingProfilePath">Path to dumping profile.</param>
    /// <param name="targetDirectory">Base directory.</param>
    /// <returns>Task.</returns>
    /// <exception cref="ArtifactDumperFactoryNotFoundException"></exception>
    public static async Task DumpAsync(string dumpingProfilePath, string targetDirectory)
    {
        ArtifactDumpingProfile? profile = Extensions.LoadFromFile<ArtifactDumpingProfile>(dumpingProfilePath);
        if (!ArtifactDumperFactoryLoader.TryLoad(profile, out ArtifactDumperFactory? fac))
            throw new ArtifactDumperFactoryNotFoundException(profile.Dumper);
        string targetDir = Path.Combine(targetDirectory, profile.TargetFolder);
        var srm = new DiskArtifactRegistrationManager(targetDir);
        var sdm = new DiskArtifactDataManager(targetDir);
        ArtifactDumper? dumper = await fac.Create(srm, sdm, profile);
        await dumper.DumpAsync();
    }
}
