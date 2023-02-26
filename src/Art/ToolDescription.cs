namespace Art.Common;

/// <summary>
/// Representation of a tool's metadata.
/// </summary>
/// <param name="Type">Tool type.</param>
/// <param name="Id">Tool ID.</param>
public readonly record struct ToolDescription(Type Type, ArtifactToolID Id);
