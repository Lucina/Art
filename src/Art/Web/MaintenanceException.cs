namespace Art.Web;

/// <summary>
/// Exception thrown when external factors are undergoing maintenance.
/// </summary>
public class MaintenanceException : Exception
{
    /// <summary>
    /// Creates a new instance of <see cref="MaintenanceException"/>.
    /// </summary>
    public MaintenanceException()
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="MaintenanceException"/>.
    /// </summary>
    /// <param name="message">Message.</param>
    public MaintenanceException(string message) : base(message)
    {
    }
}
