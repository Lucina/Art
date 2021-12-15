namespace Art;

/// <summary>
/// Proxy to run artifact tool as a dump tool.
/// </summary>
public record ArtifactToolDumpProxy
{
    /// <summary>Artifact tool.</summary>
    public ArtifactTool ArtifactTool { get; init; }

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
    public ArtifactToolDumpProxy(ArtifactTool artifactTool, ArtifactToolDumpOptions options, IToolLogHandler? logHandler)
    {
        if (artifactTool == null) throw new ArgumentNullException(nameof(artifactTool));
        if (options == null) throw new ArgumentNullException(nameof(options));
        ArtifactToolDumpOptions.Validate(options, true);
        ArtifactTool = artifactTool;
        Options = options;
        LogHandler = logHandler;
    }


    /// <summary>
    /// Dumps artifacts.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public async ValueTask DumpAsync(CancellationToken cancellationToken = default)
    {
        ArtifactToolDumpOptions.Validate(Options, false);
        if (LogHandler != null) ArtifactTool.LogHandler = LogHandler;
        if (ArtifactTool is IArtifactToolDump dumpTool)
        {
            await dumpTool.DumpAsync(cancellationToken).ConfigureAwait(false);
            return;
        }
        if (ArtifactTool is IArtifactToolList listTool)
        {
            IAsyncEnumerable<ArtifactData> enumerable = listTool.ListAsync(cancellationToken);
            if ((Options.EagerFlags & ArtifactTool.AllowedEagerModes & EagerFlags.ArtifactList) != 0) enumerable = enumerable.EagerAsync();
            await foreach (ArtifactData data in enumerable.ConfigureAwait(false))
            {
                switch (Options.SkipMode)
                {
                    case ArtifactSkipMode.None:
                        break;
                    case ArtifactSkipMode.FastExit:
                        {
                            ArtifactInfo? info = await ArtifactTool.TryGetArtifactAsync(data.Info.Key.Id, cancellationToken).ConfigureAwait(false);
                            if (info != null)
                                return;
                            break;
                        }
                    case ArtifactSkipMode.Known:
                        {
                            ArtifactInfo? info = await ArtifactTool.TryGetArtifactAsync(data.Info.Key.Id, cancellationToken).ConfigureAwait(false);
                            if (info != null)
                                continue;
                            break;
                        }
                }
                if (!data.Info.Full && !Options.IncludeNonFull) continue;
                await ArtifactTool.DumpArtifactAsync(data, LogHandler, Options.ResourceUpdate, Options.EagerFlags, cancellationToken);
            }
            return;
        }
        throw new NotSupportedException("Artifact tool is not a supported type");
    }
}
