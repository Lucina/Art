using System.Text.Json;

namespace Art.Tesler;

public class TieredToolDefaultPropertyProvider : IToolDefaultPropertyProvider
{
    private readonly IToolDefaultPropertyProvider[] _propertyProviders;

    /// <summary>
    /// Initializes an instance of <see cref="TieredToolDefaultPropertyProvider"/>.
    /// </summary>
    /// <param name="propertyProviders">Sequence of underlying providers to use (later elements have precedence)</param>
    public TieredToolDefaultPropertyProvider(IEnumerable<IToolDefaultPropertyProvider> propertyProviders)
    {
        _propertyProviders = propertyProviders.ToArray();
    }

    public IEnumerable<KeyValuePair<string, JsonElement>> EnumerateDefaultProperties(ArtifactToolID artifactToolId)
    {
        return _propertyProviders.SelectMany(v => v.EnumerateDefaultProperties(artifactToolId));
    }
}