using Microsoft.EntityFrameworkCore.Design;

namespace Art.EF;

/// <summary>
/// Context factory for artifact registration.
/// </summary>
public abstract class ArtifactContextFactoryBase : IDesignTimeDbContextFactory<ArtifactContext>
{
    /// <inheritdoc />
    public abstract ArtifactContext CreateDbContext(string[] args);
}
