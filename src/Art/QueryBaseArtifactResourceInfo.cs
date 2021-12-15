namespace Art;

/// <summary>
/// Provides artifact information.
/// </summary>
/// <param name="Key">Resource key.</param>
/// <param name="ContentType">MIME content type.</param>
/// <param name="Updated">Updated date.</param>
/// <param name="Version">Version.</param>
public record QueryBaseArtifactResourceInfo(ArtifactResourceKey Key, string? ContentType = "application/octet-stream", DateTimeOffset? Updated = null, string? Version = null)
    : ArtifactResourceInfo(Key, ContentType, Updated, Version)
{
    /// <summary>
    /// Gets this instance modified with metadata from specified response.
    /// </summary>
    /// <param name="response">Response.</param>
    /// <returns>Instance.</returns>
    protected ArtifactResourceInfo WithMetadata(HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();
        string? contentType = response.Content.Headers.ContentType?.MediaType;
        DateTimeOffset? updated = response.Content.Headers.LastModified;
        string? version = response.Headers.ETag?.Tag;
        return this with { ContentType = contentType ?? ContentType, Updated = updated ?? Updated, Version = version ?? Version };
    }
}
