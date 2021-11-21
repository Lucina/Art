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
}
