using System.Diagnostics.CodeAnalysis;

namespace Art.Common;

/// <summary>
/// Represents a provider for loading <see cref="IArtifactToolRegistry"/> modules.
/// </summary>
public interface IModuleProvider
{
    /// <summary>
    /// Attempts to locate a module.
    /// </summary>
    /// <param name="assembly">Assembly simple name.</param>
    /// <param name="moduleLocation">Module location, if successful.</param>
    /// <returns>True if successful.</returns>
    bool TryLocateModule(string assembly, [NotNullWhen(true)] out IModuleLocation? moduleLocation);

    /// <summary>
    /// Loads a module.
    /// </summary>
    /// <param name="moduleLocation">Module location.</param>
    /// <returns>True if successful.</returns>
    /// <exception cref="ArgumentException">Thrown for invalid type.</exception>
    IArtifactToolRegistry LoadModule(IModuleLocation moduleLocation);

    /// <summary>
    /// Gets all module locations.
    /// </summary>
    /// <param name="dictionary">Dictionary to populate.</param>
    void LoadModuleLocations(IDictionary<string, IModuleLocation> dictionary);
}
