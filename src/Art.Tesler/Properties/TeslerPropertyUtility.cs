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
}