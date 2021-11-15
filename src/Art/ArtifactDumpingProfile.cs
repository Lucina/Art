using System.Text.Json;

namespace Art;

/// <summary>
/// Represents an artifact dumping profile.
/// </summary>
/// <param name="Dumper">Artifact dumper target string (assembly::factoryType).</param>
/// <param name="TargetFolder">Destination subfolder name.</param>
/// <param name="Options">Dumper-specific options.</param>
public record ArtifactDumpingProfile(
    string Dumper,
    string TargetFolder,
    Dictionary<string, JsonElement> Options
    );
