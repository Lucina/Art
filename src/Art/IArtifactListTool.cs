namespace Art;

/// <summary>
/// Represents an artifact tool that lists.
/// </summary>
public interface IArtifactListTool : IArtifactTool
{
    /// <summary>
    /// Lists artifacts.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Async-enumerable artifacts.</returns>
    IAsyncEnumerable<IArtifactData> ListAsync(CancellationToken cancellationToken = default);
}
