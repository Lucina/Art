using System.Linq.Expressions;
using Art.Resources;

namespace Art.Management;

/// <summary>
/// Represents an in-memory artifact registration manager.
/// </summary>
public class InMemoryArtifactRegistrationManager : ArtifactRegistrationManager
{
    private readonly Dictionary<ArtifactKey, ArtifactInfo> _artifacts = new();

    private readonly Dictionary<ArtifactKey, Dictionary<ArtifactResourceKey, ArtifactResourceInfo>> _resources = new();

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
    {
        if (_resources.TryGetValue(key, out var dict))
        {
            return Task.FromResult(dict.Values.ToList());
        }
        return Task.FromResult(new List<ArtifactResourceInfo>());
    }

    /// <inheritdoc />
    public override ValueTask AddArtifactAsync(ArtifactInfo artifactInfo, CancellationToken ct = default)
    {
        _artifacts[artifactInfo.Key] = artifactInfo;
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public override ValueTask<ArtifactInfo?> TryGetArtifactAsync(ArtifactKey key, CancellationToken ct = default)
    {
        return new ValueTask<ArtifactInfo?>(_artifacts.TryGetValue(key, out ArtifactInfo? value) ? value : null);
    }

    /// <inheritdoc />
    public override ValueTask AddResourceAsync(ArtifactResourceInfo artifactResourceInfo, CancellationToken ct = default)
    {
        if (!_resources.TryGetValue(artifactResourceInfo.Key.Artifact, out var dict))
        {
            _resources.Add(artifactResourceInfo.Key.Artifact, dict = new Dictionary<ArtifactResourceKey, ArtifactResourceInfo>());
        }
        dict.Add(artifactResourceInfo.Key, artifactResourceInfo);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public override ValueTask<ArtifactResourceInfo?> TryGetResourceAsync(ArtifactResourceKey key, CancellationToken ct = default)
    {
        return new ValueTask<ArtifactResourceInfo?>(_resources.TryGetValue(key.Artifact, out var dict) && dict.TryGetValue(key, out var value) ? value : null);
    }

    /// <inheritdoc />
    public override ValueTask RemoveArtifactAsync(ArtifactKey key, CancellationToken cancellationToken = default)
    {
        _artifacts.Remove(key);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public override ValueTask RemoveResourceAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default)
    {
        if (_resources.TryGetValue(key.Artifact, out var dict))
        {
            dict.Remove(key);
        }
        return ValueTask.CompletedTask;
    }
}
