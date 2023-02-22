namespace Art.Common;

/// <summary>
/// Represents a factory for artifact tools.
/// </summary>
/// <typeparam name="TSelf">Self type.</typeparam>
public interface IArtifactToolSelfFactory<TSelf> : IArtifactToolFactory where TSelf : IArtifactTool, new()
{
    static IArtifactTool IArtifactToolFactory.CreateArtifactTool() => new TSelf();

    static ArtifactToolID IArtifactToolFactory.GetArtifactToolId() => ArtifactToolIdUtil.CreateToolId<TSelf>();
}
