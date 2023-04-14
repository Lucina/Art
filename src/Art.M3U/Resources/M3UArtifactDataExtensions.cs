using Art.Common;

namespace Art.M3U.Resources;

/// <summary>
/// Extensions for <see cref="ArtifactData"/>.
/// </summary>
public static partial class M3UArtifactDataExtensions
{
    /// <summary>
    /// Creates an <see cref="M3UDownloaderContextSaverArtifactResourceInfo"/> resource.
    /// </summary>
    /// <param name="artifactData">Source <see cref="ArtifactData"/> instance.</param>
    /// <param name="saver">Stream context.</param>
    /// <param name="key">Resource key.</param>
    /// <param name="contentType">MIME content type.</param>
    /// <param name="updated">Updated date.</param>
    /// <param name="version">Version.</param>
    /// <param name="checksum">Checksum.</param>
    public static ArtifactDataResource HLSStream(this ArtifactData artifactData,
        M3UDownloaderContextStreamOutputSaver saver,
        ArtifactResourceKey key,
        string? contentType = "application/octet-stream",
        DateTimeOffset? updated = null,
        string? version = null,
        Checksum? checksum = null)
        => new(artifactData, new M3UDownloaderContextSaverArtifactResourceInfo(saver, key, contentType, updated, version, checksum));
}
