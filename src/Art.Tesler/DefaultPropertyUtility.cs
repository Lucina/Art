using System.Text.Json;
using Art.Common;

namespace Art.Tesler;

public static class DefaultPropertyUtility
{
    public static void ApplyProperties(
        IRunnerDefaultPropertyProvider runnerDefaultPropertyProvider,
        IDictionary<string, JsonElement> dictionary)
    {
        foreach (var pair in runnerDefaultPropertyProvider.EnumerateDefaultProperties())
        {
            dictionary[pair.Key] = pair.Value;
        }
    }

    public static void ApplyProperties(
        IToolDefaultPropertyProvider toolDefaultPropertyProvider,
        IDictionary<string, JsonElement> dictionary,
        ArtifactToolID artifactToolId)
    {
        foreach (var pair in toolDefaultPropertyProvider.EnumerateDefaultProperties(artifactToolId))
        {
            dictionary[pair.Key] = pair.Value;
        }
    }

    public static void ApplyPropertiesDeep(
        IToolDefaultPropertyProvider toolDefaultPropertyProvider,
        IDictionary<string, JsonElement> dictionary,
        Type type
    )
    {
        Type? currentType = type;
        while (currentType != null)
        {
            ApplyProperties(toolDefaultPropertyProvider, dictionary, ArtifactToolIDUtil.CreateToolID(currentType));
            currentType = currentType.BaseType;
        }
    }
}