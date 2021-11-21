namespace Art
{
    /// <summary>
    /// Log handler output to console.
    /// </summary>
    public class ConsoleLogHandler : ILogHandler
    {
        /// <summary>
        /// Default instance.
        /// </summary>
        public static readonly ConsoleLogHandler Default = new();

        /// <inheritdoc/>
        public void Log(string? title, string? body, LogLevel level)
        {
            if (title != null)
            {
                Console.Write(level switch
                {
                    LogLevel.Information => "-- ",
                    LogLevel.Title => ">> ",
                    LogLevel.Warning => "?? ",
                    LogLevel.Error => "!! ",
                    _ => throw new ArgumentOutOfRangeException(nameof(level)),
                });
                Console.WriteLine(title);
            }
            if (body != null)
                Console.WriteLine(body);
        }
    }
}
