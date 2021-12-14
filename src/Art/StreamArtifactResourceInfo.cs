namespace Art;

/// <summary>
/// Provides artifact information.
/// </summary>
/// <param name="Resource">Resource.</param>
/// <param name="Key">Resource key.</param>
/// <param name="Updated">Updated date.</param>
/// <param name="Version">Version.</param>
public record StreamArtifactResourceInfo(Stream Resource, ArtifactResourceKey Key, DateTimeOffset? Updated = null, string? Version = null)
    : ArtifactResourceInfo(Key, Updated, Version)
{
    /// <inheritdoc/>
    public override bool Exportable => true;

    /// <inheritdoc/>
    public override async ValueTask ExportAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        await Resource.CopyToAsync(stream, cancellationToken).ConfigureAwait(false);
    }
}
