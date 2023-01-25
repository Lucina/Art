namespace Art.Http;

/// <summary>
/// Exception thrown when denied access.
/// </summary>
public class AccessDeniedException : ArtSystemException
{
    /// <summary>
    /// Creates a new instance of <see cref="AccessDeniedException"/>.
    /// </summary>
    public AccessDeniedException()
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="AccessDeniedException"/>.
    /// </summary>
    /// <param name="message">Message.</param>
    public AccessDeniedException(string message) : base(message)
    {
    }
}
