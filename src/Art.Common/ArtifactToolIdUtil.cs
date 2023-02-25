using System.Reflection;

namespace Art.Common;

/// <summary>
/// Utility for creating tool strings.
/// </summary>
public static class ArtifactToolIDUtil
{
    /// <summary>
    /// Creates a tool ID for the specified tool.
    /// </summary>
    /// <param name="type">Tool type.</param>
    /// <returns>Tool ID.</returns>
    public static ArtifactToolID CreateToolId(Type type)
    {
        string assemblyName = type.Assembly.GetName().Name ?? throw new InvalidOperationException();
        string typeName = type.FullName ?? throw new InvalidOperationException();
        return new ArtifactToolID(assemblyName, typeName);
    }

    /// <summary>
    /// Creates a tool string for the specified tool.
    /// </summary>
    /// <param name="type">Tool type.</param>
    /// <returns>Tool string.</returns>
    public static string CreateToolString(Type type)
    {
        string assemblyName = type.Assembly.GetName().Name ?? throw new InvalidOperationException();
        string typeName = type.FullName ?? throw new InvalidOperationException();
        return $"{assemblyName}::{typeName}";
    }

    /// <summary>
    /// Creates a core tool ID for the specified tool.
    /// </summary>
    /// <param name="type">Tool type.</param>
    /// <returns>Tool ID.</returns>
    public static ArtifactToolID CreateCoreToolId(Type type)
    {
        Type? coreType = type;
        while (coreType != null && coreType.GetCustomAttribute<CoreAttribute>() == null) coreType = coreType.BaseType;
        return CreateToolId(coreType ?? type);
    }

    /// <summary>
    /// Creates a core tool string for the specified tool.
    /// </summary>
    /// <param name="type">Tool type.</param>
    /// <returns>Tool string.</returns>
    public static string CreateCoreToolString(Type type)
    {
        Type? coreType = type;
        while (coreType != null && coreType.GetCustomAttribute<CoreAttribute>() == null) coreType = coreType.BaseType;
        return CreateToolString(coreType ?? type);
    }

    /// <summary>
    /// Creates a tool ID for the specified tool.
    /// </summary>
    /// <typeparam name="TTool">Tool type.</typeparam>
    /// <returns>Tool ID.</returns>
    public static ArtifactToolID CreateCoreToolId<TTool>() where TTool : IArtifactTool
    {
        return CreateCoreToolId(typeof(TTool));
    }

    /// <summary>
    /// Creates a tool string for the specified tool.
    /// </summary>
    /// <typeparam name="TTool">Tool type.</typeparam>
    /// <returns>Tool string.</returns>
    public static string CreateCoreToolString<TTool>() where TTool : IArtifactTool
    {
        return CreateCoreToolString(typeof(TTool));
    }

    /// <summary>
    /// Creates a tool ID for the specified tool.
    /// </summary>
    /// <typeparam name="TTool">Tool type.</typeparam>
    /// <returns>Tool ID.</returns>
    public static ArtifactToolID CreateToolId<TTool>() where TTool : IArtifactTool
    {
        return CreateToolId(typeof(TTool));
    }

    /// <summary>
    /// Creates a tool string for the specified tool.
    /// </summary>
    /// <typeparam name="TTool">Tool type.</typeparam>
    /// <returns>Tool string.</returns>
    public static string CreateToolString<TTool>() where TTool : IArtifactTool
    {
        return CreateToolString(typeof(TTool));
    }
}
