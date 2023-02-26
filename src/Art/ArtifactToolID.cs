namespace Art;

/// <summary>
/// Represents identity of artifact tool.
/// </summary>
/// <param name="Assembly">Assembly.</param>
/// <param name="Type">Type.</param>
public readonly record struct ArtifactToolID(string Assembly, string Type)
{
    /// <summary>
    /// Gets a tool string from this instance.
    /// </summary>
    /// <returns>Tool string.</returns>
    public string GetToolString()
    {
        return $"{Assembly}::{Type}";
    }
}
