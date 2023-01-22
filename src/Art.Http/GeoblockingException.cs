namespace Art.Http;

/// <summary>
/// Exception thrown when geoblocking has been encountered.
/// </summary>
public class GeoblockingException : Exception
{
    /// <summary>
    /// Creates a new instance of <see cref="GeoblockingException"/>.
    /// </summary>
    public GeoblockingException()
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="GeoblockingException"/>.
    /// </summary>
    /// <param name="message">Message.</param>
    public GeoblockingException(string message) : base(message)
    {
    }
}
