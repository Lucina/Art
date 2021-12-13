namespace Art;

/// <summary>
/// Provides artifact information.
/// </summary>
/// <param name="Key">Artifact key.</param>
/// <param name="Date">Artifact creation date.</param>
/// <param name="UpdateDate">Artifact update date.</param>
/// <param name="Full">True if this is a full artifact.</param>
public record ArtifactInfo(ArtifactKey Key, DateTimeOffset? Date = null, DateTimeOffset? UpdateDate = null, bool Full = true);
