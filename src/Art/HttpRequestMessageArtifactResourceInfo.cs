namespace Art;

/// <summary>
/// Provides artifact information.
/// </summary>
/// <param name="ArtifactTool">Artifact tool.</param>
/// <param name="Request">Request.</param>
/// <param name="Key">Resource key.</param>
/// <param name="Updated">Updated date.</param>
/// <param name="Version">Version.</param>
public record HttpRequestMessageArtifactResourceInfo(HttpArtifactTool ArtifactTool, HttpRequestMessage Request, ArtifactResourceKey Key, DateTimeOffset? Updated = null, string? Version = null)
    : ArtifactResourceInfo(Key, Updated, Version)
{
    /// <inheritdoc/>
    public override bool Exportable => true;

    /// <inheritdoc/>
    public override async ValueTask ExportAsync(Stream stream, CancellationToken cancellationToken = default)
        => await ArtifactTool.DownloadResourceAsync(Request, stream, cancellationToken);
}
