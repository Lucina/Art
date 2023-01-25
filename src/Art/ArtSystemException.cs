namespace Art;

/// <summary>
/// Represents an exception for any exception caused by external services.
/// </summary>
public class ArtSystemException : Exception
{
    /// <summary>
    /// Initializes an instance of <see cref="ArtSystemException"/>.
    /// </summary>
    public ArtSystemException()
    {
    }

    /// <summary>
    /// Initializes an instance of <see cref="ArtSystemException"/> with a message.
    /// </summary>
    /// <param name="message">Message.</param>
    public ArtSystemException(string? message) : base(message)
    {
    }
}
