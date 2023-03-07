using Art.Common;

namespace Art.Tesler;

public class ToolControlContext
{
    protected readonly IArtifactToolRegistryStore PluginStore;

    public ToolControlContext(IArtifactToolRegistryStore pluginStore)
    {
        PluginStore = pluginStore;
    }

    protected IArtifactTool LoadTool(ArtifactToolProfile artifactToolProfile)
    {
        var context = PluginStore.LoadRegistry(ArtifactToolProfileUtil.GetID(artifactToolProfile.Tool)); // InvalidOperationException
        if (!context.TryLoad(artifactToolProfile.GetID(), out IArtifactTool? t))
        {
            throw new ArtifactToolNotFoundException(artifactToolProfile.Tool);
        }
        return t;
    }
}
