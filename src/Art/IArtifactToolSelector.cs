using System.Diagnostics.CodeAnalysis;

namespace Art;

/// <summary>
/// Represents a provider for identifying an applicable tool ID and artifact ID for a given input key.
/// </summary>
/// <remarks>
/// This is meant for extraction of a URL or something similar to an applicable key.
/// </remarks>
public interface IArtifactToolSelector<in TKey>
{
    /// <summary>
    /// Attempts to identify an applicable tool ID and artifact ID from an input key.
    /// </summary>
    /// <param name="key">Key to evaluate.</param>
    /// <param name="artifactToolId">Artifact tool ID, if successful.</param>
    /// <param name="artifactId">Artifact ID, if successful.</param>
    /// <returns>True if successful.</returns>
    static abstract bool TryIdentify(TKey key, [NotNullWhen(true)] out ArtifactToolID? artifactToolId, [NotNullWhen(true)] out string? artifactId);
}
