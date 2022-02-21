using System.Net;

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
    public static bool ShouldBypass(Exception exception, FailureBypassFlags flags) => (FilterFlags(exception) & flags) != 0;

    private static FailureBypassFlags FilterFlags(Exception exception) =>
        exception switch
        {
            AggregateException a => a.InnerExceptions.Aggregate(FailureBypassFlags.None, (f, e) => f | FilterFlags(e)),
            GeoblockingException => FailureBypassFlags.Geoblocking,
            AccessDeniedException => FailureBypassFlags.AccessDenied,
            MaintenanceException => FailureBypassFlags.Maintenance,
            HttpRequestException httpRequestException => FilterHttpFlags(httpRequestException),
            IOException => FailureBypassFlags.IO,
            _ => FailureBypassFlags.Miscellaneous
        };

    private static FailureBypassFlags FilterHttpFlags(HttpRequestException exception) =>
        exception.StatusCode switch
        {
            HttpStatusCode.Forbidden => FailureBypassFlags.AccessDenied,
            _ => FailureBypassFlags.Network
        };
}
