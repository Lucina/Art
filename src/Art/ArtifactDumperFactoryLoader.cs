using System.Diagnostics.CodeAnalysis;

namespace Art;

/// <summary>
/// Provides loading facility for artifact dumper factories via reflection.
/// </summary>
public static class ArtifactDumperFactoryLoader
{
    /// <summary>
    /// Attempts to load artifact dumper factory from a profile.
    /// </summary>
    /// <param name="dumpingProfile">Artifact dumping profile.</param>
    /// <param name="factory">Factory.</param>
    /// <returns>True if successfully located and created a dumper.</returns>
    public static bool TryLoad(ArtifactDumpingProfile dumpingProfile, [NotNullWhen(true)] out ArtifactDumperFactory? factory) => TryLoad(dumpingProfile.AssemblyName, dumpingProfile.FactoryTypeName, out factory);

    /// <summary>
    /// Attempts to load artifact dumper factory from an assembly name and factory type name.
    /// </summary>
    /// <param name="assemblyName">Artifact dumper factory assembly name.</param>
    /// <param name="factoryTypeName">Artifact dumper factory type name.</param>
    /// <param name="factory">Factory.</param>
    /// <returns>True if successfully located and created a dumper.</returns>
    public static bool TryLoad(string assemblyName, string factoryTypeName, [NotNullWhen(true)] out ArtifactDumperFactory? factory)
    {
        var assembly = AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(v => v.GetName().Name == assemblyName);
        if (assembly == null) goto fail;
        var type = assembly.GetType(factoryTypeName);
        var obj = type == null ? null : Activator.CreateInstance(type);
        factory = obj is ArtifactDumperFactory adf ? adf : null;
        return factory != null;
    fail:
        factory = null;
        return false;
    }
}
