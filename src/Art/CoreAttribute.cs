namespace Art;

/// <summary>
/// Indicates that this type is a core <see cref="IArtifactTool"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class CoreAttribute : Attribute
{
}
