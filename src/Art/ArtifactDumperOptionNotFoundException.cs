using System.Text;

namespace Art;

/// <summary>
/// Represents an exception thrown when an option needed by a <see cref="ArtifactDumper"/> is not found.
/// </summary>
public class ArtifactDumperOptionNotFoundException : Exception
{
    /// <summary>
    /// Missing options.
    /// </summary>
    public IReadOnlyList<string> Options { get; }

    private string? _message;

    /// <summary>
    /// Creates a new instance of <see cref="ArtifactDumperOptionNotFoundException"/>.
    /// </summary>
    /// <param name="options">Missing options.</param>
    public ArtifactDumperOptionNotFoundException(params string[] options)
    {
        Options = options;
    }

    private string GetMessageString()
    {
        StringBuilder sb = new();
        if (Options.Count == 1)
            sb.Append("Configuration was missing required option ");
        else
            sb.Append("Configuration was missing required options ");
        sb.AppendJoin(", ", Options);
        return sb.ToString();
    }

    /// <inheritdoc/>
    public override string Message => _message ??= GetMessageString();
}
