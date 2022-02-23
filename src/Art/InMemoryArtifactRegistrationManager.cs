using System.Linq.Expressions;

namespace Art;

/// <summary>
/// Represents an in-memory artifact registration manager.
/// </summary>
public class InMemoryArtifactRegistrationManager : ArtifactRegistrationManager
{
    /// <summary>
    /// Artifacts that have been searched for.
    /// </summary>
    public IReadOnlySet<ArtifactInfo> CheckedArtifacts => _checkedArtifacts;

    private readonly HashSet<ArtifactInfo> _checkedArtifacts = new();

    /// <summary>
    /// Artifact keys that have been searched for.
    /// </summary>
    public IReadOnlySet<ArtifactKey> CheckedIds => _checkedIds;

    private readonly HashSet<ArtifactKey> _checkedIds = new();

    /// <summary>
    /// Artifacts stored in manager.
    /// </summary>
    public IReadOnlyDictionary<ArtifactKey, ArtifactInfo> Artifacts => _artifacts;

    private readonly Dictionary<ArtifactKey, ArtifactInfo> _artifacts = new();

    /// <summary>
    /// Resources stored in manager.
    /// </summary>
    public IReadOnlyDictionary<ArtifactResourceKey, ArtifactResourceInfo> Resources => _resources;

    private readonly Dictionary<ArtifactResourceKey, ArtifactResourceInfo> _resources = new();

    /// <inheritdoc />
    public override Task<List<ArtifactInfo>> ListArtifactsAsync(CancellationToken cancellationToken = new())
        => Task.FromResult(_artifacts.Values.ToList());

    /// <inheritdoc />
    public override Task<List<ArtifactInfo>> ListArtifactsAsync(Expression<Func<ArtifactInfoModel, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var x = predicate.Compile();
        return Task.FromResult(_artifacts.Values.Where(v => x(v)).ToList());
    }

    /// <inheritdoc />
    public override Task<List<ArtifactInfo>> ListArtifactsAsync(string tool, CancellationToken cancellationToken = new())
        => Task.FromResult(_artifacts.Values.Where(v => v.Key.Tool == tool).ToList());

    /// <inheritdoc />
    public override Task<List<ArtifactInfo>> ListArtifactsAsync(string tool, string group, CancellationToken cancellationToken = new())
        => Task.FromResult(_artifacts.Values.Where(v => v.Key.Tool == tool && v.Key.Group == group).ToList());

    /// <inheritdoc />
    public override Task<List<ArtifactResourceInfo>> ListResourcesAsync(ArtifactKey key, CancellationToken cancellationToken = new())
        => Task.FromResult(_resources.Values.Where(v => v.Key.Artifact == key).ToList());

    /// <inheritdoc />
    public override ValueTask AddArtifactAsync(ArtifactInfo artifactInfo, CancellationToken ct = default)
    {
        _checkedArtifacts.Add(artifactInfo);
        _checkedIds.Add(artifactInfo.Key);
        _artifacts[artifactInfo.Key] = artifactInfo;
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public override ValueTask<ArtifactInfo?> TryGetArtifactAsync(ArtifactKey key, CancellationToken ct = default)
    {
        _checkedIds.Add(key);
        return new ValueTask<ArtifactInfo?>(_artifacts.TryGetValue(key, out ArtifactInfo? value) ? value : null);
    }

    /// <inheritdoc />
    public override ValueTask AddResourceAsync(ArtifactResourceInfo artifactResourceInfo, CancellationToken ct = default)
    {
        _resources.Add(artifactResourceInfo.Key, artifactResourceInfo);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public override ValueTask<ArtifactResourceInfo?> TryGetResourceAsync(ArtifactResourceKey key, CancellationToken ct = default)
    {
        return new ValueTask<ArtifactResourceInfo?>(_resources.TryGetValue(key, out ArtifactResourceInfo? value) ? value : null);
    }

    /// <inheritdoc />
    public override ValueTask RemoveArtifactAsync(ArtifactKey key, CancellationToken cancellationToken = default)
    {
        _checkedIds.Add(key);
        _artifacts.Remove(key);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public override ValueTask RemoveResourceAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default)
    {
        _resources.Remove(key);
        return ValueTask.CompletedTask;
    }
}
