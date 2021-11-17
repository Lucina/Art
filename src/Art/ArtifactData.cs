using System;
using System.Text.Json;

namespace Art;

/// <summary>
/// Stores data relevant to an artifact.
/// </summary>
public class ArtifactData
{
    /// <summary>
    /// Info for this artifact.
    /// </summary>
    public readonly ArtifactInfo Info;

    /// <summary>
    /// Resources for this artifact.
    /// </summary>
    public readonly List<ArtifactResourceInfo> Resources = new();

    /// <summary>
    /// Creates a new instance of <see cref="ArtifactData"/>.
    /// </summary>
    /// <param name="id">Artifact ID.</param>
    /// <param name="date">Artifact creation date.</param>
    /// <param name="updateDate">Artifact update date.</param>
    /// <param name="properties">Artifact properties.</param>
    public ArtifactData(string id, DateTimeOffset? date = null, DateTimeOffset? updateDate = null, IReadOnlyDictionary<string, JsonElement>? properties = null)
    {
        Info = new ArtifactInfo(id, date, updateDate, properties ?? ArtifactInfo.EmptyProperties);
    }

    /// <summary>
    /// Creates a new instance of <see cref="ArtifactData"/>.
    /// </summary>
    /// <param name="info">Artifact info.</param>
    public ArtifactData(ArtifactInfo info)
    {
        Info = info;
    }

    /// <summary>
    /// Adds a resource to this instance.
    /// </summary>
    /// <param name="resource">Resource to add.</param>
    public void Add(ArtifactResourceInfo resource)
        => Resources.Add(resource);

    /// <summary>
    /// Adds resources to this instance.
    /// </summary>
    /// <param name="resources">Resources to add.</param>
    public void AddRange(IEnumerable<ArtifactResourceInfo> resources)
        => Resources.AddRange(resources);

    /// <summary>
    /// Adds a <see cref="StringArtifactResourceInfo"/> instance.
    /// </summary>
    /// <param name="resource">Resource.</param>
    /// <param name="artifactId">Artifact ID.</param>
    /// <param name="file">Filename.</param>
    /// <param name="path">Path.</param>
    /// <param name="inArtifactFolder">If false, sent to common directory.</param>
    /// <param name="properties">Resource properties.</param>
    public void AddString(string resource, string artifactId, string file, string? path = null, bool inArtifactFolder = false, IReadOnlyDictionary<string, JsonElement>? properties = null)
        => Resources.Add(new StringArtifactResourceInfo(resource, artifactId, file, path, inArtifactFolder, properties ?? ArtifactResourceInfo.EmptyProperties));

    /// <summary>
    /// Adds a <see cref="UriArtifactResourceInfo"/> instance.
    /// </summary>
    /// <param name="artifactDumper">Artifact dumper.</param>
    /// <param name="uri">URI.</param>
    /// <param name="artifactId">Artifact ID.</param>
    /// <param name="file">Filename.</param>
    /// <param name="path">Path.</param>
    /// <param name="inArtifactFolder">If false, sent to common directory.</param>
    /// <param name="properties">Resource properties.</param>
    public void AddUri(HttpArtifactDumper artifactDumper, Uri uri, string artifactId, string file, string? path = null, bool inArtifactFolder = false, IReadOnlyDictionary<string, JsonElement>? properties = null)
        => Resources.Add(new UriArtifactResourceInfo(artifactDumper, uri, artifactId, file, path, inArtifactFolder, properties ?? ArtifactResourceInfo.EmptyProperties));

    /// <summary>
    /// Adds a <see cref="JsonArtifactResourceInfo{Task}"/> instance.
    /// </summary>
    /// <param name="resource">Resource.</param>
    /// <param name="serializerOptions">Serializer options.</param>
    /// <param name="artifactId">Artifact ID.</param>
    /// <param name="file">Filename.</param>
    /// <param name="path">Path.</param>
    /// <param name="inArtifactFolder">If false, sent to common directory.</param>
    /// <param name="properties">Resource properties.</param>
    public void AddJson<T>(T resource, JsonSerializerOptions? serializerOptions, string artifactId, string file, string? path = null, bool inArtifactFolder = false, IReadOnlyDictionary<string, JsonElement>? properties = null)
        => Resources.Add(new JsonArtifactResourceInfo<T>(resource, serializerOptions, artifactId, file, path, inArtifactFolder, properties ?? ArtifactResourceInfo.EmptyProperties));

    /// <summary>
    /// Adds a <see cref="UriStringArtifactResourceInfo"/> instance.
    /// </summary>
    /// <param name="artifactDumper">Artifact dumper.</param>
    /// <param name="uri">URI.</param>
    /// <param name="artifactId">Artifact ID.</param>
    /// <param name="file">Filename.</param>
    /// <param name="path">Path.</param>
    /// <param name="inArtifactFolder">If false, sent to common directory.</param>
    /// <param name="properties">Resource properties.</param>
    public void AddUriString(HttpArtifactDumper artifactDumper, string uri, string artifactId, string file, string? path = null, bool inArtifactFolder = false, IReadOnlyDictionary<string, JsonElement>? properties = null)
        => Resources.Add(new UriStringArtifactResourceInfo(artifactDumper, uri, artifactId, file, path, inArtifactFolder, properties ?? ArtifactResourceInfo.EmptyProperties));

    /// <summary>
    /// Adds a <see cref="HttpRequestMessageArtifactResourceInfo"/> instance.
    /// </summary>
    /// <param name="artifactDumper">Artifact dumper.</param>
    /// <param name="request">Request.</param>
    /// <param name="artifactId">Artifact ID.</param>
    /// <param name="file">Filename.</param>
    /// <param name="path">Path.</param>
    /// <param name="inArtifactFolder">If false, sent to common directory.</param>
    /// <param name="properties">Resource properties.</param>
    public void AddHttpRequestMessage(HttpArtifactDumper artifactDumper, HttpRequestMessage request, string artifactId, string file, string? path = null, bool inArtifactFolder = false, IReadOnlyDictionary<string, JsonElement>? properties = null)
        => Resources.Add(new HttpRequestMessageArtifactResourceInfo(artifactDumper, request, artifactId, file, path, inArtifactFolder, properties ?? ArtifactResourceInfo.EmptyProperties));
}

