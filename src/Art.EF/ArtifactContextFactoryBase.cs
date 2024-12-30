using Microsoft.EntityFrameworkCore.Design;

namespace Art.EF;

/// <summary>
/// Context factory for artifact registration.
/// </summary>
public abstract class ArtifactContextFactoryBase<TContext> : IDesignTimeDbContextFactory<TContext> where TContext : ArtifactContext
{
    /// <inheritdoc />
    public abstract TContext CreateDbContext(string[] args);
}
