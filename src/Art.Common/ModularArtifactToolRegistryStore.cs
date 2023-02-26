using System.Diagnostics.CodeAnalysis;

namespace Art.Common;

/// <summary>
/// Represents a store of dynamically loaded <see cref="IArtifactToolRegistry"/> based on a <see cref="IModuleProvider"/>.
/// </summary>
[RequiresUnreferencedCode("Loading artifact tools might require types that cannot be statically analyzed.")]
public class ModularArtifactToolRegistryStore : IArtifactToolRegistryStore
{
    private readonly IModuleProvider _moduleProvider;

    /// <summary>
    /// Initializes an instance of <see cref="ModularArtifactToolRegistryStore"/>.
    /// </summary>
    /// <param name="moduleProvider"><see cref="IModuleProvider"/>.</param>
    public ModularArtifactToolRegistryStore(IModuleProvider moduleProvider)
    {
        _moduleProvider = moduleProvider;
    }

    /// <inheritdoc />
    public IArtifactToolRegistry LoadRegistry(ArtifactToolID artifactToolId)
    {
        string assembly = artifactToolId.Assembly;
        if (!_moduleProvider.TryLocateModule(assembly, out var module))
        {
            throw new ArtUserException($"No applicable manifest for the assembly {assembly} could be found.");
        }
        return _moduleProvider.LoadModule(module);
    }

    /// <inheritdoc />
    public IEnumerable<IArtifactToolRegistry> LoadAllRegistries()
    {
        var modules = new Dictionary<string, IModuleLocation>();
        _moduleProvider.LoadModuleLocations(modules);
        foreach (var module in modules.Values)
        {
            yield return _moduleProvider.LoadModule(module);
        }
    }
}
