using System.Text.Json;

namespace Art;

/// <summary>
/// Represents an artifact dumping profile.
/// </summary>
/// <param name="AssemblyName">Artifact dumper factory assembly name.</param>
/// <param name="FactoryTypeName">Artifact dumper factory type name.</param>
/// <param name="TargetFolder">Destination subfolder name.</param>
/// <param name="Options">Dumper-specific options.</param>
public record ArtifactDumpingProfile(
    string AssemblyName,
    string FactoryTypeName,
    string TargetFolder,
    Dictionary<string, JsonElement> Options
    );
