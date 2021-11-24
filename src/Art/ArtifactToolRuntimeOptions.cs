namespace Art;

/// <summary>
/// Represents runtime options for tool operation.
/// </summary>
/// <param name="ResourceUpdate">Resource update mode.</param>
/// <param name="IncludeNonFull">Overwrite full entries with non-full if newer.</param>
public record ArtifactToolRuntimeOptions(ResourceUpdateMode ResourceUpdate = ResourceUpdateMode.ArtifactHard, bool IncludeNonFull = true);
