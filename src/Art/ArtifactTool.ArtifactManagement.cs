namespace Art;

public partial class ArtifactTool
{
    #region Artifact management

    /// <summary>
    /// Creates a new instance of <see cref="ArtifactData"/>.
    /// </summary>
    /// <param name="id">Artifact ID.</param>
    /// <param name="name">Name.</param>
    /// <param name="date">Artifact creation date.</param>
    /// <param name="updateDate">Artifact update date.</param>
    /// <param name="full">True if this is a full artifact.</param>
    public ArtifactData CreateData(string id, string? name = null, DateTimeOffset? date = null, DateTimeOffset? updateDate = null, bool full = true)
        => new(this, Profile.Tool, Profile.Group, id, name, date, updateDate, full);

    /// <summary>
    /// Registers artifact as known.
    /// </summary>
    /// <param name="artifactInfo">Artifact to register.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public async Task AddArtifactAsync(ArtifactInfo artifactInfo, CancellationToken cancellationToken = default)
        => await RegistrationManager.AddArtifactAsync(artifactInfo, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Registers artifact resource as known.
    /// </summary>
    /// <param name="artifactResourceInfo">Artifact resource to register.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public async Task AddResourceAsync(ArtifactResourceInfo artifactResourceInfo, CancellationToken cancellationToken = default)
        => await RegistrationManager.AddResourceAsync(artifactResourceInfo, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Attempts to get info for the artifact with the specified ID.
    /// </summary>
    /// <param name="id">Artifact ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning retrieved artifact, if it exists.</returns>
    public async Task<ArtifactInfo?> TryGetArtifactAsync(string id, CancellationToken cancellationToken = default)
        => await RegistrationManager.TryGetArtifactAsync(new ArtifactKey(Profile.Tool, Profile.Group, id), cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Attempts to get info for the artifact with the specified ID.
    /// </summary>
    /// <param name="key">Artifact key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning retrieved artifact, if it exists.</returns>
    public async Task<ArtifactInfo?> TryGetArtifactAsync(ArtifactKey key, CancellationToken cancellationToken = default)
        => await RegistrationManager.TryGetArtifactAsync(key, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Attempts to get info for the resource with the specified key.
    /// </summary>
    /// <param name="key">Resource key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning retrieved resource, if it exists.</returns>
    public async Task<ArtifactResourceInfo?> TryGetResourceAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default)
        => await RegistrationManager.TryGetResourceAsync(key, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Tests artifact state against existing.
    /// </summary>
    /// <param name="artifactInfo">Artifact to check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning comparison against whatever exists with the same ID.</returns>
    public async ValueTask<ItemStateFlags> CompareArtifactAsync(ArtifactInfo artifactInfo, CancellationToken cancellationToken = default)
    {
        ItemStateFlags state = ItemStateFlags.None;
        if (await TryGetArtifactAsync(artifactInfo.Key, cancellationToken).ConfigureAwait(false) is not { } oldArtifact)
        {
            state |= ItemStateFlags.New;
            if (artifactInfo.Date != null || artifactInfo.UpdateDate != null)
                state |= ItemStateFlags.NewerDate;
        }
        else
        {
            if (artifactInfo.UpdateDate != null && oldArtifact.UpdateDate != null)
            {
                if (artifactInfo.UpdateDate > oldArtifact.UpdateDate)
                    state |= ItemStateFlags.NewerDate;
                else if (artifactInfo.UpdateDate < oldArtifact.UpdateDate)
                    state |= ItemStateFlags.OlderDate;
            }
            else if (artifactInfo.UpdateDate != null && oldArtifact.UpdateDate == null)
                state |= ItemStateFlags.NewerDate;
            else if (artifactInfo.UpdateDate == null && oldArtifact.UpdateDate == null)
            {
                if (artifactInfo.Date != null && oldArtifact.Date != null)
                {
                    if (artifactInfo.Date > oldArtifact.Date)
                        state |= ItemStateFlags.NewerDate;
                    else if (artifactInfo.Date < oldArtifact.Date)
                        state |= ItemStateFlags.OlderDate;
                }
                else if (artifactInfo.Date != null && oldArtifact.Date == null)
                    state |= ItemStateFlags.NewerDate;
            }
            if (artifactInfo.Full && !oldArtifact.Full)
                state |= ItemStateFlags.New;
        }
        return state;
    }

    /// <summary>
    /// Attempts to get resource with populated version (if available) based on provided resource.
    /// </summary>
    /// <param name="resource">Resource to check.</param>
    /// <param name="resourceUpdate">Resource update mode.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning instance for the resource with populated version (if available), and additional state information.</returns>
    public async Task<ArtifactResourceInfoWithState> DetermineUpdatedResourceAsync(ArtifactResourceInfo resource, ResourceUpdateMode resourceUpdate, CancellationToken cancellationToken = default)
    {
        ItemStateFlags state = resourceUpdate switch
        {
            ResourceUpdateMode.ArtifactSoft => ItemStateFlags.None,
            ResourceUpdateMode.ArtifactHard => ItemStateFlags.EnforceNew,
            ResourceUpdateMode.Soft => ItemStateFlags.None,
            ResourceUpdateMode.Hard => ItemStateFlags.EnforceNew,
            _ => throw new ArgumentOutOfRangeException(nameof(resourceUpdate), resourceUpdate, null)
        };
        resource = await resource.WithMetadataAsync(cancellationToken).ConfigureAwait(false);
        if (await TryGetResourceAsync(resource.Key, cancellationToken).ConfigureAwait(false) is not { } prev)
        {
            state |= ItemStateFlags.ChangedMetadata | ItemStateFlags.New;
            if (resource.Updated != null)
                state |= ItemStateFlags.NewerDate;
            if (resource.Version != null)
                state |= ItemStateFlags.ChangedVersion;
            if (resource.Checksum != null && !Checksum.DatawiseEquals(resource.Checksum, null))
                state |= ItemStateFlags.NewChecksum;
        }
        else
        {
            if (resource.Version != prev.Version)
                state |= ItemStateFlags.ChangedVersion;
            if (resource.Updated > prev.Updated)
                state |= ItemStateFlags.NewerDate;
            else if (resource.Updated < prev.Updated)
                state |= ItemStateFlags.OlderDate;
            if (resource.IsMetadataDifferent(prev))
                state |= ItemStateFlags.ChangedMetadata;
            if (resource.Checksum != null && !Checksum.DatawiseEquals(resource.Checksum, prev.Checksum))
                state |= ItemStateFlags.NewChecksum;
        }
        return new ArtifactResourceInfoWithState(resource, state);
    }

    #endregion
}
