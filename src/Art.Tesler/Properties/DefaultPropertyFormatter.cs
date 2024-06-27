using System.Text.Json;

namespace Art.Tesler.Properties;

public class DefaultPropertyFormatter : PropertyFormatter
{
    public static readonly DefaultPropertyFormatter Instance = new();

    public override string FormatProperty(ConfigProperty configProperty)
    {
        return ConfigPropertyUtility.FormatPropertyForDisplay(configProperty);
    }

    public override string FormatProperty(ArtifactToolID artifactToolId, ConfigProperty configProperty)
    {
        return ConfigPropertyUtility.FormatPropertyForDisplay(configProperty);
    }

    public override string FormatProperty(int profileIndex, ArtifactToolProfile artifactToolProfile, ArtifactToolID artifactToolId, ConfigProperty configProperty)
    {
        return ConfigPropertyUtility.FormatPropertyForDisplay(profileIndex, artifactToolProfile.Group ?? "<unspecified>", artifactToolId, configProperty);
    }

    public override string FormatPropertyValue(JsonElement propertyValue)
    {
        return ConfigPropertyUtility.FormatPropertyValueForDisplay(propertyValue);
    }
}
