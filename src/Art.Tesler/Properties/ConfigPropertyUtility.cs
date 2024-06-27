using System.Text.Json;

namespace Art.Tesler.Properties;

public static class ConfigPropertyUtility
{
    public static string FormatPropertyKeyForDisplay(ConfigScope configScope, string key)
    {
        return $"{configScope}::{key}";
    }

    public static string FormatPropertyForDisplay(string key, JsonElement value)
    {
        return $"{key}={FormatPropertyValueForDisplay(value)}";
    }

    public static string FormatPropertyForDisplay(ConfigProperty configProperty)
    {
        return $"{configProperty.ConfigScope}::{configProperty.Key}={FormatPropertyValueForDisplay(configProperty.Value)}";
    }

    public static string FormatPropertyForDisplay(ArtifactToolID artifactToolId, ConfigProperty configProperty)
    {
        return $"{artifactToolId}::{configProperty.ConfigScope}::{configProperty.Key}={FormatPropertyValueForDisplay(configProperty.Value)}";
    }

    public static string FormatPropertyForDisplay(int profileIndex, string profileGroup, ArtifactToolID artifactToolId, ConfigProperty configProperty)
    {
        return $"{profileIndex}::{profileGroup}::{artifactToolId}::{configProperty.ConfigScope}::{configProperty.Key}={FormatPropertyValueForDisplay(configProperty.Value)}";
    }

    public static string FormatPropertyValueForDisplay(JsonElement value)
    {
        return JsonSerializer.Serialize(value, SourceGenerationContext.s_context.JsonElement);
    }
}
