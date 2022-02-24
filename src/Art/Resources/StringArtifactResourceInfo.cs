using System.Text;
using Art.Crypto;

namespace Art.Resources;

/// <summary>
/// Provides artifact information.
/// </summary>
/// <param name="Resource">Resource.</param>
/// <param name="Key">Resource key.</param>
/// <param name="ContentType">MIME content type.</param>
/// <param name="Updated">Updated date.</param>
/// <param name="Version">Version.</param>
/// <param name="Checksum">Checksum.</param>
public record StringArtifactResourceInfo(string Resource, ArtifactResourceKey Key, string? ContentType = "text/plain", DateTimeOffset? Updated = null, string? Version = null, Checksum? Checksum = null)
    : ArtifactResourceInfo(Key, ContentType, Updated, Version, Checksum)
{
    /// <inheritdoc/>
    public override bool Exportable => true;

    /// <inheritdoc/>
    public override async ValueTask ExportStreamAsync(Stream targetStream, CancellationToken cancellationToken = default)
    {
        await using StreamWriter sw = new(targetStream, Encoding.UTF8, leaveOpen: true);
        await sw.WriteAsync(Resource).ConfigureAwait(false);
    }
}
