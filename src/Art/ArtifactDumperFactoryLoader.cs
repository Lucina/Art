using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.RegularExpressions;

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
    public static bool TryLoad(ArtifactDumpingProfile dumpingProfile, [NotNullWhen(true)] out ArtifactDumperFactory? factory) => TryLoad(dumpingProfile.Dumper, out factory);

    /// <summary>
    /// Attempts to load artifact dumper factory from an assembly name and factory type name.
    /// </summary>
    /// <param name="assemblyName">Artifact dumper factory assembly name.</param>
    /// <param name="factoryTypeName">Artifact dumper factory type name.</param>
    /// <param name="factory">Factory.</param>
    /// <returns>True if successfully located and created a dumper.</returns>
    public static bool TryLoad(string assemblyName, string factoryTypeName, [NotNullWhen(true)] out ArtifactDumperFactory? factory)
    {
        Assembly? assembly = Assembly.Load(assemblyName);
        if (assembly == null) goto fail;
        Type? type = assembly.GetType(factoryTypeName);
        object? obj = type == null ? null : Activator.CreateInstance(type);
        factory = obj is ArtifactDumperFactory adf ? adf : null;
        return factory != null;
    fail:
        factory = null;
        return false;
    }

    private static readonly Regex _dumperRegex = new Regex(@"^([\S\s]+)::([\S\s]+)$");
    /// <summary>
    /// Attempts to load artifact dumper factory from an artifact dumper target string (assembly::factoryType).
    /// </summary>
    /// <param name="dumper">Artifact dumper target string (assembly::factoryType).</param>
    /// <param name="factory">Factory.</param>
    /// <returns>True if successfully located and created a dumper.</returns>
    public static bool TryLoad(string dumper, [NotNullWhen(true)] out ArtifactDumperFactory? factory)
    {
        if (_dumperRegex.Match(dumper) is not { Success: true } match)
            throw new ArgumentException("Dumper string is in invalid format, must be \"<assembly>::<factoryType>\"", nameof(dumper));
        return TryLoad(match.Groups[1].Value, match.Groups[2].Value, out factory);
    }
}
