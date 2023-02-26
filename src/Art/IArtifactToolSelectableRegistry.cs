using System.Diagnostics.CodeAnalysis;

namespace Art;

/// <summary>
/// Represents a registry of artifact tools which can be created as needed and selected based on key.
/// </summary>
public interface IArtifactToolSelectableRegistry<in TKey> : IArtifactToolRegistry
{
    /// <summary>
    /// Attempts to identify an applicable tool ID and artifact ID from an input key.
    /// </summary>
    /// <param name="key">Key to evaluate.</param>
    /// <param name="artifactToolId">Artifact tool ID, if successful.</param>
    /// <param name="artifactId">Artifact ID, if successful.</param>
    /// <returns>True if successful.</returns>
    bool TryIdentify(TKey key, out ArtifactToolID artifactToolId, [NotNullWhen(true)] out string? artifactId);

    /// <summary>
    /// Attempts to load an applicable tool and identify an artifact ID from an input key.
    /// </summary>
    /// <param name="key">Key to evaluate.</param>
    /// <param name="tool">Artifact tool, if successful.</param>
    /// <param name="artifactId">Artifact ID, if successful.</param>
    /// <returns>True if successful.</returns>
    [Obsolete("Chain TryIdentify and TryLoad calls instead")]
    bool TryIdentifyAndLoad(TKey key, [NotNullWhen(true)] out IArtifactTool? tool, [NotNullWhen(true)] out string? artifactId)
    {
        if (TryIdentify(key, out ArtifactToolID artifactToolId, out artifactId) && TryLoad(artifactToolId, out tool))
        {
            return true;
        }
        tool = null;
        artifactId = null;
        return false;
    }
}
