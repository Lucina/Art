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
    /// <param name="artifactToolProfilePath">Path to tool profile.</param>
    /// <param name="targetDirectory">Base directory.</param>
    /// <param name="options">Runtime tool options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    /// <exception cref="ArtifactToolNotFoundException"></exception>
    public static async Task DumpAsync(string artifactToolProfilePath, string targetDirectory, ArtifactToolRuntimeOptions? options = null, CancellationToken cancellationToken = default)
    {
        JsonElement element = ArtExtensions.LoadFromFile<JsonElement>(artifactToolProfilePath);
        options ??= new ArtifactToolRuntimeOptions();
        if (element.ValueKind == JsonValueKind.Object)
            await DumpAsync(element.Deserialize<ArtifactToolProfile>(ArtJsonOptions.s_jsonOptions)!, targetDirectory, options, cancellationToken: cancellationToken).ConfigureAwait(false);
        else
            foreach (ArtifactToolProfile profile in element.Deserialize<List<ArtifactToolProfile>>(ArtJsonOptions.s_jsonOptions)!)
                await DumpAsync(profile, targetDirectory, options, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Directly dump to disk using a tool profile.
    /// </summary>
    /// <param name="artifactToolProfile">Tool profile.</param>
    /// <param name="targetDirectory">Base directory.</param>
    /// <param name="options">Runtime tool options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    /// <exception cref="ArtifactToolNotFoundException"></exception>
    public static async Task DumpAsync(ArtifactToolProfile artifactToolProfile, string targetDirectory, ArtifactToolRuntimeOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (artifactToolProfile.Group == null) throw new IOException("Group not specified in profile");
        if (!ArtifactToolLoader.TryLoad(artifactToolProfile, out ArtifactTool? t))
            throw new ArtifactToolNotFoundException(artifactToolProfile.Tool);
        var srm = new DiskArtifactRegistrationManager(targetDirectory);
        var sdm = new DiskArtifactDataManager(targetDirectory);
        ArtifactToolRuntimeConfig config = new(srm, sdm, artifactToolProfile, options ?? new ArtifactToolRuntimeOptions());
        using ArtifactTool? tool = t;
        tool.LogHandler = ConsoleLogHandler.Default;
        await tool.ConfigureAsync(config, cancellationToken).ConfigureAwait(false);
        await tool.DumpAsync(cancellationToken);
    }

    /// <summary>
    /// Creates a tool profile for the specified tool.
    /// </summary>
    /// <param name="tool">Target tool string.</param>
    /// <param name="group">Target group.</param>
    /// <param name="options">Options.</param>
    /// <returns>Profile.</returns>
    public static ArtifactToolProfile CreateProfile(string tool, string group, params (string, JsonElement)[] options)
       => new(tool, group, options.ToDictionary(v => v.Item1, v => v.Item2));

    /// <summary>
    /// Creates a tool profile for the specified tool.
    /// </summary>
    /// <typeparam name="TTool">Tool type.</typeparam>
    /// <param name="group">Target group.</param>
    /// <param name="options">Options.</param>
    /// <returns>Profile.</returns>
    public static ArtifactToolProfile CreateProfile<TTool>(string group, params (string, JsonElement)[] options) where TTool : ArtifactTool
        => new(CreateToolString<TTool>(), group, options.ToDictionary(v => v.Item1, v => v.Item2));

    /// <summary>
    /// Creates a <see cref="JsonElement"/> from this value.
    /// </summary>
    /// <typeparam name="T">Value type.</typeparam>
    /// <param name="value">Value.</param>
    /// <returns>JSON element.</returns>
    public static JsonElement J<T>(this T value) => JsonSerializer.SerializeToElement(value);

    /// <summary>
    /// Creates a tool string for the specified tool.
    /// </summary>
    /// <typeparam name="TTool">Tool type.</typeparam>
    /// <returns>Tool string.</returns>
    public static string CreateToolString<TTool>() where TTool : ArtifactTool
    {
        Type t = typeof(TTool);
        string assemblyName = t.Assembly.GetName().Name ?? throw new InvalidOperationException();
        string typeName = t.FullName ?? throw new InvalidOperationException();
        return $"{assemblyName}::{typeName}";
    }
}
