namespace Art;

/// <summary>
/// Represents a resource key.
/// </summary>
/// <param name="File">Filename.</param>
/// <param name="Path">File path.</param>
/// <param name="InArtifactFolder">True if resourc is in artifact folder.</param>
public record struct RK(string File, string? Path = null, bool InArtifactFolder = true);

