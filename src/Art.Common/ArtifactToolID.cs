namespace Art.Common;

/// <summary>
/// Represents identity of artifact tool.
/// </summary>
/// <param name="Assembly">Assembly.</param>
/// <param name="Type">Type.</param>
public readonly record struct ArtifactToolID(string Assembly, string Type);
