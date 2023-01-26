using Art.Common.Management;

namespace Art.Common;

/// <summary>
/// Provides utilities for <see cref="ArtifactToolConfig"/>.
/// </summary>
public static class ArtifactToolConfigUtil
{
    /// <summary>
    /// Shared default instance.
    /// </summary>
    public static readonly ArtifactToolConfig DefaultInstance = new(NullArtifactRegistrationManager.Instance, NullArtifactDataManager.Instance);
}
