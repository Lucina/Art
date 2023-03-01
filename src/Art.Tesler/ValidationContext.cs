using System.Security.Cryptography;
using Art.Common;

namespace Art.Tesler;

public class ValidationContext
{
    private readonly IArtifactToolRegistryStore _pluginStore;
    private readonly Dictionary<ArtifactKey, List<ArtifactResourceInfo>> _failed = new();
    private readonly IArtifactRegistrationManager _arm;
    private readonly IArtifactDataManager _adm;
    private readonly IToolLogHandler _l;

    public bool AnyFailed => _failed.Count != 0;

    public IEnumerable<KeyValuePair<ArtifactKey, int>> GetFailureCountsByKey() => _failed.Select(v => new KeyValuePair<ArtifactKey, int>(v.Key, v.Value.Count));

    public int CountResourceFailures() => _failed.Sum(v => v.Value.Count);

    public ValidationContext(IArtifactToolRegistryStore pluginStore, IArtifactRegistrationManager arm, IArtifactDataManager adm, IToolLogHandler l)
    {
        _pluginStore = pluginStore;
        _arm = arm;
        _adm = adm;
        _l = l;
    }

    private void AddFail(ArtifactResourceInfo r)
    {
        if (!_failed.TryGetValue(r.Key.Artifact, out var list)) list = _failed[r.Key.Artifact] = new List<ArtifactResourceInfo>();
        list.Add(r);
    }

    public async Task<ValidationProcessResult> ProcessAsync(List<ArtifactInfo> artifacts, ChecksumSource? checksumSourceForAdd)
    {
        int artifactCount = 0, resourceCount = 0;
        if (checksumSourceForAdd == null)
        {
            foreach (ArtifactInfo inf in artifacts)
            {
                var result = await ProcessAsync(inf, (ActiveHashAlgorithm?)null);
                artifactCount += result.Artifacts;
                resourceCount += result.Resources;
            }
        }
        else
        {
            using var hashAlgorithm = checksumSourceForAdd.CreateHashAlgorithm();
            foreach (ArtifactInfo inf in artifacts)
            {
                var result = await ProcessAsync(inf, new ActiveHashAlgorithm(checksumSourceForAdd.Id, hashAlgorithm));
                artifactCount += result.Artifacts;
                resourceCount += result.Resources;
            }
        }
        return new ValidationProcessResult(artifactCount, resourceCount);
    }

    public async Task<ValidationProcessResult> ProcessAsync(ArtifactInfo artifact, ChecksumSource? checksumSourceForAdd)
    {
        ValidationProcessResult result;
        if (checksumSourceForAdd == null)
        {
            result = await ProcessAsync(artifact, (ActiveHashAlgorithm?)null);
        }
        else
        {
            using var hashAlgorithm = checksumSourceForAdd.CreateHashAlgorithm();
            result = await ProcessAsync(artifact, new ActiveHashAlgorithm(checksumSourceForAdd.Id, hashAlgorithm));
        }
        return result;
    }

    private readonly record struct ActiveHashAlgorithm(string Id, HashAlgorithm HashAlgorithm);

    private async Task<ValidationProcessResult> ProcessAsync(ArtifactInfo artifact, ActiveHashAlgorithm? activeHashAlgorithmForAdd)
    {
        int resourceCount = 0;
        foreach (ArtifactResourceInfo rInf in await _arm.ListResourcesAsync(artifact.Key))
        {
            resourceCount++;
            if (!await _adm.ExistsAsync(rInf.Key))
            {
                AddFail(rInf);
                continue;
            }
            if (rInf.Checksum == null)
            {
                if (activeHashAlgorithmForAdd is not { } activeHashAlgorithmForAddReal)
                {
                    AddFail(rInf);
                }
                else
                {
                    await using Stream sourceStreamAdd = await _adm.OpenInputStreamAsync(rInf.Key);
                    byte[] newHash = await activeHashAlgorithmForAddReal.HashAlgorithm.ComputeHashAsync(sourceStreamAdd);
                    await _arm.AddResourceAsync(rInf with { Checksum = new Checksum(activeHashAlgorithmForAddReal.Id, newHash) });
                }
                continue;
            }
            if (!ChecksumSource.DefaultSources.TryGetValue(rInf.Checksum.Id, out ChecksumSource? checksumSource))
            {
                AddFail(rInf);
                continue;
            }
            using var hashAlgorithm = checksumSource.CreateHashAlgorithm();
            await using Stream sourceStream = await _adm.OpenInputStreamAsync(rInf.Key);
            byte[] existingHash = await hashAlgorithm.ComputeHashAsync(sourceStream);
            if (!rInf.Checksum.Value.AsSpan().SequenceEqual(existingHash)) AddFail(rInf);
        }
        return new ValidationProcessResult(1, resourceCount);
    }

    public async Task<ValidationProcessResult> ProcessAsync(IEnumerable<ArtifactToolProfile> profiles, ChecksumSource? checksumSourceForAdd)
    {
        int artifactCount = 0, resourceCount = 0;
        foreach (ArtifactToolProfile profile in profiles)
        {
            var context = _pluginStore.LoadRegistry(ArtifactToolProfileUtil.GetID(profile.Tool)); // InvalidOperationException
            if (!context.TryLoad(profile.GetID(), out var t))
                throw new InvalidOperationException($"Unknown tool {profile.Tool}");
            using IArtifactTool tool = t;
            var pp = profile.WithCoreTool(tool);
            string group = pp.GetGroupOrFallback(tool.GroupFallback);
            _l.Log($"Processing entries for profile {pp.Tool}/{group}", null, LogLevel.Title);
            var artifacts = await _arm.ListArtifactsAsync(pp.Tool, group);
            // respect profile's artifact list
            // (checking against it being a find tool matches the behaviour of dump / list proxies)
            if (profile.Options.TryGetOption("artifactList", out string[]? artifactList, SourceGenerationContext.s_context.StringArray) && tool is IArtifactFindTool)
            {
                var set = artifactList.ToHashSet();
                artifacts.RemoveAll(v => set.Contains(v.Key.Id));
            }
            var result = await ProcessAsync(artifacts, checksumSourceForAdd);
            _l.Log($"Processed {result.Artifacts} artifacts and {result.Resources} resources for profile {pp.Tool}/{group}", null, LogLevel.Information);
            artifactCount += result.Artifacts;
            resourceCount += result.Resources;
        }
        return new ValidationProcessResult(artifactCount, resourceCount);
    }

    public RepairContext CreateRepairContext() => new(_pluginStore, _failed, _arm, _adm, _l);
}

public readonly record struct ValidationProcessResult(int Artifacts, int Resources);
