namespace Art;

/// <summary>
/// Represents an artifact data manager that does not preserve data.
/// </summary>
public class NullArtifactDataManager : ArtifactDataManager
{
    /// <summary>
    /// Shared instance.
    /// </summary>
    public static readonly NullArtifactDataManager Instance = new();

    /// <inheritdoc />
    public override ValueTask<CommittableStream> CreateOutputStreamAsync(ArtifactResourceKey key, OutputStreamOptions? options = null, CancellationToken cancellationToken = default) => new(new CommittableSinkStream());

    /// <inheritdoc />
    public override ValueTask<bool> ExistsAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default) => new(false);

    /// <inheritdoc />
    public override ValueTask<bool> DeleteAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default) => new(true);

    /// <inheritdoc />
    public override ValueTask<Stream> OpenInputStreamAsync(ArtifactResourceKey key, CancellationToken cancellationToken = default) => throw new IOException("Specified resource was not found.");
}
