using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Art;

/// <summary>
/// Provides loading facility for artifact tools via reflection.
/// </summary>
public static class ArtifactToolLoader
{
    /// <summary>
    /// Attempts to load artifact tool from a profile.
    /// </summary>
    /// <param name="artifactToolProfile">Artifact tool profile.</param>
    /// <param name="tool">Tool.</param>
    /// <returns>True if successfully located and created a tool.</returns>
    public static bool TryLoad(ArtifactToolProfile artifactToolProfile, [NotNullWhen(true)] out ArtifactToolBase? tool) => TryLoad(artifactToolProfile.Tool, out tool);

    /// <summary>
    /// Attempts to load artifact tool from an assembly name and tool type name.
    /// </summary>
    /// <param name="assemblyName">Artifact tool assembly name.</param>
    /// <param name="toolTypeName">Artifact tool type name.</param>
    /// <param name="tool">Tool.</param>
    /// <returns>True if successfully located and created a tool.</returns>
    public static bool TryLoad(string assemblyName, string toolTypeName, [NotNullWhen(true)] out ArtifactToolBase? tool)
    {
        try
        {
            Assembly assembly = Assembly.Load(assemblyName);
            Type? type = assembly.GetType(toolTypeName);
            object? obj = type == null ? null : Activator.CreateInstance(type);
            tool = obj is ArtifactToolBase at ? at : null;
            return tool != null;
        }
        catch
        {
            tool = null;
            return false;
        }
    }

    /// <summary>
    /// Attempts to load artifact tool from an artifact tool target string (assembly::toolType).
    /// </summary>
    /// <param name="toolId">Artifact tool target string (assembly::toolType).</param>
    /// <param name="tool">Tool.</param>
    /// <returns>True if successfully located and created a tool.</returns>
    public static bool TryLoad(string toolId, [NotNullWhen(true)] out ArtifactToolBase? tool)
    {
        (string assembly, string type) = ArtifactToolProfile.GetId(toolId);
        return TryLoad(assembly, type, out tool);
    }
}
