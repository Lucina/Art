using System.Diagnostics.CodeAnalysis;

namespace Art.Common;

/// <summary>
/// Represents a store of a single <see cref="IArtifactToolRegistry"/>.
/// </summary>
public class StaticArtifactToolRegistryStore : IArtifactToolRegistryStore
{
    private readonly IArtifactToolRegistry _artifactToolRegistry;

    /// <summary>
    /// Initializes an instance of <see cref="StaticArtifactToolRegistryStore"/>.
    /// </summary>
    /// <param name="artifactToolRegistry">Registry to use.</param>
    public StaticArtifactToolRegistryStore(IArtifactToolRegistry artifactToolRegistry) => _artifactToolRegistry = artifactToolRegistry;

    /// <inheritdoc />
    public bool TryLoadRegistry(ArtifactToolID artifactToolId, [NotNullWhen(true)] out IArtifactToolRegistry? artifactToolRegistry)
    {
        if (!_artifactToolRegistry.Contains(artifactToolId))
        {
            artifactToolRegistry = null;
            return false;
        }
        artifactToolRegistry = _artifactToolRegistry;
        return true;
    }

    /// <inheritdoc />
    public IEnumerable<IArtifactToolRegistry> LoadAllRegistries()
    {
        yield return _artifactToolRegistry;
    }
}
