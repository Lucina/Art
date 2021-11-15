namespace Art;

/// <summary>
/// Represents an exception thrown when a <see cref="ArtifactDumperFactory"/> could not be found.
/// </summary>
public class ArtifactDumperFactoryNotFoundException : Exception
{
    /// <summary>
    /// Target dumper factory.
    /// </summary>
    public string Dumper { get; set; }


    private string? _message;

    /// <summary>
    /// Creates a new instance of <see cref="ArtifactDumperFactoryNotFoundException"/>.
    /// </summary>
    /// <param name="dumper">Target dumper factory.</param>
    public ArtifactDumperFactoryNotFoundException(string dumper) => (Dumper) = (dumper);

    /// <inheritdoc/>
    public override string Message => _message ??= $"Failed to find dumper factory {Dumper}";
}
