namespace Art.Tesler;

public interface IGlobalLocalRunnerPropertyProvider : IRunnerPropertyProvider
{
    IEnumerable<ConfigProperty> GetProperties(ConfigScopeFlags configScopeFlags);

    bool TryGetProperty(string key, ConfigScopeFlags configScopeFlags, out ConfigProperty configProperty);
}