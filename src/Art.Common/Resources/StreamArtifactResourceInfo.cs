namespace Art.Common.Resources;

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

    /// <inheritdoc />
    public override void AugmentOutputStreamOptions(ref OutputStreamOptions options)
    {
        if (Resource.CanSeek) options = options with { PreallocationSize = Resource.Length };
    }
}

public partial class ArtifactDataExtensions
{
    /// <summary>
    /// Creates a <see cref="StreamArtifactResourceInfo"/> resource.
    /// </summary>
    /// <param name="artifactData">Source <see cref="ArtifactData"/> instance.</param>
    /// <param name="resource">Resource.</param>
    /// <param name="key">Resource key.</param>
    /// <param name="contentType">MIME content type.</param>
    /// <param name="updated">Updated date.</param>
    /// <param name="version">Version.</param>
    /// <param name="checksum">Checksum.</param>
    public static ArtifactDataResource Stream(this ArtifactData artifactData, Stream resource, ArtifactResourceKey key, string? contentType = "application/octet-stream", DateTimeOffset? updated = null, string? version = null, Checksum? checksum = null)
        => new(artifactData, new StreamArtifactResourceInfo(resource, key, contentType, updated, version, checksum));

    /// <summary>
    /// Creates a <see cref="StreamArtifactResourceInfo"/> resource.
    /// </summary>
    /// <param name="artifactData">Source <see cref="ArtifactData"/> instance.</param>
    /// <param name="resource">Resource.</param>
    /// <param name="file">Filename.</param>
    /// <param name="path">Path.</param>
    /// <param name="contentType">MIME content type.</param>
    /// <param name="updated">Updated date.</param>
    /// <param name="version">Version.</param>
    /// <param name="checksum">Checksum.</param>
    public static ArtifactDataResource Stream(this ArtifactData artifactData, Stream resource, string file, string path = "", string? contentType = "application/octet-stream", DateTimeOffset? updated = null, string? version = null, Checksum? checksum = null)
        => new(artifactData, new StreamArtifactResourceInfo(resource, new ArtifactResourceKey(artifactData.Info.Key, file, path), contentType, updated, version, checksum));
}
