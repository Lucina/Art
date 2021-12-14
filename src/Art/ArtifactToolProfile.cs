using System.Text.Json;
using System.Text.RegularExpressions;

namespace Art;

/// <summary>
/// Represents an artifact tool profile.
/// </summary>
/// <param name="Tool">Artifact tool target string (assembly::toolType).</param>
/// <param name="Group">Destination group.</param>
/// <param name="Options">Tool-specific options.</param>
public record ArtifactToolProfile(
    string Tool,
    string Group,
    Dictionary<string, JsonElement>? Options
)
{
    private static readonly Regex s_toolRegex = new(@"^([\S\s]+)::([\S\s]+)$");

    /// <summary>
    /// Separates assembly and type name from <see cref="Tool"/>.
    /// </summary>
    /// <returns>Separated assembly and type name.</returns>
    /// <exception cref="ArgumentException">Thrown if this instance has an invalid <see cref="Tool"/> value.</exception>
    public (string assembly, string type) GetId() => GetId(Tool);

    /// <summary>
    /// Separates assembly and type name from <see cref="Tool"/>.
    /// </summary>
    /// <param name="tool">Artifact tool target string(assembly::toolType)</param>
    /// <returns>Separated assembly and type name.</returns>
    /// <exception cref="ArgumentException">Thrown if this instance has an invalid <paramref name="tool"/> value.</exception>
    public static (string assembly, string type) GetId(string tool)
    {
        if (s_toolRegex.Match(tool) is not { Success: true } match)
            throw new ArgumentException("Tool string is in invalid format, must be \"<assembly>::<toolType>\"", nameof(tool));
        return (match.Groups[1].Value, match.Groups[2].Value);
    }

    /// <summary>
    /// Deserializes profiles.
    /// </summary>
    /// <param name="path">Path to file containing profile or profile array.</param>
    /// <returns>Array of profiles.</returns>
    /// <exception cref="InvalidDataException">Thrown if null value encountered.</exception>
    public static ArtifactToolProfile[] DeserializeProfilesFromFile(string path)
    {
        return DeserializeProfiles(JsonSerializer.Deserialize<JsonElement>(File.ReadAllText(path)));
    }

    /// <summary>
    /// Deserializes profiles.
    /// </summary>
    /// <param name="utf8Stream">UTF-8 stream containing profile or profile array.</param>
    /// <returns>Array of profiles.</returns>
    /// <exception cref="InvalidDataException">Thrown if null value encountered.</exception>
    public static ArtifactToolProfile[] DeserializeProfiles(Stream utf8Stream)
    {
        return DeserializeProfiles(JsonSerializer.Deserialize<JsonElement>(utf8Stream));
    }

    /// <summary>
    /// Deserializes profiles.
    /// </summary>
    /// <param name="element">Element containing profile or profile array.</param>
    /// <returns>Array of profiles.</returns>
    /// <exception cref="InvalidDataException">Thrown if null value encountered.</exception>
    public static ArtifactToolProfile[] DeserializeProfiles(JsonElement element)
    {
        if (element.ValueKind == JsonValueKind.Object)
            return new[] { element.Deserialize<ArtifactToolProfile>(ArtJsonOptions.s_jsonOptions) ?? throw new InvalidDataException() };
        else
            return element.Deserialize<ArtifactToolProfile[]>(ArtJsonOptions.s_jsonOptions) ?? throw new InvalidDataException();
    }

    /// <summary>
    /// Creates a tool profile for the specified tool.
    /// </summary>
    /// <param name="tool">Target tool string.</param>
    /// <param name="group">Target group.</param>
    /// <param name="options">Options.</param>
    /// <returns>Profile.</returns>
    public static ArtifactToolProfile Create(string tool, string group, params (string, JsonElement)[] options)
        => new(tool, group, options.ToDictionary(v => v.Item1, v => v.Item2));

    /// <summary>
    /// Creates a tool profile for the specified tool.
    /// </summary>
    /// <typeparam name="TTool">Tool type.</typeparam>
    /// <param name="group">Target group.</param>
    /// <param name="options">Options.</param>
    /// <returns>Profile.</returns>
    public static ArtifactToolProfile Create<TTool>(string group, params (string, JsonElement)[] options) where TTool : ArtifactTool
        => new(ArtifactTool.CreateToolString<TTool>(), group, options.ToDictionary(v => v.Item1, v => v.Item2));

    /// <summary>
    /// Creates an instance of this profile with most derived core type of instance or instance's type.
    /// </summary>
    /// <param name="instance">Instance to derive tool type from.</param>
    /// <returns>Profile.</returns>
    public ArtifactToolProfile WithCoreTool(object instance) => WithCoreTool(instance.GetType());

    /// <summary>
    /// Creates an instance of this profile with most derived core type or given type.
    /// </summary>
    /// <param name="type">Tool type.</param>
    /// <returns>Profile.</returns>
    public ArtifactToolProfile WithCoreTool(Type type) => this with { Tool = ArtifactTool.CreateCoreToolString(type) };
}
