using System.Text.Json;

namespace Art.Tesler.Properties;

public class SimplePropertyFormatter : PropertyFormatter
{
    public static readonly SimplePropertyFormatter Instance = new();

    public override string FormatProperty(ConfigProperty configProperty)
    {
        return ConfigPropertyUtility.FormatPropertyForDisplay(configProperty.Key, configProperty.Value);
    }

    public override string FormatProperty(ArtifactToolID artifactToolId, ConfigProperty configProperty)
    {
        return ConfigPropertyUtility.FormatPropertyForDisplay(configProperty.Key, configProperty.Value);
    }

    public override string FormatProperty(int profileIndex, ArtifactToolProfile artifactToolProfile, ArtifactToolID artifactToolId, ConfigProperty configProperty)
    {
        return ConfigPropertyUtility.FormatPropertyForDisplay(configProperty.Key, configProperty.Value);
    }

    public override string FormatPropertyValue(JsonElement propertyValue)
    {
        return ConfigPropertyUtility.FormatPropertyValueForDisplay(propertyValue);
    }
}
