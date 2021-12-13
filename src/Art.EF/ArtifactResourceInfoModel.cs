using Microsoft.EntityFrameworkCore;

namespace Art.EF;

/// <summary>
/// EF model type for <see cref="ArtifactResourceInfo"/>.
/// </summary>
public class ArtifactResourceInfoModel
{
    /// <summary>
    /// Tool id.
    /// </summary>
    public virtual string ArtifactTool { get; set; } = null!;

    /// <summary>
    /// Group.
    /// </summary>
    public virtual string ArtifactGroup { get; set; } = null!;

    /// <summary>
    /// Artifact id.
    /// </summary>
    public virtual string ArtifactId { get; set; } = null!;

    /// <summary>
    /// Artifact.
    /// </summary>
    public virtual ArtifactInfoModel Artifact { get; set; } = null!;

    /// <summary>
    /// Filename.
    /// </summary>
    public virtual string File { get; set; } = null!;

    /// <summary>
    /// Path.
    /// </summary>
    public virtual string Path { get; set; }

    /// <summary>
    /// Version.
    /// </summary>
    public virtual string? Version { get; set; }

    /*[ModelBuilderCallback]*/
    internal static void OnModelCreating(ModelBuilder m)
    {
        m.Entity<ArtifactResourceInfoModel>(b =>
        {
            b.HasKey(x => new
            {
                x.ArtifactTool,
                x.ArtifactGroup,
                x.ArtifactId,
                x.File,
                x.Path
            });
        });
    }
}
