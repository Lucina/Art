using System.Diagnostics.CodeAnalysis;

namespace Art.Common;

/// <summary>
/// Represents a registry of artifact tools which can be created as needed.
/// </summary>
public interface IArtifactToolRegistry
{
    /// <summary>
    /// Checks if tool entry with specified name is contained in this registry.
    /// </summary>
    /// <param name="artifactToolId">Artifact tool ID.</param>
    /// <returns>True if tool entry exists.</returns>
    bool Contains(ArtifactToolID artifactToolId);

    /// <summary>
    /// Attempts to create an artifact tool for the specified name.
    /// </summary>
    /// <param name="artifactToolId">Artifact tool ID.</param>
    /// <param name="tool">Crated artifact tool, if successful.</param>
    /// <returns>True if successful.</returns>
    bool TryLoad(ArtifactToolID artifactToolId, [NotNullWhen(true)] out IArtifactTool? tool);

    /// <summary>
    /// Gets available tool descriptions in this registry.
    /// </summary>
    /// <returns>Tool descriptions.</returns>
    IEnumerable<ToolDescription> GetToolDescriptions();
}
