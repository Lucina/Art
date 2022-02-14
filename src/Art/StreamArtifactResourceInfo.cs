namespace Art;

/// <summary>
/// Provides artifact information.
/// </summary>
/// <param name="Resource">Resource.</param>
/// <param name="Key">Resource key.</param>
/// <param name="ContentType">MIME content type.</param>
/// <param name="Updated">Updated date.</param>
/// <param name="Version">Version.</param>
/// <param name="Checksum">Checksum.</param>
public record StreamArtifactResourceInfo(Stream Resource, ArtifactResourceKey Key, string? ContentType = "application/octet-stream", DateTimeOffset? Updated = null, string? Version = null, Checksum? Checksum = null)
    : ArtifactResourceInfo(Key, ContentType, Updated, Version, Checksum)
{
    /// <inheritdoc/>
    public override bool Exportable => true;

    /// <inheritdoc/>
    public override async ValueTask ExportStreamAsync(Stream targetStream, CancellationToken cancellationToken = default)
    {
        long? position = Resource.CanSeek ? Resource.Position : null;
        try
        {
            await Resource.CopyToAsync(targetStream, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            if (position is { } p) Resource.Position = p;
        }
    }
}
