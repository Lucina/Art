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
    public virtual string Path { get; set; } = null!;

    /// <summary>
    /// Content type.
    /// </summary>
    public virtual string? ContentType { get; set; }

    /// <summary>
    /// Updated date.
    /// </summary>
    public virtual DateTimeOffset? Updated { get; set; }

    /// <summary>
    /// Version.
    /// </summary>
    public virtual string? Version { get; set; }

    /// <summary>
    /// Checksum algorithm ID.
    /// </summary>
    public virtual string? ChecksumId { get; set; }

    /// <summary>
    /// Checksum value.
    /// </summary>
    public virtual byte[]? ChecksumValue { get; set; }

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
            b.HasIndex(x => new { x.ArtifactTool, x.ArtifactGroup, x.ArtifactId });
        });
    }
}
