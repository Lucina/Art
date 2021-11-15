﻿using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Art;

/// <summary>
/// Represents a manager for artifact data.
/// </summary>
public abstract class ArtifactRegistrationManager
{
    /// <summary>
    /// Registers artifact as known.
    /// </summary>
    /// <param name="artifactInfo">Artifact to register.</param>
    /// <returns>Task.</returns>
    public abstract ValueTask AddArtifactAsync(ArtifactInfo artifactInfo);

    /// <summary>
    /// Attempts to get info for the artifact with the specified ID.
    /// </summary>
    /// <param name="id">Artifact ID.</param>
    /// <returns>Task returning retrieved artifact, if it exists.</returns>
    public abstract ValueTask<ArtifactInfo?> TryGetArtifactAsync(string id);

    /// <summary>
    /// Tests if artifact is recognizably new.
    /// </summary>
    /// <param name="artifactInfo">Artifact to check.</param>
    /// <returns>True if this is a new artifact (newer than whatever exists with the same ID).</returns>
    public async ValueTask<bool> IsNewArtifactAsync(ArtifactInfo artifactInfo)
    {
        return (await TryGetArtifactAsync(artifactInfo.Id).ConfigureAwait(false)) is not { } oldArtifact
            || oldArtifact.UpdateDate == null
            || artifactInfo.UpdateDate != null && oldArtifact.UpdateDate.Value < artifactInfo.UpdateDate.Value;
    }
}
