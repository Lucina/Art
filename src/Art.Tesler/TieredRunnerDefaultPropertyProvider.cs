using System.Text.Json;

namespace Art.Tesler;

public class TieredRunnerDefaultPropertyProvider : IRunnerDefaultPropertyProvider
{
    private readonly IRunnerDefaultPropertyProvider[] _propertyProviders;

    /// <summary>
    /// Initializes an instance of <see cref="TieredRunnerDefaultPropertyProvider"/>.
    /// </summary>
    /// <param name="propertyProviders">Sequence of underlying providers to use (later elements have precedence)</param>
    public TieredRunnerDefaultPropertyProvider(IEnumerable<IRunnerDefaultPropertyProvider> propertyProviders)
    {
        _propertyProviders = propertyProviders.ToArray();
    }

    public IEnumerable<KeyValuePair<string, JsonElement>> EnumerateDefaultProperties()
    {
        return _propertyProviders.SelectMany(static s => s.EnumerateDefaultProperties());
    }
}