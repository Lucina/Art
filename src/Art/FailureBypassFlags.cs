namespace Art;

/// <summary>
/// Represents kinds of failures to ignore.
/// </summary>
[Flags]
public enum FailureBypassFlags
{
    /// <summary>
    /// Ignore no failures.
    /// </summary>
    None = 0,
    /// <summary>
    /// Ignore miscellaneous failures.
    /// </summary>
    Miscellaneous = 1 << 0,
    /// <summary>
    /// Ignore geoblocking.
    /// </summary>
    Geoblocking = 1 << 1,
    /// <summary>
    /// Ignore access denial.
    /// </summary>
    AccessDenied = 1 << 2,
    /// <summary>
    /// Ignore maintenance downtime.
    /// </summary>
    Maintenance = 1 << 3,
    /// <summary>
    /// Ignore network errors.
    /// </summary>
    Network = 1 << 4,
    /// <summary>
    /// Ignore IO errors.
    /// </summary>
    IO = 1 << 5,
    /// <summary>
    /// Ignore all failures.
    /// </summary>
    All = Miscellaneous | Geoblocking | AccessDenied | Maintenance | Network | IO
}
