namespace Art;

/// <summary>
/// Represents a target for updates on an operation.
/// </summary>
public interface IOperationProgressContext : IProgress<float>, IDisposable
{
}
