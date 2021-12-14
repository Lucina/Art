namespace Art;

/// <summary>
/// Provides artifact information.
/// </summary>
/// <param name="ArtifactTool">Artifact tool.</param>
/// <param name="Uri">URI.</param>
/// <param name="Origin">Request origin.</param>
/// <param name="Referrer">Request referrer.</param>
/// <param name="Key">Resource key.</param>
/// <param name="Updated">Updated date.</param>
/// <param name="Version">Version.</param>
public record UriArtifactResourceInfo(HttpArtifactTool ArtifactTool, Uri Uri, string? Origin, string? Referrer, ArtifactResourceKey Key, DateTimeOffset? Updated = null, string? Version = null)
    : QueryBaseArtifactResourceInfo(Key, Updated, Version)
{
    /// <inheritdoc/>
    public override bool Exportable => true;

    /// <inheritdoc/>
    public override async ValueTask<Stream> ExportStreamAsync(CancellationToken cancellationToken = default)
    {
        Stream stream = new MemoryStream();
        await ArtifactTool.DownloadResourceAsync(Uri, stream, Origin, Referrer, cancellationToken).ConfigureAwait(false);
        stream.Position = 0;
        return stream;
    }

    /// <inheritdoc/>
    public override bool MetadataQueryable => true;

    /// <inheritdoc/>
    public override async ValueTask<ArtifactResourceInfo> WithMetadataAsync(CancellationToken cancellationToken = default)
    {
        HttpResponseMessage rsp = await ArtifactTool.HeadAsync(Uri, Origin, Referrer, cancellationToken).ConfigureAwait(false);
        rsp.EnsureSuccessStatusCode();
        return WithMetadata(rsp);
    }
}
