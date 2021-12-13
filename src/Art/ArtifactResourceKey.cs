namespace Art;

/// <summary>
/// Represents a key for an artifact resource.
/// </summary>
/// <param name="Artifact">Artifact key.</param>
/// <param name="File">Filename.</param>
/// <param name="Path">Path.</param>
public record ArtifactResourceKey(ArtifactKey Artifact, string File, string Path = "");
