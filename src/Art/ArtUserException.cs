namespace Art;

/// <summary>
/// Represents an exception for any trivial user exception such as missing configuration values or an unknown tool.
/// </summary>
public class ArtUserException : Exception
{
    /// <summary>
    /// Initializes an instance of <see cref="ArtUserException"/>.
    /// </summary>
    public ArtUserException()
    {
    }

    /// <summary>
    /// Initializes an instance of <see cref="ArtUserException"/> with a message.
    /// </summary>
    /// <param name="message">Message.</param>
    public ArtUserException(string? message) : base(message)
    {
    }
}
