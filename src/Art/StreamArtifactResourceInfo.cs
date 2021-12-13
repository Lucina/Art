namespace Art;

/// <summary>
/// Provides artifact information.
/// </summary>
/// <param name="Resource">Resource.</param>
/// <param name="Key">Resource key.</param>
/// <param name="Version">Version.</param>
public record StreamArtifactResourceInfo(Stream Resource, ArtifactResourceKey Key, string? Version = null)
    : ArtifactResourceInfo(Key, Version)
{
    /// <inheritdoc/>
    public override bool Exportable => true;

    /// <inheritdoc/>
    public override async ValueTask ExportAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        await Resource.CopyToAsync(stream, cancellationToken).ConfigureAwait(false);
    }
}
