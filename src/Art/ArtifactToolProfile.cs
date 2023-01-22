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
    IReadOnlyDictionary<string, JsonElement>? Options
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
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="tool"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if this instance has an invalid <paramref name="tool"/> value.</exception>
    public static (string assembly, string type) GetId(string tool)
    {
        if (tool == null) throw new ArgumentNullException(nameof(tool));
        if (s_toolRegex.Match(tool) is not { Success: true } match)
            throw new ArgumentException("Tool string is in invalid format, must be \"<assembly>::<toolType>\"", nameof(tool));
        return (match.Groups[1].Value, match.Groups[2].Value);
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
    public static ArtifactToolProfile Create<TTool>(string group, params (string, JsonElement)[] options) where TTool : ArtifactToolBase
        => new(ArtifactToolBase.CreateToolString<TTool>(), group, options.ToDictionary(v => v.Item1, v => v.Item2));

    /// <summary>
    /// Creates a tool profile for the specified tool.
    /// </summary>
    /// <param name="toolType">Tool type.</param>
    /// <param name="group">Target group.</param>
    /// <param name="options">Options.</param>
    /// <returns>Profile.</returns>
    /// <remarks>
    /// This overload sets <see cref="Options"/> to null if no options are specified.
    /// </remarks>
    public static ArtifactToolProfile Create(Type toolType, string group, params (string, JsonElement)[] options)
        => CreateInternal(toolType, group, options, false);

    /// <summary>
    /// Creates a tool profile for the specified tool.
    /// </summary>
    /// <param name="toolType">Tool type.</param>
    /// <param name="group">Target group.</param>
    /// <param name="options">Options.</param>
    /// <returns>Profile.</returns>
    /// <remarks>
    /// This overload sets <see cref="Options"/> to a valid dictionary even if no options are specified.
    /// </remarks>
    public static ArtifactToolProfile CreateWithOptions(Type toolType, string group, params (string, JsonElement)[] options)
        => CreateInternal(toolType, group, options, true);

    private static ArtifactToolProfile CreateInternal(Type toolType, string group, (string, JsonElement)[] options, bool alwaysOptions)
        => new(ArtifactToolBase.CreateToolString(toolType), group, options.Length == 0 ? alwaysOptions ? new Dictionary<string, JsonElement>() : null : options.ToDictionary(v => v.Item1, v => v.Item2));

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
    public ArtifactToolProfile WithCoreTool(Type type) => this with { Tool = ArtifactToolBase.CreateCoreToolString(type) };

    /// <summary>
    /// Creates an instance of this profile with specified comparer.
    /// </summary>
    /// <param name="comparer">Comparer to use.</param>
    /// <returns>Profile</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="comparer"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if duplicate keys are encountered using the specified comparer.</exception>
    public ArtifactToolProfile WithOptionsComparer(StringComparer comparer)
    {
        if (comparer == null) throw new ArgumentNullException(nameof(comparer));
        return Options == null ? new ArtifactToolProfile(this) : this with { Options = new Dictionary<string, JsonElement>(Options, comparer) };
    }

    /// <summary>
    /// Creates a new options dictionary for this profile with the specified comparer.
    /// </summary>
    /// <param name="comparer">Comparer to use.</param>
    /// <returns>Profile</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="comparer"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if duplicate keys are encountered using the specified comparer.</exception>
    public Dictionary<string, JsonElement> GetOptionsWithOptionsComparer(StringComparer comparer)
    {
        if (comparer == null) throw new ArgumentNullException(nameof(comparer));
        return Options == null ? new Dictionary<string, JsonElement>() : new Dictionary<string, JsonElement>(Options, comparer);
    }
}
