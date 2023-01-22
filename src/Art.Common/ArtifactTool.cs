using System.Text.Json;

namespace Art.Common;

/// <summary>
/// Common type for artifact tools.
/// </summary>
public partial class ArtifactTool : ArtifactToolBase
{
    private static readonly HashSet<string> s_yesLower = new() { "y", "yes", "" };

    private JsonSerializerOptions? _jsonOptions;
    private bool _delayFirstCalled;

    /// <summary>
    /// JSON serialization defaults.
    /// </summary>
    public JsonSerializerOptions JsonOptions
    {
        get => _jsonOptions ??= new JsonSerializerOptions();
        set => _jsonOptions = value;
    }

    /// <summary>
    /// True if this tool is in debug mode.
    /// </summary>
    public bool DebugMode;

    /// <summary>
    /// Default delay time in seconds.
    /// </summary>
    public virtual double DelaySeconds => 0.25;
}
