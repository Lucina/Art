namespace Art;

/// <summary>
/// Represents a stream that will be committed upon disposal if <see cref="ShouldCommit"/> is set.
/// </summary>
public abstract class CommittableStream : Stream
{
    /// <summary>
    /// If true, commit this stream upon disposal.
    /// </summary>
    public bool ShouldCommit { get; set; }
}
