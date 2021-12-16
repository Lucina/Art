namespace Art;

/// <summary>
/// Contains methods for failure bypass.
/// </summary>
public static class FailureBypass
{
    /// <summary>
    /// Evaluates whether the exception should be ignored based on ignore flags.
    /// </summary>
    /// <param name="exception">Exception.</param>
    /// <param name="flags">Flags specifying types of errors to ignore.</param>
    /// <returns>True if exception should be ignored.</returns>
    public static bool ShouldBypass(Exception exception, FailureBypassFlags flags)
    {
        return (exception switch
        {
            GeoblockingException => FailureBypassFlags.Geoblocking,
            AccessDeniedException => FailureBypassFlags.AccessDenied,
            MaintenanceException => FailureBypassFlags.Maintenance,
            HttpRequestException => FailureBypassFlags.Network,
            IOException => FailureBypassFlags.IO,
            _ => FailureBypassFlags.Miscellaneous
        } & flags) != 0;
    }
}
