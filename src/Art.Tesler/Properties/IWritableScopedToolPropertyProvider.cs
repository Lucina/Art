namespace Art.Tesler.Properties;

public interface IWritableScopedToolPropertyProvider : IScopedToolPropertyProvider
{
    bool TrySetProperty(ArtifactToolID artifactToolId, ConfigProperty configProperty);
}
