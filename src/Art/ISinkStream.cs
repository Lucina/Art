namespace Art;

/// <summary>
/// Tag interface for <see cref="Stream"/> types that are a no-op for output.
/// </summary>
/// <remarks>This should be kept in the shared assembly, since this could potentially be needed across plugin / host bounds.</remarks>
public interface ISinkStream
{
}
