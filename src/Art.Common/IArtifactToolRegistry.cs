using System.Diagnostics.CodeAnalysis;

namespace Art.Common;

/// <summary>
/// Represents a registry of artifact tools which can be created as needed.
/// </summary>
public interface IArtifactToolRegistry
{
    /// <summary>
    /// Attempts to create an artifact tool for the specified name.
    /// </summary>
    /// <param name="artifactToolId">Artifact tool ID.</param>
    /// <param name="tool">Crated artifact tool, if successful.</param>
    /// <returns>True if successful.</returns>
    bool TryLoad(ArtifactToolID artifactToolId, [NotNullWhen(true)] out IArtifactTool? tool);
}
