namespace Art;

/// <summary>
/// Represents a key for an artifact resource.
/// </summary>
/// <param name="Artifact">Artifact key.</param>
/// <param name="File">Filename.</param>
/// <param name="Path">Path.</param>
/// <param name="InArtifactFolder">If false, sent to common directory.</param>
public record ArtifactResourceKey(ArtifactKey Artifact, string File, string? Path, bool InArtifactFolder)
{
    /// <summary>
    /// Creates a new instance of <see cref="ArtifactResourceKey"/>.
    /// </summary>
    /// <param name="key">Artifact key.</param>
    /// <param name="file">Filename.</param>
    /// <param name="path">Path.</param>
    /// <param name="inArtifactFolder">If false, sent to common directory.</param>
    /// <returns>Value.</returns>
    public static ArtifactResourceKey Create(ArtifactKey key, string file, string? path = null, bool inArtifactFolder = true)
        => new(key, file, path, inArtifactFolder);

    /// <summary>
    /// Creates a new instance of <see cref="ArtifactResourceKey"/>.
    /// </summary>
    /// <param name="tool">Tool id.</param>
    /// <param name="group">Group.</param>
    /// <param name="id">Artifact ID.</param>
    /// <param name="file">Filename.</param>
    /// <param name="path">Path.</param>
    /// <param name="inArtifactFolder">If false, sent to common directory.</param>
    /// <returns>Value.</returns>
    public static ArtifactResourceKey Create(string tool, string group, string id, string file, string? path = null, bool inArtifactFolder = true)
        => new(new ArtifactKey(tool, group, id), file, path, inArtifactFolder);

}
