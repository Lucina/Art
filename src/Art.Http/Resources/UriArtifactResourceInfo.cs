namespace Art.Http.Resources;

/// <summary>
/// Provides artifact information.
/// </summary>
/// <param name="ArtifactTool">Artifact tool.</param>
/// <param name="Uri">URI.</param>
/// <param name="Origin">Request origin.</param>
/// <param name="Referrer">Request referrer.</param>
/// <param name="Key">Resource key.</param>
/// <param name="ContentType">MIME content type.</param>
/// <param name="Updated">Updated date.</param>
/// <param name="Version">Version.</param>
/// <param name="Checksum">Checksum.</param>
public record UriArtifactResourceInfo(HttpArtifactTool ArtifactTool, Uri Uri, string? Origin, string? Referrer, ArtifactResourceKey Key, string? ContentType = "application/octet-stream", DateTimeOffset? Updated = null, string? Version = null, Checksum? Checksum = null)
    : QueryBaseArtifactResourceInfo(Key, ContentType, Updated, Version, Checksum)
{
    /// <inheritdoc/>
    public override bool Exportable => true;

    /// <inheritdoc/>
    public override async ValueTask ExportStreamAsync(Stream targetStream, CancellationToken cancellationToken = default)
    {
        await ArtifactTool.DownloadResourceAsync(Uri, targetStream, Origin, Referrer, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override bool UsesMetadata => true;

    /// <inheritdoc/>
    public override async ValueTask<ArtifactResourceInfo> WithMetadataAsync(CancellationToken cancellationToken = default)
    {
        HttpResponseMessage res = await ArtifactTool.HeadAsync(Uri, Origin, Referrer, cancellationToken).ConfigureAwait(false);
        ArtHttpResponseMessageException.EnsureSuccessStatusCode(res);
        return WithMetadata(res);
    }
}

public partial class HttpArtifactDataExtensions
{
    /// <summary>
    /// Creates a <see cref="UriArtifactResourceInfo"/> resource.
    /// </summary>
    /// <param name="artifactData">Source <see cref="ArtifactData"/> instance.</param>
    /// <param name="artifactTool">Artifact tool.</param>
    /// <param name="uri">URI.</param>
    /// <param name="key">Resource key.</param>
    /// <param name="contentType">MIME content type.</param>
    /// <param name="updated">Updated date.</param>
    /// <param name="version">Version.</param>
    /// <param name="checksum">Checksum.</param>
    /// <param name="origin">Request origin.</param>
    /// <param name="referrer">Request referrer.</param>
    public static ArtifactDataResource Uri(this ArtifactData artifactData, HttpArtifactTool artifactTool, Uri uri, ArtifactResourceKey key, string? contentType = "application/octet-stream", DateTimeOffset? updated = null, string? version = null, Checksum? checksum = null, string? origin = null, string? referrer = null)
        => new(artifactData, new UriArtifactResourceInfo(artifactTool, uri, origin, referrer, key, contentType, updated, version, checksum));

    /// <summary>
    /// Creates a <see cref="UriArtifactResourceInfo"/> resource.
    /// </summary>
    /// <param name="artifactData">Source <see cref="ArtifactData"/> instance.</param>
    /// <param name="artifactTool">Artifact tool.</param>
    /// <param name="uri">URI.</param>
    /// <param name="file">Filename.</param>
    /// <param name="path">Path.</param>
    /// <param name="contentType">MIME content type.</param>
    /// <param name="updated">Updated date.</param>
    /// <param name="version">Version.</param>
    /// <param name="checksum">Checksum.</param>
    /// <param name="origin">Request origin.</param>
    /// <param name="referrer">Request referrer.</param>
    public static ArtifactDataResource Uri(this ArtifactData artifactData, HttpArtifactTool artifactTool, Uri uri, string file, string path = "", string? contentType = "application/octet-stream", DateTimeOffset? updated = null, string? version = null, Checksum? checksum = null, string? origin = null, string? referrer = null)
        => new(artifactData, new UriArtifactResourceInfo(artifactTool, uri, origin, referrer, new ArtifactResourceKey(artifactData.Info.Key, file, path), contentType, updated, version, checksum));

    /// <summary>
    /// Creates a <see cref="UriArtifactResourceInfo"/> resource.
    /// </summary>
    /// <param name="artifactData">Source <see cref="ArtifactData"/> instance.</param>
    /// <param name="uri">URI.</param>
    /// <param name="key">Resource key.</param>
    /// <param name="contentType">MIME content type.</param>
    /// <param name="updated">Updated date.</param>
    /// <param name="version">Version.</param>
    /// <param name="checksum">Checksum.</param>
    /// <param name="origin">Request origin.</param>
    /// <param name="referrer">Request referrer.</param>
    public static ArtifactDataResource Uri(this ArtifactData artifactData, Uri uri, ArtifactResourceKey key, string? contentType = "application/octet-stream", DateTimeOffset? updated = null, string? version = null, Checksum? checksum = null, string? origin = null, string? referrer = null)
        => artifactData.Uri(artifactData.GetArtifactTool<HttpArtifactTool>(), uri, key, contentType, updated, version, checksum, origin, referrer);

    /// <summary>
    /// Creates a <see cref="UriArtifactResourceInfo"/> resource.
    /// </summary>
    /// <param name="artifactData">Source <see cref="ArtifactData"/> instance.</param>
    /// <param name="uri">URI.</param>
    /// <param name="file">Filename.</param>
    /// <param name="path">Path.</param>
    /// <param name="contentType">MIME content type.</param>
    /// <param name="updated">Updated date.</param>
    /// <param name="version">Version.</param>
    /// <param name="checksum">Checksum.</param>
    /// <param name="origin">Request origin.</param>
    /// <param name="referrer">Request referrer.</param>
    public static ArtifactDataResource Uri(this ArtifactData artifactData, Uri uri, string file, string path = "", string? contentType = "application/octet-stream", DateTimeOffset? updated = null, string? version = null, Checksum? checksum = null, string? origin = null, string? referrer = null)
        => artifactData.Uri(artifactData.GetArtifactTool<HttpArtifactTool>(), uri, file, path, contentType, updated, version, checksum, origin, referrer);

    /// <summary>
    /// Creates a <see cref="UriArtifactResourceInfo"/> resource.
    /// </summary>
    /// <param name="artifactData">Source <see cref="ArtifactData"/> instance.</param>
    /// <param name="artifactTool">Artifact tool.</param>
    /// <param name="uri">URI.</param>
    /// <param name="key">Resource key.</param>
    /// <param name="contentType">MIME content type.</param>
    /// <param name="updated">Updated date.</param>
    /// <param name="version">Version.</param>
    /// <param name="checksum">Checksum.</param>
    /// <param name="origin">Request origin.</param>
    /// <param name="referrer">Request referrer.</param>
    public static ArtifactDataResource Uri(this ArtifactData artifactData, HttpArtifactTool artifactTool, string uri, ArtifactResourceKey key, string? contentType = "application/octet-stream", DateTimeOffset? updated = null, string? version = null, Checksum? checksum = null, string? origin = null, string? referrer = null)
        => new(artifactData, new UriArtifactResourceInfo(artifactTool, new Uri(uri), origin, referrer, key, contentType, updated, version, checksum));

    /// <summary>
    /// Creates a <see cref="UriArtifactResourceInfo"/> resource.
    /// </summary>
    /// <param name="artifactData">Source <see cref="ArtifactData"/> instance.</param>
    /// <param name="artifactTool">Artifact tool.</param>
    /// <param name="uri">URI.</param>
    /// <param name="file">Filename.</param>
    /// <param name="path">Path.</param>
    /// <param name="contentType">MIME content type.</param>
    /// <param name="updated">Updated date.</param>
    /// <param name="version">Version.</param>
    /// <param name="checksum">Checksum.</param>
    /// <param name="origin">Request origin.</param>
    /// <param name="referrer">Request referrer.</param>
    public static ArtifactDataResource Uri(this ArtifactData artifactData, HttpArtifactTool artifactTool, string uri, string file, string path = "", string? contentType = "application/octet-stream", DateTimeOffset? updated = null, string? version = null, Checksum? checksum = null, string? origin = null, string? referrer = null)
        => new(artifactData, new UriArtifactResourceInfo(artifactTool, new Uri(uri), origin, referrer, new ArtifactResourceKey(artifactData.Info.Key, file, path), contentType, updated, version, checksum));

    /// <summary>
    /// Creates a <see cref="UriArtifactResourceInfo"/> resource.
    /// </summary>
    /// <param name="artifactData">Source <see cref="ArtifactData"/> instance.</param>
    /// <param name="uri">URI.</param>
    /// <param name="key">Resource key.</param>
    /// <param name="contentType">MIME content type.</param>
    /// <param name="updated">Updated date.</param>
    /// <param name="version">Version.</param>
    /// <param name="checksum">Checksum.</param>
    /// <param name="origin">Request origin.</param>
    /// <param name="referrer">Request referrer.</param>
    public static ArtifactDataResource Uri(this ArtifactData artifactData, string uri, ArtifactResourceKey key, string? contentType = "application/octet-stream", DateTimeOffset? updated = null, string? version = null, Checksum? checksum = null, string? origin = null, string? referrer = null)
        => artifactData.Uri(artifactData.GetArtifactTool<HttpArtifactTool>(), uri, key, contentType, updated, version, checksum, origin, referrer);

    /// <summary>
    /// Creates a <see cref="UriArtifactResourceInfo"/> resource.
    /// </summary>
    /// <param name="artifactData">Source <see cref="ArtifactData"/> instance.</param>
    /// <param name="uri">URI.</param>
    /// <param name="file">Filename.</param>
    /// <param name="path">Path.</param>
    /// <param name="contentType">MIME content type.</param>
    /// <param name="updated">Updated date.</param>
    /// <param name="version">Version.</param>
    /// <param name="checksum">Checksum.</param>
    /// <param name="origin">Request origin.</param>
    /// <param name="referrer">Request referrer.</param>
    public static ArtifactDataResource Uri(this ArtifactData artifactData, string uri, string file, string path = "", string? contentType = "application/octet-stream", DateTimeOffset? updated = null, string? version = null, Checksum? checksum = null, string? origin = null, string? referrer = null)
        => artifactData.Uri(artifactData.GetArtifactTool<HttpArtifactTool>(), uri, file, path, contentType, updated, version, checksum, origin, referrer);
}
