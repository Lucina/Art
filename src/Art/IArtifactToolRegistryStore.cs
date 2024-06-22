using System.Diagnostics.CodeAnalysis;

namespace Art;

/// <summary>
/// Represents a store of <see cref="IArtifactToolRegistry"/>.
/// </summary>
public interface IArtifactToolRegistryStore
{
    /// <summary>
    /// Attempts to load a registry that applies to the specified tool ID.
    /// </summary>
    /// <param name="artifactToolId">Tool ID to get a registry for.</param>
    /// <param name="artifactToolRegistry">Registry, if successful.</param>
    /// <returns>True if successfully loaded registry.</returns>
    bool TryLoadRegistry(ArtifactToolID artifactToolId, [NotNullWhen(true)] out IArtifactToolRegistry? artifactToolRegistry);

    /// <summary>
    /// Loads all available registries in this store.
    /// </summary>
    /// <returns>Registry sequence.</returns>
    IEnumerable<IArtifactToolRegistry> LoadAllRegistries();
}
