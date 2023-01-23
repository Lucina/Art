using System.Reflection;

namespace Art;

/// <summary>
/// Utility for creating tool strings.
/// </summary>
public static class ArtifactToolStringUtil
{
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
    /// Creates a tool string for the specified tool.
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
    /// Creates a tool string for the specified tool.
    /// </summary>
    /// <typeparam name="TTool">Tool type.</typeparam>
    /// <returns>Tool string.</returns>
    public static string CreateCoreToolString<TTool>() where TTool : IArtifactTool
        =>
            CreateCoreToolString(typeof(TTool));

    /// <summary>
    /// Creates a tool string for the specified tool.
    /// </summary>
    /// <typeparam name="TTool">Tool type.</typeparam>
    /// <returns>Tool string.</returns>
    public static string CreateToolString<TTool>() where TTool : IArtifactTool
        =>
            CreateToolString(typeof(TTool));
}
