namespace Art;

/// <summary>
/// Represents runtime options for tool operation.
/// </summary>
/// <param name="ResourceUpdate">Resource update mode.</param>
public record ArtifactToolRuntimeOptions(ResourceUpdateMode ResourceUpdate = ResourceUpdateMode.Artifact);
