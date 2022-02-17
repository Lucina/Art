namespace Art;

/// <summary>
/// Represents a wrapper around a <see cref="SinkStream"/>.
/// </summary>
public class CommittableSinkStream : CommittableMemoryStream
{
    /// <summary>
    /// Creates a new instance of <see cref="CommittableSinkStream"/>.
    /// </summary>
    public CommittableSinkStream() : base(new SinkStream())
    {
    }
}
