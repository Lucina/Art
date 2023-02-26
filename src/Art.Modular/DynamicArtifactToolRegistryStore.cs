using System.Diagnostics.CodeAnalysis;

namespace Art.Modular;

[RequiresUnreferencedCode("Loading artifact tools might require types that cannot be statically analyzed.")]
public class DynamicArtifactToolRegistryStore : IArtifactToolRegistryStore
{
    private readonly ModuleManifestProvider _moduleManifestProvider;

    public DynamicArtifactToolRegistryStore(ModuleManifestProvider moduleManifestProvider)
    {
        _moduleManifestProvider = moduleManifestProvider;
    }

    public IArtifactToolRegistry LoadRegistry(ArtifactToolID artifactToolId)
    {
        string assembly = artifactToolId.Assembly;
        if (!_moduleManifestProvider.TryFind(assembly, out var manifest))
        {
            throw new ManifestNotFoundException(assembly);
        }
        return _moduleManifestProvider.LoadForManifest(manifest);
    }

    public IEnumerable<IArtifactToolRegistry> LoadAllRegistries()
    {
        var manifests = new Dictionary<string, ModuleManifest>();
        _moduleManifestProvider.LoadManifests(manifests);
        foreach (ModuleManifest manifest in manifests.Values)
        {
            yield return _moduleManifestProvider.LoadForManifest(manifest);
        }
    }
}
