namespace Art;

/// <summary>
/// Provides artifact information.
/// </summary>
/// <param name="Key">Resource key.</param>
/// <param name="ContentType">MIME content type.</param>
/// <param name="Updated">Updated date.</param>
/// <param name="Version">Version.</param>
/// <param name="Checksum">Checksum.</param>
/// <param name="ContentLength">Content length.</param>
public record QueryBaseArtifactResourceInfo(ArtifactResourceKey Key, string? ContentType = "application/octet-stream", DateTimeOffset? Updated = null, string? Version = null, Checksum? Checksum = null, long? ContentLength = null)
    : ArtifactResourceInfo(Key, ContentType, Updated, Version, Checksum)
{
    /// <summary>
    /// Gets this instance modified with metadata from specified response.
    /// </summary>
    /// <param name="response">Response.</param>
    /// <returns>Instance.</returns>
    /// <exception cref="AggregateException">Thrown with <see cref="HttpRequestException"/> and <see cref="ExHttpResponseMessageException"/> on HTTP error.</exception>
    protected ArtifactResourceInfo WithMetadata(HttpResponseMessage response)
    {
        ExHttpResponseMessageException.EnsureSuccessStatusCode(response);
        string? contentType = response.Content.Headers.ContentType?.MediaType;
        DateTimeOffset? updated = response.Content.Headers.LastModified;
        string? version = response.Headers.ETag?.Tag;
        long? contentLength = response.Content.Headers.ContentLength;
        return this with { ContentType = contentType ?? ContentType, Updated = updated ?? Updated, Version = version ?? Version, ContentLength = contentLength ?? ContentLength };
    }

    /// <inheritdoc />
    public override void AugmentOutputStreamOptions(ref OutputStreamOptions options)
    {
        if (ContentLength is { } contentLength) options = options with { PreallocationSize = contentLength };
    }
}
