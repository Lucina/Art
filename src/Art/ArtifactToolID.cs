namespace Art;

/// <summary>
/// Represents identity of artifact tool.
/// </summary>
/// <param name="Assembly">Assembly.</param>
/// <param name="Type">Type.</param>
public record ArtifactToolID(string Assembly, string Type)
{
    /// <summary>
    /// Tool string equivalent for this instance.
    /// </summary>
    public string GetToolString() => $"{Assembly}::{Type}";
}
