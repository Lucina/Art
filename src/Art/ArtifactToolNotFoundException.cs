namespace Art;

/// <summary>
/// Represents an exception thrown when a <see cref="ArtifactToolBase"/> could not be found.
/// </summary>
public class ArtifactToolNotFoundException : Exception
{
    /// <summary>
    /// Target tool.
    /// </summary>
    public string Tool { get; set; }


    private string? _message;

    /// <summary>
    /// Creates a new instance of <see cref="ArtifactToolNotFoundException"/>.
    /// </summary>
    /// <param name="tool">Target tool.</param>
    public ArtifactToolNotFoundException(string tool) => Tool = tool;

    /// <inheritdoc/>
    public override string Message => _message ??= $"Failed to find tool {Tool}";
}
