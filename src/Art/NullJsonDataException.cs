namespace Art;

/// <summary>
/// Represents exception thrown when null JSON data is encountered.
/// </summary>
public class NullJsonDataException : Exception
{
    /// <summary>
    /// Creates a new instance of <see cref="NullJsonDataException"/>.
    /// </summary>
    public NullJsonDataException()
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="NullJsonDataException"/> with the specified message.
    /// </summary>
    /// <param name="message">Message.</param>
    public NullJsonDataException(string message) : base(message)
    {
    }
}
