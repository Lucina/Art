
using System.Text.Json;

namespace Art.Tesler;

public class GlobalLocalRunnerDefaultPropertyProvider : IRunnerDefaultPropertyProvider
{
    private readonly IRunnerDefaultPropertyProvider _globalProvider;
    private readonly IRunnerDefaultPropertyProvider _localProvider;

    public GlobalLocalRunnerDefaultPropertyProvider(
        IRunnerDefaultPropertyProvider globalProvider,
        IRunnerDefaultPropertyProvider localProvider)
    {
        _globalProvider = globalProvider;
        _localProvider = localProvider;
    }

    public IEnumerable<KeyValuePair<string, JsonElement>> EnumerateDefaultProperties()
    {
        return _globalProvider.EnumerateDefaultProperties().Concat(_localProvider.EnumerateDefaultProperties());
    }

    public IEnumerable<ConfigProperty> GetProperties(ConfigScopeFlags configScopeFlags)
    {
        if ((configScopeFlags & ConfigScopeFlags.Global) != 0)
        {
            foreach (var pair in _globalProvider.EnumerateDefaultProperties())
            {
                yield return new ConfigProperty(ConfigScope.Global, pair.Key, pair.Value);
            }
        }
        if ((configScopeFlags & ConfigScopeFlags.Local) != 0)
        {
            foreach (var pair in _localProvider.EnumerateDefaultProperties())
            {
                yield return new ConfigProperty(ConfigScope.Local, pair.Key, pair.Value);
            }
        }
    }

    public bool TryGetProperty(string key, ConfigScopeFlags configScopeFlags, out ConfigProperty configProperty)
    {
        if ((configScopeFlags & ConfigScopeFlags.Local) != 0)
        {
            if (_localProvider.TryGetDefaultProperty(key, out var subValue))
            {
                configProperty = new ConfigProperty(ConfigScope.Local, key, subValue);
                return true;
            }
        }
        if ((configScopeFlags & ConfigScopeFlags.Global) != 0)
        {
            if (_globalProvider.TryGetDefaultProperty(key, out var subValue))
            {
                configProperty = new ConfigProperty(ConfigScope.Global, key, subValue);
                return true;
            }
        }
        configProperty = default;
        return false;
    }

    public bool TryGetDefaultProperty(string key, out JsonElement value)
    {
        if (TryGetProperty(key, ConfigScopeFlags.All, out ConfigProperty configProperty))
        {
            value = configProperty.Value;
            return true;
        }
        value = default;
        return false;
    }
}