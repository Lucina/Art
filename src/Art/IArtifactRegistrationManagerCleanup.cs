namespace Art;

/// <summary>
/// Represents a manager for artifact data that supports cleanup of database content.
/// </summary>
public interface IArtifactRegistrationManagerCleanup
{
    /// <summary>
    /// Performs cleanup of database.
    /// </summary>
    /// <returns>Task representing operation.</returns>
    Task CleanupDatabaseAsync(CancellationToken cancellationToken = default);
}
