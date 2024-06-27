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
        return $"{key}={value}";
    }

    public static string FormatPropertyForDisplay(ConfigProperty property)
    {
        return $"{property.ConfigScope}::{property.Key}={property.Value}";
    }

    public static string FormatPropertyForDisplay(ArtifactToolID artifactToolId, ConfigProperty property)
    {
        return $"{artifactToolId}::{property.ConfigScope}::{property.Key}={property.Value}";
    }
}
