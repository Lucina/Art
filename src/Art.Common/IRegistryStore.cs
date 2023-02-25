namespace Art.Common;

/// <summary>
/// Represents a store of <see cref="IArtifactToolRegistry"/>.
/// </summary>
public interface IRegistryStore
{
    /// <summary>
    /// Loads a registry that applies to the specified tool ID.
    /// </summary>
    /// <param name="artifactToolId">Tool ID to get a registry for.</param>
    /// <returns>Registry.</returns>
    IArtifactToolRegistry LoadRegistry(ArtifactToolID artifactToolId);

    /// <summary>
    /// Loads all available registries in this store.
    /// </summary>
    /// <returns>Registry sequence.</returns>
    IEnumerable<IArtifactToolRegistry> LoadAllRegistries();
}
