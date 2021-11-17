using System.Text.Json;

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
    public static async ValueTask DumpAsync(string dumpingProfilePath, string targetDirectory)
    {
        JsonElement element = ArtExtensions.LoadFromFile<JsonElement>(dumpingProfilePath);
        if (element.ValueKind == JsonValueKind.Object)
            await DumpAsync(element.Deserialize<ArtifactDumpingProfile>(ArtJsonOptions.JsonOptions)!, targetDirectory).ConfigureAwait(false);
        else
            foreach (ArtifactDumpingProfile profile in element.Deserialize<List<ArtifactDumpingProfile>>(ArtJsonOptions.JsonOptions)!)
                await DumpAsync(profile, targetDirectory).ConfigureAwait(false);
    }

    private static async ValueTask DumpAsync(ArtifactDumpingProfile artifactDumpingProfile, string targetDirectory)
    {
        if (artifactDumpingProfile.TargetFolder == null) throw new IOException("Target folder not specified in profile");
        if (!s_dumpers.TryGetValue(artifactDumpingProfile.Dumper, out ArtifactDumperFactory? fac))
        {
            if (!ArtifactDumperFactoryLoader.TryLoad(artifactDumpingProfile, out fac))
                throw new ArtifactDumperFactoryNotFoundException(artifactDumpingProfile.Dumper);
            s_dumpers.Add(artifactDumpingProfile.Dumper, fac);
        }
        string targetDir = Path.Combine(targetDirectory, artifactDumpingProfile.TargetFolder);
        var srm = new DiskArtifactRegistrationManager(targetDir);
        var sdm = new DiskArtifactDataManager(targetDir);
        using ArtifactDumper? dumper = await fac.CreateAsync(srm, sdm, artifactDumpingProfile);
        dumper.LogHandler = ConsoleLogHandler.Default;
        await dumper.RunAsync();
    }

    private static readonly Dictionary<string, ArtifactDumperFactory> s_dumpers = new();
}
