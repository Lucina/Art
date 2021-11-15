using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Art;

/// <summary>
/// Represents a manager for artifact data.
/// </summary>
public abstract class ArtifactRegistrationManager
{
    /// <summary>
    /// Attempts to get info for the artifact with the specified ID.
    /// </summary>
    /// <param name="id">Artifact ID.</param>
    /// <returns>Task returning retrieved artifact, if it exists.</returns>
    public abstract Task<ArtifactInfo?> TryGetInfoAsync(string id);

    /// <summary>
    /// Registers artifact as known.
    /// </summary>
    /// <param name="artifactInfo">Artifact to register.</param>
    /// <returns>Task.</returns>
    public abstract Task AddInfoAsync(ArtifactInfo artifactInfo);

    /// <summary>
    /// Tests if artifact is recognizably new.
    /// </summary>
    /// <param name="artifactInfo">Artifact to check.</param>
    /// <returns>True if this is a new artifact (newer than whatever exists with the same ID).</returns>
    public async Task<bool> IsNewArtifactAsync(ArtifactInfo artifactInfo)
    {
        return (await TryGetInfoAsync(artifactInfo.Id).ConfigureAwait(false)) is not { } oldArtifact
            || oldArtifact.UpdateDate == null
            || artifactInfo.UpdateDate != null && oldArtifact.UpdateDate.Value < artifactInfo.UpdateDate.Value;
    }
}
