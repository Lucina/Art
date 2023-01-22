namespace Art.Http.Resources;

/// <summary>
/// Provides artifact information.
/// </summary>
/// <param name="ArtifactTool">Artifact tool.</param>
/// <param name="Request">Request.</param>
/// <param name="Key">Resource key.</param>
/// <param name="ContentType">MIME content type.</param>
/// <param name="Updated">Updated date.</param>
/// <param name="Version">Version.</param>
/// <param name="Checksum">Checksum.</param>
public record HttpRequestMessageArtifactResourceInfo(HttpArtifactTool ArtifactTool, HttpRequestMessage Request, ArtifactResourceKey Key, string? ContentType = "application/octet-stream", DateTimeOffset? Updated = null, string? Version = null, Checksum? Checksum = null)
    : QueryBaseArtifactResourceInfo(Key, ContentType, Updated, Version, Checksum)
{
    /// <inheritdoc/>
    public override bool Exportable => true;

    /// <inheritdoc/>
    public override async ValueTask ExportStreamAsync(Stream targetStream, CancellationToken cancellationToken = default)
    {
        await ArtifactTool.DownloadResourceAsync(Request, targetStream, cancellationToken).ConfigureAwait(false);
    }
}

public partial class HttpArtifactDataExtensions
{
    /// <summary>
    /// Creates a <see cref="HttpRequestMessageArtifactResourceInfo"/> resource.
    /// </summary>
    /// <param name="artifactData">Source <see cref="ArtifactData"/> instance.</param>
    /// <param name="artifactTool">Artifact tool.</param>
    /// <param name="request">Request.</param>
    /// <param name="key">Resource key.</param>
    /// <param name="contentType">MIME content type.</param>
    /// <param name="updated">Updated date.</param>
    /// <param name="version">Version.</param>
    /// <param name="checksum">Checksum.</param>
    public static ArtifactDataResource HttpRequestMessage(this ArtifactData artifactData, HttpArtifactTool artifactTool, HttpRequestMessage request, ArtifactResourceKey key, string? contentType = "application/octet-stream", DateTimeOffset? updated = null, string? version = null, Checksum? checksum = null)
        => new(artifactData, new HttpRequestMessageArtifactResourceInfo(artifactTool, request, key, contentType, updated, version, checksum));

    /// <summary>
    /// Creates a <see cref="HttpRequestMessageArtifactResourceInfo"/> resource.
    /// </summary>
    /// <param name="artifactData">Source <see cref="ArtifactData"/> instance.</param>
    /// <param name="artifactTool">Artifact tool.</param>
    /// <param name="request">Request.</param>
    /// <param name="file">Filename.</param>
    /// <param name="path">Path.</param>
    /// <param name="contentType">MIME content type.</param>
    /// <param name="updated">Updated date.</param>
    /// <param name="version">Version.</param>
    /// <param name="checksum">Checksum.</param>
    public static ArtifactDataResource HttpRequestMessage(this ArtifactData artifactData, HttpArtifactTool artifactTool, HttpRequestMessage request, string file, string path = "", string? contentType = "application/octet-stream", DateTimeOffset? updated = null, string? version = null, Checksum? checksum = null)
        => new(artifactData, new HttpRequestMessageArtifactResourceInfo(artifactTool, request, new ArtifactResourceKey(artifactData.Info.Key, file, path), contentType, updated, version, checksum));

    /// <summary>
    /// Creates a <see cref="HttpRequestMessageArtifactResourceInfo"/> resource.
    /// </summary>
    /// <param name="artifactData">Source <see cref="ArtifactData"/> instance.</param>
    /// <param name="request">Request.</param>
    /// <param name="key">Resource key.</param>
    /// <param name="contentType">MIME content type.</param>
    /// <param name="updated">Updated date.</param>
    /// <param name="version">Version.</param>
    /// <param name="checksum">Checksum.</param>
    public static ArtifactDataResource HttpRequestMessage(this ArtifactData artifactData, HttpRequestMessage request, ArtifactResourceKey key, string? contentType = "application/octet-stream", DateTimeOffset? updated = null, string? version = null, Checksum? checksum = null)
        => artifactData.HttpRequestMessage(artifactData.GetArtifactTool<HttpArtifactTool>(), request, key, contentType, updated, version, checksum);

    /// <summary>
    /// Creates a <see cref="HttpRequestMessageArtifactResourceInfo"/> resource.
    /// </summary>
    /// <param name="artifactData">Source <see cref="ArtifactData"/> instance.</param>
    /// <param name="request">Request.</param>
    /// <param name="file">Filename.</param>
    /// <param name="path">Path.</param>
    /// <param name="contentType">MIME content type.</param>
    /// <param name="updated">Updated date.</param>
    /// <param name="version">Version.</param>
    /// <param name="checksum">Checksum.</param>
    public static ArtifactDataResource HttpRequestMessage(this ArtifactData artifactData, HttpRequestMessage request, string file, string path = "", string? contentType = "application/octet-stream", DateTimeOffset? updated = null, string? version = null, Checksum? checksum = null)
        => artifactData.HttpRequestMessage(artifactData.GetArtifactTool<HttpArtifactTool>(), request, file, path, contentType, updated, version, checksum);
}
