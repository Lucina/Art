using Microsoft.EntityFrameworkCore;

namespace Art.EF;

/// <summary>
/// EF model type for <see cref="ArtifactInfo"/>.
/// </summary>
public class ArtifactInfoModel
{
    /// <summary>
    /// Tool id.
    /// </summary>
    public virtual string Tool { get; set; } = null!;

    /// <summary>
    /// Group.
    /// </summary>
    public virtual string Group { get; set; } = null!;

    /// <summary>
    /// Artifact id.
    /// </summary>
    public virtual string Id { get; set; } = null!;

    /// <summary>
    /// Name.
    /// </summary>
    public virtual string? Name { get; set; } = null!;

    /// <summary>
    /// Artifact creation date.
    /// </summary>
    public virtual DateTimeOffset? Date { get; set; }

    /// <summary>
    /// Artifact update date.
    /// </summary>
    public virtual DateTimeOffset? UpdateDate { get; set; }

    /// <summary>
    /// True if this is a full artifact.
    /// </summary>
    public virtual bool Full { get; set; }

    /// <summary>
    /// Resources.
    /// </summary>
    public virtual HashSet<ArtifactResourceInfoModel> Resources { get; set; } = null!;

    /*[ModelBuilderCallback]*/
    internal static void OnModelCreating(ModelBuilder m)
    {
        m.Entity<ArtifactInfoModel>(b =>
        {
            b.HasKey(x => new { x.Tool, x.Group, x.Id });
            b
                .HasMany(x => x.Resources)
                .WithOne(x => x.Artifact)
                .HasForeignKey(x => new { x.ArtifactTool, x.ArtifactGroup, x.ArtifactId })
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
