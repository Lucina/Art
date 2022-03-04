namespace Art;

/// <summary>
/// Model type for <see cref="ArtifactInfo"/>.
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
    public virtual string? Name { get; set; }

    /// <summary>
    /// Artifact creation date.
    /// </summary>
    public virtual DateTimeOffset? Date { get; set; }

    /// <summary>
    /// Artifact update date.
    /// </summary>
    public virtual DateTimeOffset? UpdateDate { get; set; }

    /// <summary>
    /// Artifact retrieval date.
    /// </summary>
    public virtual DateTimeOffset? RetrievalDate { get; set; }
    // TODO core support on AI for creation, other logics

    /// <summary>
    /// True if this is a full artifact.
    /// </summary>
    public virtual bool Full { get; set; }

    /// <summary>
    /// Resources.
    /// </summary>
    public virtual HashSet<ArtifactResourceInfoModel> Resources { get; set; } = null!;

    /// <summary>
    /// Converts model to info record.
    /// </summary>
    /// <param name="value">Model.</param>
    /// <returns>Record.</returns>
    public static implicit operator ArtifactInfo(ArtifactInfoModel value)
        => new(new ArtifactKey(value.Tool, value.Group, value.Id), value.Name, value.Date, value.UpdateDate, value.Full);
}
