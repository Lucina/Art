using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.Loader;

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
    public static bool TryLoad(ArtifactToolProfile artifactToolProfile, [NotNullWhen(true)] out IArtifactTool? tool)
    {
        return TryLoad(artifactToolProfile.Tool, out tool);
    }

    /// <summary>
    /// Attempts to load artifact tool from a profile.
    /// </summary>
    /// <param name="assemblyLoadContext">Custom <see cref="AssemblyLoadContext"/>.</param>
    /// <param name="artifactToolProfile">Artifact tool profile.</param>
    /// <param name="tool">Tool.</param>
    /// <returns>True if successfully located and created a tool.</returns>
    public static bool TryLoad(AssemblyLoadContext assemblyLoadContext, ArtifactToolProfile artifactToolProfile, [NotNullWhen(true)] out IArtifactTool? tool)
    {
        return TryLoad(assemblyLoadContext, artifactToolProfile.Tool, out tool);
    }

    /// <summary>
    /// Attempts to load artifact tool from an artifact tool target string (assembly::toolType).
    /// </summary>
    /// <param name="toolId">Artifact tool target string (assembly::toolType).</param>
    /// <param name="tool">Tool.</param>
    /// <returns>True if successfully located and created a tool.</returns>
    public static bool TryLoad(string toolId, [NotNullWhen(true)] out IArtifactTool? tool)
    {
        return TryLoad(ArtifactToolProfileUtil.GetID(toolId), out tool);
    }

    /// <summary>
    /// Attempts to load artifact tool from an artifact tool target string (assembly::toolType).
    /// </summary>
    /// <param name="assemblyLoadContext">Custom <see cref="AssemblyLoadContext"/>.</param>
    /// <param name="toolId">Artifact tool target string (assembly::toolType).</param>
    /// <param name="tool">Tool.</param>
    /// <returns>True if successfully located and created a tool.</returns>
    public static bool TryLoad(AssemblyLoadContext assemblyLoadContext, string toolId, [NotNullWhen(true)] out IArtifactTool? tool)
    {
        return TryLoad(assemblyLoadContext, ArtifactToolProfileUtil.GetID(toolId), out tool);
    }

    /// <summary>
    /// Attempts to load artifact tool from an assembly name and tool type name.
    /// </summary>
    /// <param name="artifactToolId">Artifact tool ID.</param>
    /// <param name="tool">Tool.</param>
    /// <returns>True if successfully located and created a tool.</returns>
    public static bool TryLoad(ArtifactToolID artifactToolId, [NotNullWhen(true)] out IArtifactTool? tool)
    {
        try
        {
            Assembly assembly = Assembly.Load(artifactToolId.Assembly);
            return TryLoad(assembly, artifactToolId, out tool);
        }
        catch
        {
            tool = null;
            return false;
        }
    }

    /// <summary>
    /// Attempts to load artifact tool from an assembly name and tool type name.
    /// </summary>
    /// <param name="assemblyLoadContext">Custom <see cref="AssemblyLoadContext"/>.</param>
    /// <param name="artifactToolId">Artifact tool ID.</param>
    /// <param name="tool">Tool.</param>
    /// <returns>True if successfully located and created a tool.</returns>
    public static bool TryLoad(AssemblyLoadContext assemblyLoadContext, ArtifactToolID artifactToolId, [NotNullWhen(true)] out IArtifactTool? tool)
    {
        try
        {
            Assembly assembly = assemblyLoadContext.LoadFromAssemblyName(new AssemblyName(artifactToolId.Assembly));
            return TryLoad(assembly, artifactToolId, out tool);
        }
        catch
        {
            tool = null;
            return false;
        }
    }

    /// <summary>
    /// Attempts to load artifact tool from an assembly and tool type name.
    /// </summary>
    /// <param name="assembly">Assembly.</param>
    /// <param name="artifactToolId">Artifact tool ID.</param>
    /// <param name="tool">Tool.</param>
    /// <returns>True if successfully located and created a tool.</returns>
    public static bool TryLoad(Assembly assembly, ArtifactToolID artifactToolId, [NotNullWhen(true)] out IArtifactTool? tool)
    {
        try
        {
            Type? type = assembly.GetType(artifactToolId.Type);
            object? obj = type == null ? null : Activator.CreateInstance(type);
            tool = obj as IArtifactTool;
            return tool != null;
        }
        catch
        {
            tool = null;
            return false;
        }
    }

    /// <summary>
    /// Attempts to load artifact tool from an assembly and artifact tool target string (assembly::toolType).
    /// </summary>
    /// <param name="assembly">Assembly.</param>
    /// <param name="toolId">Artifact tool target string (assembly::toolType).</param>
    /// <param name="tool">Tool.</param>
    /// <returns>True if successfully located and created a tool.</returns>
    public static bool TryLoad(Assembly assembly, string toolId, [NotNullWhen(true)] out IArtifactTool? tool)
    {
        return TryLoad(assembly, ArtifactToolProfileUtil.GetID(toolId), out tool);
    }
}
