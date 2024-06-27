namespace Art.Tesler.Properties;

public class PropertyValueFormatter : PropertyFormatter
{
    public static readonly PropertyValueFormatter Instance = new();

    public override string FormatProperty(ConfigProperty configProperty)
    {
        return ConfigPropertyUtility.FormatPropertyValueForDisplay(configProperty.Value);
    }

    public override string FormatProperty(ArtifactToolID artifactToolId, ConfigProperty configProperty)
    {
        return ConfigPropertyUtility.FormatPropertyValueForDisplay(configProperty.Value);
    }

    public override string FormatProperty(int profileIndex, ArtifactToolProfile artifactToolProfile, ArtifactToolID artifactToolId, ConfigProperty configProperty)
    {
        return ConfigPropertyUtility.FormatPropertyValueForDisplay(configProperty.Value);
    }
}
