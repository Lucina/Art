namespace Art.Common;

/// <summary>
/// Represents an entry for a <see cref="IArtifactToolRegistry"/>.
/// </summary>
/// <param name="Id">Tool name.</param>
public abstract record ArtifactToolRegistryEntry(ArtifactToolID Id)
{
    /// <summary>
    /// Creates an artifact tool.
    /// </summary>
    /// <returns>Artifact tool.</returns>
    public abstract IArtifactTool CreateArtifactTool();
}

/// <summary>
/// Represents an entry for a <see cref="IArtifactToolRegistry"/>.
/// </summary>
/// <param name="Id">Tool name.</param>
/// <typeparam name="T">Tool type.</typeparam>
public record ArtifactToolRegistryEntry<T>(ArtifactToolID Id) : ArtifactToolRegistryEntry(Id) where T : IArtifactToolFactory
{
    /// <inheritdoc />
    public override IArtifactTool CreateArtifactTool()
    {
        return T.CreateArtifactTool();
    }
}
