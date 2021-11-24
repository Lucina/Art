namespace Art;

/// <summary>
/// Represents a key for an artifact.
/// </summary>
/// <param name="Tool">Tool id.</param>
/// <param name="Group">Group.</param>
/// <param name="Id">Artifact ID.</param>
public record ArtifactKey(string Tool, string Group, string Id);
