using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Art.Common.Resources;

/// <summary>
/// Provides artifact information.
/// </summary>
/// <param name="Resource">Resource.</param>
/// <param name="SerializerOptions">Serializer options.</param>
/// <param name="Key">Resource key.</param>
/// <param name="ContentType">MIME content type.</param>
/// <param name="Updated">Updated date.</param>
/// <param name="Version">Version.</param>
/// <param name="Checksum">Checksum.</param>
[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
public record JsonArtifactResourceInfo<T>(T Resource, JsonSerializerOptions? SerializerOptions, ArtifactResourceKey Key, string? ContentType = "application/json", DateTimeOffset? Updated = null, string? Version = null, Checksum? Checksum = null)
    : ArtifactResourceInfo(Key, ContentType, Updated, Version, Checksum)
{
    /// <inheritdoc/>
    public override bool Exportable => true;

    /// <inheritdoc/>
    public override async ValueTask ExportStreamAsync(Stream targetStream, CancellationToken cancellationToken = default)
    {
        await JsonSerializer.SerializeAsync(targetStream, Resource, SerializerOptions, cancellationToken).ConfigureAwait(false);
    }
}

/// <summary>
/// Provides artifact information.
/// </summary>
/// <param name="Resource">Resource.</param>
/// <param name="JsonTypeInfo">JSON type info.</param>
/// <param name="Key">Resource key.</param>
/// <param name="ContentType">MIME content type.</param>
/// <param name="Updated">Updated date.</param>
/// <param name="Version">Version.</param>
/// <param name="Checksum">Checksum.</param>
public record JsonWithJsonTypeInfoArtifactResourceInfo<T>(T Resource, JsonTypeInfo<T> JsonTypeInfo, ArtifactResourceKey Key, string? ContentType = "application/json", DateTimeOffset? Updated = null, string? Version = null, Checksum? Checksum = null)
    : ArtifactResourceInfo(Key, ContentType, Updated, Version, Checksum)
{
    /// <inheritdoc/>
    public override bool Exportable => true;

    /// <inheritdoc/>
    public override async ValueTask ExportStreamAsync(Stream targetStream, CancellationToken cancellationToken = default)
    {
        await JsonSerializer.SerializeAsync(targetStream, Resource, JsonTypeInfo, cancellationToken).ConfigureAwait(false);
    }
}

public partial class ArtifactDataExtensions
{
    /// <summary>
    /// Creates a <see cref="JsonArtifactResourceInfo{T}"/> resource.
    /// </summary>
    /// <param name="artifactData">Source <see cref="ArtifactData"/> instance.</param>
    /// <param name="resource">Resource.</param>
    /// <param name="serializerOptions">Serializer options.</param>
    /// <param name="key">Resource key.</param>
    /// <param name="contentType">MIME content type.</param>
    /// <param name="updated">Updated date.</param>
    /// <param name="version">Version.</param>
    /// <param name="checksum">Checksum.</param>
    [RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
    public static ArtifactDataResource Json<T>(this ArtifactData artifactData, T resource, JsonSerializerOptions? serializerOptions, ArtifactResourceKey key, string? contentType = "application/json", DateTimeOffset? updated = null, string? version = null, Checksum? checksum = null)
        => new(artifactData, new JsonArtifactResourceInfo<T>(resource, serializerOptions, key, contentType, updated, version, checksum));

    /// <summary>
    /// Creates a <see cref="JsonArtifactResourceInfo{T}"/> resource.
    /// </summary>
    /// <param name="artifactData">Source <see cref="ArtifactData"/> instance.</param>
    /// <param name="resource">Resource.</param>
    /// <param name="jsonTypeInfo">JSON type info.</param>
    /// <param name="key">Resource key.</param>
    /// <param name="contentType">MIME content type.</param>
    /// <param name="updated">Updated date.</param>
    /// <param name="version">Version.</param>
    /// <param name="checksum">Checksum.</param>
    public static ArtifactDataResource Json<T>(this ArtifactData artifactData, T resource, JsonTypeInfo<T> jsonTypeInfo, ArtifactResourceKey key, string? contentType = "application/json", DateTimeOffset? updated = null, string? version = null, Checksum? checksum = null)
        => new(artifactData, new JsonWithJsonTypeInfoArtifactResourceInfo<T>(resource, jsonTypeInfo, key, contentType, updated, version, checksum));

    /// <summary>
    /// Creates a <see cref="JsonArtifactResourceInfo{T}"/> resource.
    /// </summary>
    /// <param name="artifactData">Source <see cref="ArtifactData"/> instance.</param>
    /// <param name="resource">Resource.</param>
    /// <param name="serializerOptions">Serializer options.</param>
    /// <param name="file">Filename.</param>
    /// <param name="path">Path.</param>
    /// <param name="contentType">MIME content type.</param>
    /// <param name="updated">Updated date.</param>
    /// <param name="version">Version.</param>
    /// <param name="checksum">Checksum.</param>
    [RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
    public static ArtifactDataResource Json<T>(this ArtifactData artifactData, T resource, JsonSerializerOptions? serializerOptions, string file, string path = "", string? contentType = "application/json", DateTimeOffset? updated = null, string? version = null, Checksum? checksum = null)
        => new(artifactData, new JsonArtifactResourceInfo<T>(resource, serializerOptions, new ArtifactResourceKey(artifactData.Info.Key, file, path), contentType, updated, version, checksum));

    /// <summary>
    /// Creates a <see cref="JsonArtifactResourceInfo{T}"/> resource.
    /// </summary>
    /// <param name="artifactData">Source <see cref="ArtifactData"/> instance.</param>
    /// <param name="resource">Resource.</param>
    /// <param name="jsonTypeInfo">JSON type info.</param>
    /// <param name="file">Filename.</param>
    /// <param name="path">Path.</param>
    /// <param name="contentType">MIME content type.</param>
    /// <param name="updated">Updated date.</param>
    /// <param name="version">Version.</param>
    /// <param name="checksum">Checksum.</param>
    public static ArtifactDataResource Json<T>(this ArtifactData artifactData, T resource, JsonTypeInfo<T> jsonTypeInfo, string file, string path = "", string? contentType = "application/json", DateTimeOffset? updated = null, string? version = null, Checksum? checksum = null)
        => new(artifactData, new JsonWithJsonTypeInfoArtifactResourceInfo<T>(resource, jsonTypeInfo, new ArtifactResourceKey(artifactData.Info.Key, file, path), contentType, updated, version, checksum));

    /// <summary>
    /// Creates a <see cref="JsonArtifactResourceInfo{T}"/> resource.
    /// </summary>
    /// <param name="artifactData">Source <see cref="ArtifactData"/> instance.</param>
    /// <param name="resource">Resource.</param>
    /// <param name="key">Resource key.</param>
    /// <param name="contentType">MIME content type.</param>
    /// <param name="updated">Updated date.</param>
    /// <param name="version">Version.</param>
    /// <param name="checksum">Checksum.</param>
    [RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
    public static ArtifactDataResource Json<T>(this ArtifactData artifactData, T resource, ArtifactResourceKey key, string? contentType = "application/json", DateTimeOffset? updated = null, string? version = null, Checksum? checksum = null)
        => new(artifactData, new JsonArtifactResourceInfo<T>(resource, artifactData.Tool?.JsonOptions, key, contentType, updated, version, checksum));

    /// <summary>
    /// Creates a <see cref="JsonArtifactResourceInfo{T}"/> resource.
    /// </summary>
    /// <param name="artifactData">Source <see cref="ArtifactData"/> instance.</param>
    /// <param name="resource">Resource.</param>
    /// <param name="file">Filename.</param>
    /// <param name="path">Path.</param>
    /// <param name="contentType">MIME content type.</param>
    /// <param name="updated">Updated date.</param>
    /// <param name="version">Version.</param>
    /// <param name="checksum">Checksum.</param>
    [RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
    public static ArtifactDataResource Json<T>(this ArtifactData artifactData, T resource, string file, string path = "", string? contentType = "application/json", DateTimeOffset? updated = null, string? version = null, Checksum? checksum = null)
        => new(artifactData, new JsonArtifactResourceInfo<T>(resource, artifactData.Tool?.JsonOptions, new ArtifactResourceKey(artifactData.Info.Key, file, path), contentType, updated, version, checksum));
}
