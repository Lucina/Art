
using System.Text.Json;

namespace Art.Tesler.Properties;

public class GlobalLocalToolPropertyProvider : IWritableScopedToolPropertyProvider
{
    private readonly IToolPropertyProvider _globalProvider;
    private readonly IToolPropertyProvider _localProvider;

    public GlobalLocalToolPropertyProvider(
        IToolPropertyProvider globalProvider,
        IToolPropertyProvider localProvider)
    {
        _globalProvider = globalProvider;
        _localProvider = localProvider;
    }

    public IEnumerable<KeyValuePair<string, JsonElement>> GetProperties(ArtifactToolID artifactToolId)
    {
        return _globalProvider.GetProperties(artifactToolId).Concat(_localProvider.GetProperties(artifactToolId));
    }

    public IEnumerable<ConfigProperty> GetProperties(ArtifactToolID artifactToolId, ConfigScopeFlags configScopeFlags)
    {
        if ((configScopeFlags & ConfigScopeFlags.Global) != 0)
        {
            foreach (var pair in _globalProvider.GetProperties(artifactToolId))
            {
                yield return new ConfigProperty(ConfigScope.Global, pair.Key, pair.Value);
            }
        }
        if ((configScopeFlags & ConfigScopeFlags.Local) != 0)
        {
            foreach (var pair in _localProvider.GetProperties(artifactToolId))
            {
                yield return new ConfigProperty(ConfigScope.Local, pair.Key, pair.Value);
            }
        }
    }

    public bool TryGetProperty(ArtifactToolID artifactToolId, string key, ConfigScopeFlags configScopeFlags, out ConfigProperty configProperty)
    {
        if ((configScopeFlags & ConfigScopeFlags.Local) != 0)
        {
            if (_localProvider.TryGetProperty(artifactToolId, key, out var subValue))
            {
                configProperty = new ConfigProperty(ConfigScope.Local, key, subValue);
                return true;
            }
        }
        if ((configScopeFlags & ConfigScopeFlags.Global) != 0)
        {
            if (_globalProvider.TryGetProperty(artifactToolId, key, out var subValue))
            {
                configProperty = new ConfigProperty(ConfigScope.Global, key, subValue);
                return true;
            }
        }
        configProperty = default;
        return false;
    }

    public bool TryGetProperty(ArtifactToolID artifactToolId, string key, out JsonElement value)
    {
        if (TryGetProperty(artifactToolId, key, ConfigScopeFlags.All, out ConfigProperty configProperty))
        {
            value = configProperty.Value;
            return true;
        }
        value = default;
        return false;
    }

    public bool TrySetProperty(ArtifactToolID artifactToolId, ConfigProperty configProperty)
    {
        // TODO implement
        throw new NotImplementedException();
    }
}