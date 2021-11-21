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
    /// <exception cref="ArtifactToolNotFoundException"></exception>
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
        if (artifactToolProfile.Group == null) throw new IOException("Group not specified in profile");
        if (!ArtifactToolLoader.TryLoad(artifactToolProfile, out ArtifactTool? t))
            throw new ArtifactToolNotFoundException(artifactToolProfile.Tool);
        var srm = new DiskArtifactRegistrationManager(targetDirectory);
        var sdm = new DiskArtifactDataManager(targetDirectory);
        ArtifactToolRuntimeConfig config = new(srm, sdm, artifactToolProfile);
        using ArtifactTool? tool = t;
        await tool.ConfigureAsync(config).ConfigureAwait(false);
        tool.LogHandler = ConsoleLogHandler.Default;
        await tool.DumpAsync();
    }
}
