namespace Art.Tesler.Config;

public static class ConfigCommandUtility
{
    public static string GetGroupName(ArtifactToolProfile artifactToolProfile)
    {
        return artifactToolProfile.Group ?? "<unspecified>";
    }
}
