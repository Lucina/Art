namespace Art;

/// <summary>
/// Represents an exception thrown when a <see cref="ArtifactToolFactory"/> could not be found.
/// </summary>
public class ArtifactToolFactoryNotFoundException : Exception
{
    /// <summary>
    /// Target tool factory.
    /// </summary>
    public string Tool { get; set; }


    private string? _message;

    /// <summary>
    /// Creates a new instance of <see cref="ArtifactToolFactoryNotFoundException"/>.
    /// </summary>
    /// <param name="tool">Target tool factory.</param>
    public ArtifactToolFactoryNotFoundException(string tool) => Tool = tool;

    /// <inheritdoc/>
    public override string Message => _message ??= $"Failed to find tool factory {Tool}";
}
