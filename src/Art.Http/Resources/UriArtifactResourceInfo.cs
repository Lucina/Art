﻿namespace Art.Http.Resources;

/// <summary>
/// Provides artifact information.
/// </summary>
/// <param name="ArtifactTool">Artifact tool.</param>
/// <param name="Uri">URI.</param>
/// <param name="Key">Resource key.</param>
/// <param name="ContentType">MIME content type.</param>
/// <param name="Updated">Updated date.</param>
/// <param name="Version">Version.</param>
/// <param name="Checksum">Checksum.</param>
/// <param name="HttpRequestConfig">Custom request configuration.</param>
public record UriArtifactResourceInfo(
        HttpArtifactTool ArtifactTool,
        Uri Uri,
        HttpRequestConfig? HttpRequestConfig,
        ArtifactResourceKey Key,
        string? ContentType = "application/octet-stream",
        DateTimeOffset? Updated = null,
        string? Version = null,
        Checksum? Checksum = null)
    : QueryBaseArtifactResourceInfo(Key, ContentType, Updated, Version, Checksum)
{
    /// <inheritdoc/>
    public override bool Exportable => true;

    /// <inheritdoc/>
    /// <exception cref="TaskCanceledException">Thrown with <see cref="TimeoutException"/> <see cref="Exception.InnerException"/> for a timeout.</exception>
    public override async ValueTask ExportStreamAsync(Stream targetStream, CancellationToken cancellationToken = default)
    {
        // M3U behaviour depends on calling this member, or any overload targeting the contained HttpClient. Don't change this.
        await ArtifactTool.DownloadResourceAsync(Uri, targetStream, HttpRequestConfig, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override bool UsesMetadata => true;

    /// <inheritdoc/>
    /// <exception cref="TaskCanceledException">Thrown with <see cref="TimeoutException"/> <see cref="Exception.InnerException"/> for a timeout.</exception>
    public override async ValueTask<ArtifactResourceInfo> WithMetadataAsync(CancellationToken cancellationToken = default)
    {
        HttpResponseMessage res = await ArtifactTool.HeadAsync(Uri, HttpRequestConfig, cancellationToken).ConfigureAwait(false);
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
    /// <param name="httpRequestConfig">Custom request configuration.</param>
    public static ArtifactDataResource Uri(this ArtifactData artifactData,
        HttpArtifactTool artifactTool,
        Uri uri,
        ArtifactResourceKey key,
        string? contentType = "application/octet-stream",
        DateTimeOffset? updated = null,
        string? version = null,
        Checksum? checksum = null,
        HttpRequestConfig? httpRequestConfig = null)
        => new(artifactData, new UriArtifactResourceInfo(artifactTool, uri, httpRequestConfig, key, contentType, updated, version, checksum));

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
    /// <param name="httpRequestConfig">Custom request configuration.</param>
    public static ArtifactDataResource Uri(this ArtifactData artifactData,
        HttpArtifactTool artifactTool,
        Uri uri,
        string file,
        string path = "",
        string? contentType = "application/octet-stream",
        DateTimeOffset? updated = null,
        string? version = null,
        Checksum? checksum = null,
        HttpRequestConfig? httpRequestConfig = null)
        => new(artifactData, new UriArtifactResourceInfo(artifactTool, uri, httpRequestConfig, new ArtifactResourceKey(artifactData.Info.Key, file, path), contentType, updated, version, checksum));

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
    /// <param name="httpRequestConfig">Custom request configuration.</param>
    public static ArtifactDataResource Uri(this ArtifactData artifactData,
        Uri uri,
        ArtifactResourceKey key,
        string? contentType = "application/octet-stream",
        DateTimeOffset? updated = null,
        string? version = null,
        Checksum? checksum = null,
        HttpRequestConfig? httpRequestConfig = null)
        => artifactData.Uri(artifactData.GetArtifactTool<HttpArtifactTool>(), uri, key, contentType, updated, version, checksum, httpRequestConfig);

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
    /// <param name="httpRequestConfig">Custom request configuration.</param>
    public static ArtifactDataResource Uri(this ArtifactData artifactData,
        Uri uri,
        string file,
        string path = "",
        string? contentType = "application/octet-stream",
        DateTimeOffset? updated = null,
        string? version = null,
        Checksum? checksum = null,
        HttpRequestConfig? httpRequestConfig = null)
        => artifactData.Uri(artifactData.GetArtifactTool<HttpArtifactTool>(), uri, file, path, contentType, updated, version, checksum, httpRequestConfig);

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
    /// <param name="httpRequestConfig">Custom request configuration.</param>
    public static ArtifactDataResource Uri(this ArtifactData artifactData,
        HttpArtifactTool artifactTool,
        string uri,
        ArtifactResourceKey key,
        string? contentType = "application/octet-stream",
        DateTimeOffset? updated = null,
        string? version = null,
        Checksum? checksum = null,
        HttpRequestConfig? httpRequestConfig = null)
        => new(artifactData, new UriArtifactResourceInfo(artifactTool, new Uri(uri), httpRequestConfig, key, contentType, updated, version, checksum));

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
    /// <param name="httpRequestConfig">Custom request configuration.</param>
    public static ArtifactDataResource Uri(this ArtifactData artifactData,
        HttpArtifactTool artifactTool,
        string uri,
        string file,
        string path = "",
        string? contentType = "application/octet-stream",
        DateTimeOffset? updated = null,
        string? version = null,
        Checksum? checksum = null,
        HttpRequestConfig? httpRequestConfig = null)
        => new(artifactData, new UriArtifactResourceInfo(artifactTool, new Uri(uri), httpRequestConfig, new ArtifactResourceKey(artifactData.Info.Key, file, path), contentType, updated, version, checksum));

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
    /// <param name="httpRequestConfig">Custom request configuration.</param>
    public static ArtifactDataResource Uri(this ArtifactData artifactData,
        string uri,
        ArtifactResourceKey key,
        string? contentType = "application/octet-stream",
        DateTimeOffset? updated = null,
        string? version = null,
        Checksum? checksum = null,
        HttpRequestConfig? httpRequestConfig = null)
        => artifactData.Uri(artifactData.GetArtifactTool<HttpArtifactTool>(), uri, key, contentType, updated, version, checksum, httpRequestConfig);

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
    /// <param name="httpRequestConfig">Custom request configuration.</param>
    public static ArtifactDataResource Uri(this ArtifactData artifactData,
        string uri,
        string file,
        string path = "",
        string? contentType = "application/octet-stream",
        DateTimeOffset? updated = null,
        string? version = null,
        Checksum? checksum = null,
        HttpRequestConfig? httpRequestConfig = null)
        => artifactData.Uri(artifactData.GetArtifactTool<HttpArtifactTool>(), uri, file, path, contentType, updated, version, checksum, httpRequestConfig);
}
