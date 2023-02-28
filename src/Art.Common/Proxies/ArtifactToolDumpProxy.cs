namespace Art.Common.Proxies;

/// <summary>
/// Proxy to run artifact tool as a dump tool.
/// </summary>
public record ArtifactToolDumpProxy
{
    private const string OptArtifactList = "artifactList";

    /// <summary>Artifact tool.</summary>
    public IArtifactTool ArtifactTool { get; init; }

    /// <summary>Dump options.</summary>
    public ArtifactToolDumpOptions Options { get; init; }

    /// <summary>Log handler.</summary>
    public IToolLogHandler? LogHandler { get; init; }

    /// <summary>
    /// Proxy to run artifact tool as a dump tool.
    /// </summary>
    /// <param name="artifactTool">Artifact tool.</param>
    /// <param name="options">Dump options.</param>
    /// <param name="logHandler">Log handler.</param>
    public ArtifactToolDumpProxy(IArtifactTool artifactTool, ArtifactToolDumpOptions options, IToolLogHandler? logHandler)
    {
        if (artifactTool == null) throw new ArgumentNullException(nameof(artifactTool));
        if (options == null) throw new ArgumentNullException(nameof(options));
        Validate(options, true);
        ArtifactTool = artifactTool;
        Options = options;
        LogHandler = logHandler;
    }

    private static void Validate(ArtifactToolDumpOptions options, bool constructor)
    {
        ArtifactToolDumpOptions.Validate(options, constructor);
        if (options.SkipMode == ArtifactSkipMode.FastExit && (options.EagerFlags & EagerFlags.ArtifactDump) != 0)
        {
            if (constructor)
            {
                throw new ArgumentException($"Cannot pair {nameof(ArtifactSkipMode)}.{nameof(ArtifactSkipMode.FastExit)} with {nameof(EagerFlags)}.{nameof(EagerFlags.ArtifactDump)}");
            }
            throw new InvalidOperationException($"Cannot pair {nameof(ArtifactSkipMode)}.{nameof(ArtifactSkipMode.FastExit)} with {nameof(EagerFlags)}.{nameof(EagerFlags.ArtifactDump)}");
        }
    }

    /// <summary>
    /// Dumps artifacts.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    /// <exception cref="InvalidOperationException">Thrown when an invalid configuration is detected.</exception>
    public async ValueTask DumpAsync(CancellationToken cancellationToken = default)
    {
        if (ArtifactTool == null) throw new InvalidOperationException("Artifact tool cannot be null");
        if (Options == null) throw new InvalidOperationException("Options cannot be null");
        Validate(Options, false);
        IArtifactTool artifactTool = ArtifactTool;
        if (artifactTool.Profile.Options.TryGetOption(OptArtifactList, out string[]? artifactList, SourceGenerationContext.s_context.StringArray) && artifactTool is IArtifactToolFind findTool)
        {
            artifactTool = new FindAsListTool(findTool, artifactList);
        }
        if (LogHandler != null) artifactTool.LogHandler = LogHandler;
        if (artifactTool is IArtifactToolDump dumpTool)
        {
            await dumpTool.DumpAsync(cancellationToken).ConfigureAwait(false);
            return;
        }
        if (artifactTool is IArtifactToolList listTool)
        {
            IAsyncEnumerable<IArtifactData> enumerable = listTool.ListAsync(cancellationToken);
            if ((Options.EagerFlags & artifactTool.AllowedEagerModes & EagerFlags.ArtifactList) != 0) enumerable = enumerable.EagerAsync();
            if ((Options.EagerFlags & artifactTool.AllowedEagerModes & EagerFlags.ArtifactDump) != 0)
            {
                List<Task> tasks = new();
                await foreach (IArtifactData data in enumerable.ConfigureAwait(false))
                {
                    switch (Options.SkipMode)
                    {
                        case ArtifactSkipMode.None:
                            break;
                        case ArtifactSkipMode.FastExit:
                            {
                                ArtifactInfo? info = await artifactTool.RegistrationManager.TryGetArtifactAsync(new ArtifactKey(artifactTool.Profile.Tool, artifactTool.Profile.GetGroupOrFallback(artifactTool.GroupFallback), data.Info.Key.Id), cancellationToken).ConfigureAwait(false);
                                if (info != null)
                                    goto E_ArtifactDump_WaitForTasks;
                                break;
                            }
                        case ArtifactSkipMode.Known:
                            {
                                ArtifactInfo? info = await artifactTool.RegistrationManager.TryGetArtifactAsync(new ArtifactKey(artifactTool.Profile.Tool, artifactTool.Profile.GetGroupOrFallback(artifactTool.GroupFallback), data.Info.Key.Id), cancellationToken).ConfigureAwait(false);
                                if (info != null)
                                    continue;
                                break;
                            }
                    }
                    if (!data.Info.Full && !Options.IncludeNonFull) continue;
                    tasks.Add(artifactTool.DumpArtifactAsync(data, Options.ResourceUpdate, Options.ChecksumId, Options.EagerFlags, LogHandler, cancellationToken));
                }
                E_ArtifactDump_WaitForTasks:
                await Task.WhenAll(tasks).ConfigureAwait(false);
                var exc = tasks
                    .Where(v => v.IsFaulted && v.Exception != null)
                    .SelectMany(v => v.Exception!.InnerExceptions)
                    .ToList();
                List<Exception> failed;
                if (Options.IgnoreException is { } ignoreException)
                {
                    var ignored = exc.Where(ignoreException).ToList();
                    foreach (var ignore in ignored)
                        LogHandler?.Log($"Ignored exception of type {ignore.GetType().FullName}", ignore.ToString(), LogLevel.Warning);
                    failed = exc.Where(v => !ignoreException(v)).ToList();
                }
                else
                {
                    failed = exc;
                }
                if (failed.Any())
                    throw new AggregateException(exc);
            }
            else
            {
                await foreach (IArtifactData data in enumerable.ConfigureAwait(false))
                {
                    switch (Options.SkipMode)
                    {
                        case ArtifactSkipMode.None:
                            break;
                        case ArtifactSkipMode.FastExit:
                            {
                                ArtifactInfo? info = await artifactTool.RegistrationManager.TryGetArtifactAsync(new ArtifactKey(artifactTool.Profile.Tool, artifactTool.Profile.GetGroupOrFallback(artifactTool.GroupFallback), data.Info.Key.Id), cancellationToken).ConfigureAwait(false);
                                if (info != null)
                                    return;
                                break;
                            }
                        case ArtifactSkipMode.Known:
                            {
                                ArtifactInfo? info = await artifactTool.RegistrationManager.TryGetArtifactAsync(new ArtifactKey(artifactTool.Profile.Tool, artifactTool.Profile.GetGroupOrFallback(artifactTool.GroupFallback), data.Info.Key.Id), cancellationToken).ConfigureAwait(false);
                                if (info != null)
                                    continue;
                                break;
                            }
                    }
                    if (!data.Info.Full && !Options.IncludeNonFull) continue;
                    try
                    {
                        await artifactTool.DumpArtifactAsync(data, Options.ResourceUpdate, Options.ChecksumId, Options.EagerFlags, LogHandler, cancellationToken).ConfigureAwait(false);
                    }
                    catch (Exception e)
                    {
                        if (Options.IgnoreException is { } ignoreException && ignoreException(e))
                            LogHandler?.Log($"Ignored exception of type {e.GetType().FullName}", e.ToString(), LogLevel.Warning);
                        else throw;
                    }
                }
            }
            return;
        }
        throw new NotSupportedException("Artifact tool is not a supported type");
    }
}
