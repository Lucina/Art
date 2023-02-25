namespace Art.Common;

/// <summary>
/// Represents a store of a single <see cref="IArtifactToolRegistry"/>.
/// </summary>
public class StaticRegistryStore : IRegistryStore
{
    private readonly IArtifactToolRegistry _artifactToolRegistry;

    /// <summary>
    /// Initializes an instance of <see cref="StaticRegistryStore"/>.
    /// </summary>
    /// <param name="artifactToolRegistry">Registry to use.</param>
    public StaticRegistryStore(IArtifactToolRegistry artifactToolRegistry) => _artifactToolRegistry = artifactToolRegistry;

    /// <inheritdoc />
    public IArtifactToolRegistry LoadRegistry(ArtifactToolID artifactToolId)
    {
        if (_artifactToolRegistry.Contains(artifactToolId))
        {
            throw new ArtUserException($"Registry does not contain an artifact with the ID {artifactToolId}");
        }
        return _artifactToolRegistry;
    }

    /// <inheritdoc />
    public IEnumerable<IArtifactToolRegistry> LoadAllRegistries()
    {
        yield return _artifactToolRegistry;
    }
}
