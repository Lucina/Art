using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Art.Common;

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
    /// <param name="artifactToolId">Artifact tool ID.</param>
    /// <param name="tool">Tool.</param>
    /// <returns>True if successfully located and created a tool.</returns>
    public static bool TryLoad(ArtifactToolID artifactToolId, [NotNullWhen(true)] out ArtifactToolBase? tool)
    {
        try
        {
            Assembly assembly = Assembly.Load(artifactToolId.Assembly);
            Type? type = assembly.GetType(artifactToolId.Type);
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
        return TryLoad(ArtifactToolProfileUtil.GetID(toolId), out tool);
    }
}
