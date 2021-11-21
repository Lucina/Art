using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Art;

/// <summary>
/// Stores data relevant to an artifact.
/// </summary>
public class ArtifactData : IReadOnlyDictionary<RK, ArtifactResourceInfo>
{
    /// <summary>
    /// Info for this artifact.
    /// </summary>
    public readonly ArtifactInfo Info;

    /// <summary>
    /// Resources for this artifact.
    /// </summary>
    public readonly Dictionary<RK, ArtifactResourceInfo> Resources = new();

    /// <inheritdoc/>
    public IEnumerable<RK> Keys => Resources.Keys;

    /// <inheritdoc/>
    public IEnumerable<ArtifactResourceInfo> Values => Resources.Values;

    /// <inheritdoc/>
    public int Count => Resources.Count;

    /// <inheritdoc/>
    public ArtifactResourceInfo this[RK key] => Resources[key];

    /// <summary>
    /// Creates a new instance of <see cref="ArtifactData"/>.
    /// </summary>
    /// <param name="tool">Tool id.</param>
    /// <param name="group">Group.</param>
    /// <param name="id">Artifact ID.</param>
    /// <param name="date">Artifact creation date.</param>
    /// <param name="updateDate">Artifact update date.</param>
    /// <param name="properties">Artifact properties.</param>
    public ArtifactData(string tool, string group, string id, DateTimeOffset? date = null, DateTimeOffset? updateDate = null, IReadOnlyDictionary<string, JsonElement>? properties = null)
    {
        Info = new ArtifactInfo(tool, group, id, date, updateDate, properties ?? ArtifactInfo.EmptyProperties);
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
        Resources[resource.ToKey()] = resource;
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
    /// Adds a <see cref="StringArtifactResourceInfo"/> instance.
    /// </summary>
    /// <param name="resource">Resource.</param>
    /// <param name="file">Filename.</param>
    /// <param name="path">Path.</param>
    /// <param name="inArtifactFolder">If false, sent to common directory.</param>
    /// <param name="properties">Resource properties.</param>
    public void AddString(string resource, string file, string? path = null, bool inArtifactFolder = true, IReadOnlyDictionary<string, JsonElement>? properties = null)
        => Add(new StringArtifactResourceInfo(resource, Info.Id, file, path, inArtifactFolder, properties ?? ArtifactResourceInfo.EmptyProperties));

    /// <summary>
    /// Adds a <see cref="UriArtifactResourceInfo"/> instance.
    /// </summary>
    /// <param name="artifactTool">Artifact tool.</param>
    /// <param name="uri">URI.</param>
    /// <param name="file">Filename.</param>
    /// <param name="path">Path.</param>
    /// <param name="inArtifactFolder">If false, sent to common directory.</param>
    /// <param name="properties">Resource properties.</param>
    public void AddUri(HttpArtifactTool artifactTool, Uri uri, string file, string? path = null, bool inArtifactFolder = true, IReadOnlyDictionary<string, JsonElement>? properties = null)
        => Add(new UriArtifactResourceInfo(artifactTool, uri, Info.Id, file, path, inArtifactFolder, properties ?? ArtifactResourceInfo.EmptyProperties));

    /// <summary>
    /// Adds a <see cref="JsonArtifactResourceInfo{Task}"/> instance.
    /// </summary>
    /// <param name="resource">Resource.</param>
    /// <param name="serializerOptions">Serializer options.</param>
    /// <param name="file">Filename.</param>
    /// <param name="path">Path.</param>
    /// <param name="inArtifactFolder">If false, sent to common directory.</param>
    /// <param name="properties">Resource properties.</param>
    public void AddJson<T>(T resource, JsonSerializerOptions? serializerOptions, string file, string? path = null, bool inArtifactFolder = true, IReadOnlyDictionary<string, JsonElement>? properties = null)
        => Add(new JsonArtifactResourceInfo<T>(resource, serializerOptions, Info.Id, file, path, inArtifactFolder, properties ?? ArtifactResourceInfo.EmptyProperties));

    /// <summary>
    /// Adds a <see cref="UriStringArtifactResourceInfo"/> instance.
    /// </summary>
    /// <param name="artifactTool">Artifact tool.</param>
    /// <param name="uri">URI.</param>
    /// <param name="file">Filename.</param>
    /// <param name="path">Path.</param>
    /// <param name="inArtifactFolder">If false, sent to common directory.</param>
    /// <param name="properties">Resource properties.</param>
    public void AddUriString(HttpArtifactTool artifactTool, string uri, string file, string? path = null, bool inArtifactFolder = true, IReadOnlyDictionary<string, JsonElement>? properties = null)
        => Add(new UriStringArtifactResourceInfo(artifactTool, uri, Info.Id, file, path, inArtifactFolder, properties ?? ArtifactResourceInfo.EmptyProperties));

    /// <summary>
    /// Adds a <see cref="HttpRequestMessageArtifactResourceInfo"/> instance.
    /// </summary>
    /// <param name="artifactTool">Artifact tool.</param>
    /// <param name="request">Request.</param>
    /// <param name="file">Filename.</param>
    /// <param name="path">Path.</param>
    /// <param name="inArtifactFolder">If false, sent to common directory.</param>
    /// <param name="properties">Resource properties.</param>
    public void AddHttpRequestMessage(HttpArtifactTool artifactTool, HttpRequestMessage request, string file, string? path = null, bool inArtifactFolder = true, IReadOnlyDictionary<string, JsonElement>? properties = null)
        => Add(new HttpRequestMessageArtifactResourceInfo(artifactTool, request, Info.Id, file, path, inArtifactFolder, properties ?? ArtifactResourceInfo.EmptyProperties));

    /// <inheritdoc/>
    public bool ContainsKey(RK key) => Resources.ContainsKey(key);

    /// <inheritdoc/>
    public bool TryGetValue(RK key, [MaybeNullWhen(false)] out ArtifactResourceInfo value) => Resources.TryGetValue(key, out value);

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<RK, ArtifactResourceInfo>> GetEnumerator() => Resources.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Resources).GetEnumerator();
}

