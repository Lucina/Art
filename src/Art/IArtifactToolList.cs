namespace Art;

/// <summary>
/// Represents an artifact tool that lists.
/// </summary>
public interface IArtifactToolList
{
    /// <summary>
    /// Lists artifacts.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Async-enumerable artifacts.</returns>
    IAsyncEnumerable<ArtifactData> ListAsync(CancellationToken cancellationToken = default);
}
