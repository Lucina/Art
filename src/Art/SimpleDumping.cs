namespace Art;

/// <summary>
/// Simple dumping process.
/// </summary>
public static class SimpleDumping
{
    /// <summary>
    /// Directly dump using a dumping profile stored on disk.
    /// </summary>
    /// <param name="dumpingProfilePath">Path to dumping profile.</param>
    /// <param name="targetDirectory">Base directory.</param>
    /// <returns>Task.</returns>
    /// <exception cref="ArtifactDumperFactoryNotFoundException"></exception>
    public static async Task Dump(string dumpingProfilePath, string targetDirectory)
    {
        var profile = Extensions.LoadFromFile<ArtifactDumpingProfile>(dumpingProfilePath);
        if (!ArtifactDumperFactoryLoader.TryLoad(profile, out var fac))
            throw new ArtifactDumperFactoryNotFoundException(profile.AssemblyName, profile.FactoryTypeName);
        var sdm = new SimpleArtifactDataManager(Path.Combine(targetDirectory, profile.TargetFolder));
        var dumper = await fac.Create(sdm, profile);
        await dumper.DumpAsync();
    }
}
