﻿using System.Text.Json;

namespace Art;

/// <summary>
/// Provides artifact information.
/// </summary>
/// <param name="ArtifactTool">Artifact tool.</param>
/// <param name="Uri">URI.</param>
/// <param name="Origin">Request origin.</param>
/// <param name="Referrer">Request referrer.</param>
/// <param name="Key">Resource key.</param>
/// <param name="Version">Version.</param>
/// <param name="Properties">Resource properties.</param>
public record UriStringArtifactResourceInfo(HttpArtifactTool ArtifactTool, string Uri, string? Origin, string? Referrer, ArtifactResourceKey Key, string? Version, IReadOnlyDictionary<string, JsonElement> Properties)
    : ArtifactResourceInfo(Key, Version, Properties)
{
    /// <summary>
    /// Creates a new instance of <see cref="UriStringArtifactResourceInfo"/>.
    /// </summary>
    /// <param name="artifactTool">Artifact tool.</param>
    /// <param name="uri">URI.</param>
    /// <param name="key">Resource key.</param>
    /// <param name="version">Version.</param>
    /// <param name="properties">Resource properties.</param>
    /// <param name="origin">Request origin.</param>
    /// <param name="referrer">Request referrer.</param>
    /// <returns>Value.</returns>
    public static UriStringArtifactResourceInfo Create(HttpArtifactTool artifactTool, string uri, ArtifactResourceKey key, string? version = null, IReadOnlyDictionary<string, JsonElement>? properties = null, string? origin = null, string? referrer = null)
        => new(artifactTool, uri, origin, referrer, key, version, properties ?? EmptyProperties);

    /// <inheritdoc/>
    public override bool Exportable => true;

    /// <inheritdoc/>
    public override ValueTask ExportAsync(Stream stream)
        => ArtifactTool.DownloadResourceAsync(Uri, stream, Origin, Referrer);

    /// <inheritdoc/>
    public override bool Queryable => true;

    /// <inheritdoc/>
    public override async ValueTask<string?> QueryVersionAsync()
    {
        HttpResponseMessage rsp = await ArtifactTool.HeadAsync(Uri, Origin, Referrer).ConfigureAwait(false);
        rsp.EnsureSuccessStatusCode();
        return rsp.Headers.ETag?.Tag;
    }
}
