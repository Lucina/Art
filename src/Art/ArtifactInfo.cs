namespace Art;

/// <summary>
/// Provides artifact information.
/// </summary>
/// <param name="Key">Artifact key.</param>
/// <param name="Name">Name.</param>
/// <param name="Date">Artifact creation date.</param>
/// <param name="UpdateDate">Artifact update date.</param>
/// <param name="Full">True if this is a full artifact.</param>
public record ArtifactInfo(ArtifactKey Key, string? Name = null, DateTimeOffset? Date = null, DateTimeOffset? UpdateDate = null, bool Full = true)
{
    /// <summary>
    /// Gets informational title string.
    /// </summary>
    /// <returns>Info title string.</returns>
    public string GetInfoTitleString() => $"{(!Full ? "[partial] " : "")}{Key.Id}{(Name != null ? $" - {Name}" : "")}";

    /// <summary>
    /// Gets informational string.
    /// </summary>
    /// <returns>Info string.</returns>
    public string GetInfoString() => $"ID: {Key.Id}{(Name != null ? $"\nName: {Name}" : "")}{(Date != null ? $"\nDate: {Date}" : "")}{(UpdateDate != null ? $"\nUpdate Date: {UpdateDate}" : "")}\nFull: {Full}";
}
