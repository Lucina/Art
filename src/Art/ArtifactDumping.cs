using System.Text.Json;

namespace Art;

/// <summary>
/// Simple dumping process.
/// </summary>
public static class ArtifactDumping
{
    /// <summary>
    /// Directly dump using a tool profile stored on disk.
    /// </summary>
    /// <param name="toolProfilePath">Path to tool profile.</param>
    /// <param name="targetDirectory">Base directory.</param>
    /// <returns>Task.</returns>
    /// <exception cref="ArtifactToolFactoryNotFoundException"></exception>
    public static async ValueTask DumpAsync(string toolProfilePath, string targetDirectory)
    {
        JsonElement element = ArtExtensions.LoadFromFile<JsonElement>(toolProfilePath);
        if (element.ValueKind == JsonValueKind.Object)
            await DumpAsync(element.Deserialize<ArtifactToolProfile>(ArtJsonOptions.JsonOptions)!, targetDirectory).ConfigureAwait(false);
        else
            foreach (ArtifactToolProfile profile in element.Deserialize<List<ArtifactToolProfile>>(ArtJsonOptions.JsonOptions)!)
                await DumpAsync(profile, targetDirectory).ConfigureAwait(false);
    }

    private static async ValueTask DumpAsync(ArtifactToolProfile artifactToolProfile, string targetDirectory)
    {
        if (artifactToolProfile.TargetFolder == null) throw new IOException("Target folder not specified in profile");
        if (!s_tools.TryGetValue(artifactToolProfile.Tool, out ArtifactToolFactory? fac))
        {
            if (!ArtifactToolFactoryLoader.TryLoad(artifactToolProfile, out fac))
                throw new ArtifactToolFactoryNotFoundException(artifactToolProfile.Tool);
            s_tools.Add(artifactToolProfile.Tool, fac);
        }
        string targetDir = Path.Combine(targetDirectory, artifactToolProfile.TargetFolder);
        var srm = new DiskArtifactRegistrationManager(targetDir);
        var sdm = new DiskArtifactDataManager(targetDir);
        using ArtifactTool? tool = await fac.CreateAsync(srm, sdm, artifactToolProfile);
        tool.LogHandler = ConsoleLogHandler.Default;
        await tool.DumpAsync();
    }

    private static readonly Dictionary<string, ArtifactToolFactory> s_tools = new();
}
