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
public record UriStringArtifactResourceInfo(HttpArtifactTool ArtifactTool, string Uri, string? Origin, string? Referrer, ArtifactResourceKey Key, string? Version = null)
    : ArtifactResourceInfo(Key, Version)
{
    /// <inheritdoc/>
    public override bool Exportable => true;

    /// <inheritdoc/>
    public override async ValueTask ExportAsync(Stream stream, CancellationToken cancellationToken = default)
        => await ArtifactTool.DownloadResourceAsync(Uri, stream, Origin, Referrer, cancellationToken);

    /// <inheritdoc/>
    public override bool VersionQueryable => true;

    /// <inheritdoc/>
    public override async ValueTask<string?> QueryVersionAsync(CancellationToken cancellationToken = default)
    {
        HttpResponseMessage rsp = await ArtifactTool.HeadAsync(Uri, Origin, Referrer, cancellationToken).ConfigureAwait(false);
        rsp.EnsureSuccessStatusCode();
        return rsp.Headers.ETag?.Tag;
    }
}
