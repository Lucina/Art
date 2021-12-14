using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Art;

/// <summary>
/// Stores data relevant to an artifact.
/// </summary>
public class ArtifactData : IReadOnlyDictionary<ArtifactResourceKey, ArtifactResourceInfo>
{
    /// <summary>
    /// Info for this artifact.
    /// </summary>
    public readonly ArtifactInfo Info;

    private readonly ArtifactTool? _tool;

    /// <summary>
    /// Resources for this artifact.
    /// </summary>
    public readonly Dictionary<ArtifactResourceKey, ArtifactResourceInfo> Resources = new();

    /// <inheritdoc/>
    public IEnumerable<ArtifactResourceKey> Keys => Resources.Keys;

    /// <inheritdoc/>
    public IEnumerable<ArtifactResourceInfo> Values => Resources.Values;

    /// <inheritdoc/>
    public int Count => Resources.Count;

    /// <inheritdoc/>
    public ArtifactResourceInfo this[ArtifactResourceKey key] => Resources[key];

    /// <summary>
    /// Creates a new instance of <see cref="ArtifactData"/>.
    /// </summary>
    /// <param name="tool">Tool id.</param>
    /// <param name="group">Group.</param>
    /// <param name="id">Artifact ID.</param>
    /// <param name="name">Name.</param>
    /// <param name="date">Artifact creation date.</param>
    /// <param name="updateDate">Artifact update date.</param>
    /// <param name="full">True if this is a full artifact.</param>
    public ArtifactData(string tool, string group, string id, string? name = null, DateTimeOffset? date = null, DateTimeOffset? updateDate = null, bool full = true)
    {
        Info = new ArtifactInfo(new ArtifactKey(tool, group, id), name, date, updateDate, full);
    }

    /// <summary>
    /// Creates a new instance of <see cref="ArtifactData"/>.
    /// </summary>
    /// <param name="key">Artifact key.</param>
    /// <param name="name">Name.</param>
    /// <param name="date">Artifact creation date.</param>
    /// <param name="updateDate">Artifact update date.</param>
    /// <param name="full">True if this is a full artifact.</param>
    public ArtifactData(ArtifactKey key, string? name = null, DateTimeOffset? date = null, DateTimeOffset? updateDate = null, bool full = true)
    {
        Info = new ArtifactInfo(key, name, date, updateDate, full);
    }

    /// <summary>
    /// Creates a new instance of <see cref="ArtifactData"/>.
    /// </summary>
    /// <param name="artifactTool">Artifact tool.</param>
    /// <param name="tool">Tool id.</param>
    /// <param name="group">Group.</param>
    /// <param name="id">Artifact ID.</param>
    /// <param name="name">Name.</param>
    /// <param name="date">Artifact creation date.</param>
    /// <param name="updateDate">Artifact update date.</param>
    /// <param name="full">True if this is a full artifact.</param>
    public ArtifactData(ArtifactTool artifactTool, string tool, string group, string id, string? name = null, DateTimeOffset? date = null, DateTimeOffset? updateDate = null, bool full = true)
    {
        Info = new ArtifactInfo(new ArtifactKey(tool, group, id), name, date, updateDate, full);
        _tool = artifactTool;
    }

    /// <summary>
    /// Creates a new instance of <see cref="ArtifactData"/>.
    /// </summary>
    /// <param name="artifactTool">Artifact tool.</param>
    /// <param name="key">Artifact key.</param>
    /// <param name="name">Name.</param>
    /// <param name="date">Artifact creation date.</param>
    /// <param name="updateDate">Artifact update date.</param>
    /// <param name="full">True if this is a full artifact.</param>
    public ArtifactData(ArtifactTool artifactTool, ArtifactKey key, string? name = null, DateTimeOffset? date = null, DateTimeOffset? updateDate = null, bool full = true)
    {
        Info = new ArtifactInfo(key, name, date, updateDate, full);
        _tool = artifactTool;
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
    {
        Resources[resource.Key] = resource;
    }

    /// <summary>
    /// Adds a resource to this instance.
    /// </summary>
    /// <param name="resource">Resource to add.</param>
    public void Add(ArtifactDataResource resource)
    {
        if (resource.Data != this) throw new ArgumentException("Cannot add a data resource with different source data object");
        Resources[resource.Info.Key] = resource.Info;
    }

    /// <summary>
    /// Adds resources to this instance.
    /// </summary>
    /// <param name="resources">Resources to add.</param>
    public void AddRange(IEnumerable<ArtifactResourceInfo> resources)
    {
        foreach (ArtifactResourceInfo resource in resources)
            Add(resource);
    }

    /// <summary>
    /// Creates a <see cref="StringArtifactResourceInfo"/> resource.
    /// </summary>
    /// <param name="resource">Resource.</param>
    /// <param name="key">Resource key.</param>
    /// <param name="updated">Updated date.</param>
    /// <param name="version">Version.</param>
    public ArtifactDataResource String(string resource, ArtifactResourceKey key, DateTimeOffset? updated = null, string? version = null)
        => new(this, new StringArtifactResourceInfo(resource, key, updated, version));

    /// <summary>
    /// Creates a <see cref="StringArtifactResourceInfo"/> resource.
    /// </summary>
    /// <param name="resource">Resource.</param>
    /// <param name="file">Filename.</param>
    /// <param name="path">Path.</param>
    /// <param name="updated">Updated date.</param>
    /// <param name="version">Version.</param>
    public ArtifactDataResource String(string resource, string file, string path = "", DateTimeOffset? updated = null, string? version = null)
        => new(this, new StringArtifactResourceInfo(resource, new ArtifactResourceKey(Info.Key, file, path), updated, version));

    /// <summary>
    /// Creates a <see cref="JsonArtifactResourceInfo{T}"/> resource.
    /// </summary>
    /// <param name="resource">Resource.</param>
    /// <param name="serializerOptions">Serializer options.</param>
    /// <param name="key">Resource key.</param>
    /// <param name="updated">Updated date.</param>
    /// <param name="version">Version.</param>
    public ArtifactDataResource Json<T>(T resource, JsonSerializerOptions? serializerOptions, ArtifactResourceKey key, DateTimeOffset? updated = null, string? version = null)
        => new(this, new JsonArtifactResourceInfo<T>(resource, serializerOptions, key, updated, version));

    /// <summary>
    /// Creates a <see cref="JsonArtifactResourceInfo{T}"/> resource.
    /// </summary>
    /// <param name="resource">Resource.</param>
    /// <param name="serializerOptions">Serializer options.</param>
    /// <param name="file">Filename.</param>
    /// <param name="path">Path.</param>
    /// <param name="updated">Updated date.</param>
    /// <param name="version">Version.</param>
    public ArtifactDataResource Json<T>(T resource, JsonSerializerOptions? serializerOptions, string file, string path = "", DateTimeOffset? updated = null, string? version = null)
        => new(this, new JsonArtifactResourceInfo<T>(resource, serializerOptions, new ArtifactResourceKey(Info.Key, file, path), updated, version));

    /// <summary>
    /// Creates a <see cref="JsonArtifactResourceInfo{T}"/> resource.
    /// </summary>
    /// <param name="resource">Resource.</param>
    /// <param name="key">Resource key.</param>
    /// <param name="updated">Updated date.</param>
    /// <param name="version">Version.</param>
    public ArtifactDataResource Json<T>(T resource, ArtifactResourceKey key, DateTimeOffset? updated = null, string? version = null)
        => new(this, new JsonArtifactResourceInfo<T>(resource, _tool?.JsonOptions, key, updated, version));

    /// <summary>
    /// Creates a <see cref="JsonArtifactResourceInfo{T}"/> resource.
    /// </summary>
    /// <param name="resource">Resource.</param>
    /// <param name="file">Filename.</param>
    /// <param name="path">Path.</param>
    /// <param name="updated">Updated date.</param>
    /// <param name="version">Version.</param>
    public ArtifactDataResource Json<T>(T resource, string file, string path = "", DateTimeOffset? updated = null, string? version = null)
        => new(this, new JsonArtifactResourceInfo<T>(resource, _tool?.JsonOptions, new ArtifactResourceKey(Info.Key, file, path), updated, version));

    /// <summary>
    /// Creates a <see cref="UriArtifactResourceInfo"/> resource.
    /// </summary>
    /// <param name="artifactTool">Artifact tool.</param>
    /// <param name="uri">URI.</param>
    /// <param name="key">Resource key.</param>
    /// <param name="updated">Updated date.</param>
    /// <param name="version">Version.</param>
    /// <param name="origin">Request origin.</param>
    /// <param name="referrer">Request referrer.</param>
    public ArtifactDataResource Uri(HttpArtifactTool artifactTool, Uri uri, ArtifactResourceKey key, DateTimeOffset? updated = null, string? version = null, string? origin = null, string? referrer = null)
        => new(this, new UriArtifactResourceInfo(artifactTool, uri, origin, referrer, key, updated, version));

    /// <summary>
    /// Creates a <see cref="UriArtifactResourceInfo"/> resource.
    /// </summary>
    /// <param name="artifactTool">Artifact tool.</param>
    /// <param name="uri">URI.</param>
    /// <param name="file">Filename.</param>
    /// <param name="path">Path.</param>
    /// <param name="updated">Updated date.</param>
    /// <param name="version">Version.</param>
    /// <param name="origin">Request origin.</param>
    /// <param name="referrer">Request referrer.</param>
    public ArtifactDataResource Uri(HttpArtifactTool artifactTool, Uri uri, string file, string path = "", DateTimeOffset? updated = null, string? version = null, string? origin = null, string? referrer = null)
        => new(this, new UriArtifactResourceInfo(artifactTool, uri, origin, referrer, new ArtifactResourceKey(Info.Key, file, path), updated, version));

    /// <summary>
    /// Creates a <see cref="UriArtifactResourceInfo"/> resource.
    /// </summary>
    /// <param name="uri">URI.</param>
    /// <param name="key">Resource key.</param>
    /// <param name="updated">Updated date.</param>
    /// <param name="version">Version.</param>
    /// <param name="origin">Request origin.</param>
    /// <param name="referrer">Request referrer.</param>
    public ArtifactDataResource Uri(Uri uri, ArtifactResourceKey key, DateTimeOffset? updated = null, string? version = null, string? origin = null, string? referrer = null)
        => Uri(GetArtifactTool<HttpArtifactTool>(), uri, key, updated, version, origin, referrer);

    /// <summary>
    /// Creates a <see cref="UriArtifactResourceInfo"/> resource.
    /// </summary>
    /// <param name="uri">URI.</param>
    /// <param name="file">Filename.</param>
    /// <param name="path">Path.</param>
    /// <param name="updated">Updated date.</param>
    /// <param name="version">Version.</param>
    /// <param name="origin">Request origin.</param>
    /// <param name="referrer">Request referrer.</param>
    public ArtifactDataResource Uri(Uri uri, string file, string path = "", DateTimeOffset? updated = null, string? version = null, string? origin = null, string? referrer = null)
        => Uri(GetArtifactTool<HttpArtifactTool>(), uri, file, path, updated, version, origin, referrer);

    /// <summary>
    /// Creates a <see cref="UriStringArtifactResourceInfo"/> resource.
    /// </summary>
    /// <param name="artifactTool">Artifact tool.</param>
    /// <param name="uri">URI.</param>
    /// <param name="key">Resource key.</param>
    /// <param name="updated">Updated date.</param>
    /// <param name="version">Version.</param>
    /// <param name="origin">Request origin.</param>
    /// <param name="referrer">Request referrer.</param>
    public ArtifactDataResource UriString(HttpArtifactTool artifactTool, string uri, ArtifactResourceKey key, DateTimeOffset? updated = null, string? version = null, string? origin = null, string? referrer = null)
        => new(this, new UriStringArtifactResourceInfo(artifactTool, uri, origin, referrer, key, updated, version));

    /// <summary>
    /// Creates a <see cref="UriStringArtifactResourceInfo"/> resource.
    /// </summary>
    /// <param name="artifactTool">Artifact tool.</param>
    /// <param name="uri">URI.</param>
    /// <param name="file">Filename.</param>
    /// <param name="path">Path.</param>
    /// <param name="updated">Updated date.</param>
    /// <param name="version">Version.</param>
    /// <param name="origin">Request origin.</param>
    /// <param name="referrer">Request referrer.</param>
    public ArtifactDataResource UriString(HttpArtifactTool artifactTool, string uri, string file, string path = "", DateTimeOffset? updated = null, string? version = null, string? origin = null, string? referrer = null)
        => new(this, new UriStringArtifactResourceInfo(artifactTool, uri, origin, referrer, new ArtifactResourceKey(Info.Key, file, path), updated, version));

    /// <summary>
    /// Creates a <see cref="UriStringArtifactResourceInfo"/> resource.
    /// </summary>
    /// <param name="uri">URI.</param>
    /// <param name="key">Resource key.</param>
    /// <param name="updated">Updated date.</param>
    /// <param name="version">Version.</param>
    /// <param name="origin">Request origin.</param>
    /// <param name="referrer">Request referrer.</param>
    public ArtifactDataResource UriString(string uri, ArtifactResourceKey key, DateTimeOffset? updated = null, string? version = null, string? origin = null, string? referrer = null)
        => UriString(GetArtifactTool<HttpArtifactTool>(), uri, key, updated, version, origin, referrer);

    /// <summary>
    /// Creates a <see cref="UriStringArtifactResourceInfo"/> resource.
    /// </summary>
    /// <param name="uri">URI.</param>
    /// <param name="file">Filename.</param>
    /// <param name="path">Path.</param>
    /// <param name="updated">Updated date.</param>
    /// <param name="version">Version.</param>
    /// <param name="origin">Request origin.</param>
    /// <param name="referrer">Request referrer.</param>
    public ArtifactDataResource UriString(string uri, string file, string path = "", DateTimeOffset? updated = null, string? version = null, string? origin = null, string? referrer = null)
        => UriString(GetArtifactTool<HttpArtifactTool>(), uri, file, path, updated, version, origin, referrer);

    /// <summary>
    /// Creates a <see cref="HttpRequestMessageArtifactResourceInfo"/> resource.
    /// </summary>
    /// <param name="artifactTool">Artifact tool.</param>
    /// <param name="request">Request.</param>
    /// <param name="key">Resource key.</param>
    /// <param name="updated">Updated date.</param>
    /// <param name="version">Version.</param>
    public ArtifactDataResource HttpRequestMessage(HttpArtifactTool artifactTool, HttpRequestMessage request, ArtifactResourceKey key, DateTimeOffset? updated = null, string? version = null)
        => new(this, new HttpRequestMessageArtifactResourceInfo(artifactTool, request, key, updated, version));

    /// <summary>
    /// Creates a <see cref="HttpRequestMessageArtifactResourceInfo"/> resource.
    /// </summary>
    /// <param name="artifactTool">Artifact tool.</param>
    /// <param name="request">Request.</param>
    /// <param name="file">Filename.</param>
    /// <param name="path">Path.</param>
    /// <param name="updated">Updated date.</param>
    /// <param name="version">Version.</param>
    public ArtifactDataResource HttpRequestMessage(HttpArtifactTool artifactTool, HttpRequestMessage request, string file, string path = "", DateTimeOffset? updated = null, string? version = null)
        => new(this, new HttpRequestMessageArtifactResourceInfo(artifactTool, request, new ArtifactResourceKey(Info.Key, file, path), updated, version));

    /// <summary>
    /// Creates a <see cref="HttpRequestMessageArtifactResourceInfo"/> resource.
    /// </summary>
    /// <param name="request">Request.</param>
    /// <param name="key">Resource key.</param>
    /// <param name="updated">Updated date.</param>
    /// <param name="version">Version.</param>
    public ArtifactDataResource HttpRequestMessage(HttpRequestMessage request, ArtifactResourceKey key, DateTimeOffset? updated = null, string? version = null)
        => HttpRequestMessage(GetArtifactTool<HttpArtifactTool>(), request, key, updated, version);

    /// <summary>
    /// Creates a <see cref="HttpRequestMessageArtifactResourceInfo"/> resource.
    /// </summary>
    /// <param name="request">Request.</param>
    /// <param name="file">Filename.</param>
    /// <param name="path">Path.</param>
    /// <param name="updated">Updated date.</param>
    /// <param name="version">Version.</param>
    public ArtifactDataResource HttpRequestMessage(HttpRequestMessage request, string file, string path = "", DateTimeOffset? updated = null, string? version = null)
        => HttpRequestMessage(GetArtifactTool<HttpArtifactTool>(), request, file, path, updated, version);

    private T GetArtifactTool<T>() where T : ArtifactTool
        => (_tool
            ?? throw new InvalidOperationException("Data object was not initialized with an artifact tool")) as T
           ?? throw new InvalidCastException("Tool type for this data object is not compatible with needed type");

    /// <inheritdoc/>
    public bool ContainsKey(ArtifactResourceKey key) => Resources.ContainsKey(key);

    /// <inheritdoc/>
    public bool TryGetValue(ArtifactResourceKey key, [MaybeNullWhen(false)] out ArtifactResourceInfo value) => Resources.TryGetValue(key, out value);

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<ArtifactResourceKey, ArtifactResourceInfo>> GetEnumerator() => Resources.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Resources).GetEnumerator();
}
