using System.Text.Json;

namespace Art.Tesler.Properties;

public abstract class PropertyFormatter
{
    public abstract string FormatProperty(ConfigProperty configProperty);

    public abstract string FormatProperty(ArtifactToolID artifactToolId, ConfigProperty configProperty);

    public abstract string FormatProperty(int profileIndex, string profileGroup, ArtifactToolID artifactToolId, ConfigProperty configProperty);

    public abstract string FormatPropertyValue(JsonElement propertyValue);
}
