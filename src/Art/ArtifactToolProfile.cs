using System.Text.Json;

namespace Art;

/// <summary>
/// Represents an artifact tool profile.
/// </summary>
/// <param name="Tool">Artifact tool target string (assembly::factoryType).</param>
/// <param name="TargetFolder">Destination subfolder name.</param>
/// <param name="Options">Tool-specific options.</param>
public record ArtifactToolProfile(
    string Tool,
    string? TargetFolder,
    Dictionary<string, JsonElement>? Options
    );
