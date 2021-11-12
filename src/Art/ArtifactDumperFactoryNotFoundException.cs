namespace Art;

/// <summary>
/// Represents an exception thrown when a <see cref="ArtifactDumperFactory"/> could not be found.
/// </summary>
public class ArtifactDumperFactoryNotFoundException : Exception
{
    /// <summary>
    /// Target factory assembly name.
    /// </summary>
    public string AssemblyName { get; set; }

    /// <summary>
    /// Target factory type name.
    /// </summary>
    public string FactoryTypeName { get; set; }

    private string? _message;

    /// <summary>
    /// Creates a new instance of <see cref="ArtifactDumperFactoryNotFoundException"/>.
    /// </summary>
    /// <param name="assemblyName">Target factory assembly name.</param>
    /// <param name="factoryTypeName">Target factory type name.</param>
    public ArtifactDumperFactoryNotFoundException(string assemblyName, string factoryTypeName) => (AssemblyName, FactoryTypeName) = (assemblyName, factoryTypeName);

    /// <inheritdoc/>
    public override string Message => _message ??= $"Failed to find dumper factory {AssemblyName}::{FactoryTypeName}";
}
