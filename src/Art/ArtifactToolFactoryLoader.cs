using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Art;

/// <summary>
/// Provides loading facility for artifact tool factories via reflection.
/// </summary>
public static class ArtifactToolFactoryLoader
{
    /// <summary>
    /// Attempts to load artifact tool factory from a profile.
    /// </summary>
    /// <param name="artifactToolProfile">Artifact tool profile.</param>
    /// <param name="factory">Factory.</param>
    /// <returns>True if successfully located and created a tool.</returns>
    public static bool TryLoad(ArtifactToolProfile artifactToolProfile, [NotNullWhen(true)] out ArtifactToolFactory? factory) => TryLoad(artifactToolProfile.Tool, out factory);

    /// <summary>
    /// Attempts to load artifact tool factory from an assembly name and factory type name.
    /// </summary>
    /// <param name="assemblyName">Artifact tool factory assembly name.</param>
    /// <param name="factoryTypeName">Artifact tool factory type name.</param>
    /// <param name="factory">Factory.</param>
    /// <returns>True if successfully located and created a tool.</returns>
    public static bool TryLoad(string assemblyName, string factoryTypeName, [NotNullWhen(true)] out ArtifactToolFactory? factory)
    {
        Assembly? assembly = Assembly.Load(assemblyName);
        if (assembly == null) goto fail;
        Type? type = assembly.GetType(factoryTypeName);
        object? obj = type == null ? null : Activator.CreateInstance(type);
        factory = obj is ArtifactToolFactory adf ? adf : null;
        return factory != null;
    fail:
        factory = null;
        return false;
    }

    private static readonly Regex s_toolRegex = new(@"^([\S\s]+)::([\S\s]+)$");
    /// <summary>
    /// Attempts to load artifact tool factory from an artifact tool target string (assembly::factoryType).
    /// </summary>
    /// <param name="tool">Artifact tool target string (assembly::factoryType).</param>
    /// <param name="factory">Factory.</param>
    /// <returns>True if successfully located and created a tool.</returns>
    public static bool TryLoad(string tool, [NotNullWhen(true)] out ArtifactToolFactory? factory)
    {
        if (s_toolRegex.Match(tool) is not { Success: true } match)
            throw new ArgumentException("Tool string is in invalid format, must be \"<assembly>::<factoryType>\"", nameof(tool));
        return TryLoad(match.Groups[1].Value, match.Groups[2].Value, out factory);
    }
}
