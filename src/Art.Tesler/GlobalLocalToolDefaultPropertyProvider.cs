
using System.Text.Json;

namespace Art.Tesler;

public class GlobalLocalToolDefaultPropertyProvider : IToolDefaultPropertyProvider
{
    private readonly IToolDefaultPropertyProvider _globalProvider;
    private readonly IToolDefaultPropertyProvider _localProvider;

    public GlobalLocalToolDefaultPropertyProvider(
        IToolDefaultPropertyProvider globalProvider,
        IToolDefaultPropertyProvider localProvider)
    {
        _globalProvider = globalProvider;
        _localProvider = localProvider;
    }

    public IEnumerable<KeyValuePair<string, JsonElement>> EnumerateDefaultProperties(ArtifactToolID artifactToolId)
    {
        return _globalProvider.EnumerateDefaultProperties(artifactToolId).Concat(_localProvider.EnumerateDefaultProperties(artifactToolId));
    }

    public IEnumerable<ConfigProperty> GetProperties(ArtifactToolID artifactToolId, ConfigScopeFlags configScopeFlags)
    {
        if ((configScopeFlags & ConfigScopeFlags.Global) != 0)
        {
            foreach (var pair in _globalProvider.EnumerateDefaultProperties(artifactToolId))
            {
                yield return new ConfigProperty(ConfigScope.Global, pair.Key, pair.Value);
            }
        }
        if ((configScopeFlags & ConfigScopeFlags.Local) != 0)
        {
            foreach (var pair in _localProvider.EnumerateDefaultProperties(artifactToolId))
            {
                yield return new ConfigProperty(ConfigScope.Local, pair.Key, pair.Value);
            }
        }
    }

    public bool TryGetProperty(ArtifactToolID artifactToolId, string key, ConfigScopeFlags configScopeFlags, out ConfigProperty configProperty)
    {
        if ((configScopeFlags & ConfigScopeFlags.Local) != 0)
        {
            if (_localProvider.TryGetDefaultProperty(artifactToolId, key, out var subValue))
            {
                configProperty = new ConfigProperty(ConfigScope.Local, key, subValue);
                return true;
            }
        }
        if ((configScopeFlags & ConfigScopeFlags.Global) != 0)
        {
            if (_globalProvider.TryGetDefaultProperty(artifactToolId, key, out var subValue))
            {
                configProperty = new ConfigProperty(ConfigScope.Global, key, subValue);
                return true;
            }
        }
        configProperty = default;
        return false;
    }

    public bool TryGetDefaultProperty(ArtifactToolID artifactToolId, string key, out JsonElement value)
    {
        if (TryGetProperty(artifactToolId, key, ConfigScopeFlags.All, out ConfigProperty configProperty))
        {
            value = configProperty.Value;
            return true;
        }
        value = default;
        return false;
    }
}