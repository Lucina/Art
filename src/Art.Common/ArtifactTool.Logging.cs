namespace Art.Common;

public partial class ArtifactTool
{
    #region Logging

    /// <summary>
    /// Logs information log to logger.
    /// </summary>
    /// <param name="title">Title.</param>
    /// <param name="body">Body.</param>
    public void LogInformation(string? title, string? body = null) => LogInternal(title, body, LogLevel.Information);

    /// <summary>
    /// Logs entry log to logger.
    /// </summary>
    /// <param name="title">Title.</param>
    /// <param name="body">Body.</param>
    public void LogEntry(string? title, string? body = null) => LogInternal(title, body, LogLevel.Entry);

    /// <summary>
    /// Logs title log to logger.
    /// </summary>
    /// <param name="title">Title.</param>
    /// <param name="body">Body.</param>
    public void LogTitle(string? title, string? body = null) => LogInternal(title, body, LogLevel.Title);

    /// <summary>
    /// Logs warning log to logger.
    /// </summary>
    /// <param name="title">Title.</param>
    /// <param name="body">Body.</param>
    public void LogWarning(string? title, string? body = null) => LogInternal(title, body, LogLevel.Warning);

    /// <summary>
    /// Logs error log to logger.
    /// </summary>
    /// <param name="title">Title.</param>
    /// <param name="body">Body.</param>
    public void LogError(string? title, string? body = null) => LogInternal(title, body, LogLevel.Error);

    private void LogInternal(string? title, string? body, LogLevel logLevel)
    {
        LogHandler?.Log(Profile.Tool, Profile.GetGroupOrFallback(GroupFallback), title, body, logLevel);
    }

    #endregion
}
