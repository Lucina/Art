using System.Text.Json;

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
);
