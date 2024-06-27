using System.Text.Json;
using Art.Common;

namespace Art.Tesler.Properties;

public static class TeslerPropertyUtility
{
    public static void ApplyProperties(
        IRunnerPropertyProvider runnerDefaultPropertyProvider,
        IDictionary<string, JsonElement> dictionary)
    {
        foreach (var pair in runnerDefaultPropertyProvider.GetProperties())
        {
            dictionary[pair.Key] = pair.Value;
        }
    }

    public static void ApplyProperties(
        IToolPropertyProvider toolPropertyProvider,
        IDictionary<string, JsonElement> dictionary,
        ArtifactToolID artifactToolId)
    {
        foreach (var pair in toolPropertyProvider.GetProperties(artifactToolId))
        {
            dictionary[pair.Key] = pair.Value;
        }
    }

    public static void ApplyPropertiesDeep(
        IToolPropertyProvider toolPropertyProvider,
        IDictionary<string, JsonElement> dictionary,
        Type type
    )
    {
        if (type.BaseType is { } baseType)
        {
            ApplyPropertiesDeep(toolPropertyProvider, dictionary, baseType);
        }
        ApplyProperties(toolPropertyProvider, dictionary, ArtifactToolIDUtil.CreateToolID(type));
    }

    public static IEnumerable<ConfigProperty> GetPropertiesDeep(
        IScopedToolPropertyProvider toolPropertyProvider,
        Type type,
        ConfigScopeFlags configScopeFlags
    )
    {
        IEnumerable<ConfigProperty> enumerable = toolPropertyProvider.GetProperties(ArtifactToolIDUtil.CreateCoreToolID(type), configScopeFlags);
        if (type.BaseType is { } baseType)
        {
            return GetPropertiesDeep(toolPropertyProvider, baseType, configScopeFlags).Concat(enumerable);
        }
        return enumerable;
    }

    public static void ApplyPropertiesDeep(
        IArtifactToolRegistryStore registryStore,
        IToolPropertyProvider toolPropertyProvider,
        IOutputControl? console,
        Dictionary<string, JsonElement> opts,
        ArtifactToolID artifactToolId)
    {
        if (registryStore.TryLoadRegistry(artifactToolId, out var registry))
        {
            if (registry.TryGetType(artifactToolId, out var type))
            {
                ApplyPropertiesDeep(toolPropertyProvider, opts, type);
            }
            else
            {
                console?.Warn.WriteLine($"Warning: tool type {artifactToolId} could not be found in the registry it should be stored in, configuration will not contain values inherited from base types");
                ApplyProperties(toolPropertyProvider, opts, artifactToolId);
            }
        }
        else
        {
            console?.Warn.WriteLine($"Warning: tool type {artifactToolId} could not be found, configuration will not contain values inherited from base types");
            ApplyProperties(toolPropertyProvider, opts, artifactToolId);
        }
    }

    public static IEnumerable<ConfigProperty> GetPropertiesDeep(
        IArtifactToolRegistryStore registryStore,
        IScopedToolPropertyProvider toolPropertyProvider,
        IOutputControl? console,
        ArtifactToolID artifactToolId,
        ConfigScopeFlags configScopeFlags)
    {
        if (registryStore.TryLoadRegistry(artifactToolId, out var registry))
        {
            if (registry.TryGetType(artifactToolId, out var type))
            {
                return GetPropertiesDeep(toolPropertyProvider, type, configScopeFlags);
            }
            else
            {
                console?.Warn.WriteLine($"Warning: tool type {artifactToolId} could not be found in the registry it should be stored in, configuration will not contain values inherited from base types");
                return toolPropertyProvider.GetProperties(artifactToolId, configScopeFlags);
            }
        }
        else
        {
            console?.Warn.WriteLine($"Warning: tool type {artifactToolId} could not be found, configuration will not contain values inherited from base types");
            return toolPropertyProvider.GetProperties(artifactToolId, configScopeFlags);
        }
    }
}