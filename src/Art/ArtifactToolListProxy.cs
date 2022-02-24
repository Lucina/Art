using System.Runtime.CompilerServices;
using Art.Logging;
using Art.Management;
using Art.Resources;

namespace Art;

/// <summary>
/// Proxy to run artifact tool as a list tool.
/// </summary>
public record ArtifactToolListProxy
{
    /// <summary>Artifact tool.</summary>
    public ArtifactTool ArtifactTool { get; init; }

    /// <summary>List options.</summary>
    public ArtifactToolListOptions Options { get; init; }

    /// <summary>Log handler.</summary>
    public IToolLogHandler? LogHandler { get; init; }

    /// <summary>
    /// Proxy to run artifact tool as a list tool.
    /// </summary>
    /// <param name="artifactTool">Artifact tool.</param>
    /// <param name="options">List options.</param>
    /// <param name="logHandler">Log handler.</param>
    /// <exception cref="ArgumentException">Thrown when invalid options are specified.</exception>
    public ArtifactToolListProxy(ArtifactTool artifactTool, ArtifactToolListOptions options, IToolLogHandler? logHandler)
    {
        if (artifactTool == null) throw new ArgumentNullException(nameof(artifactTool));
        if (options == null) throw new ArgumentNullException(nameof(options));
        ArtifactToolListOptions.Validate(options, false);
        ArtifactTool = artifactTool;
        Options = options;
        LogHandler = logHandler;
    }

    #region API

    /// <summary>
    /// Lists artifacts.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Async-enumerable artifacts.</returns>
    /// <exception cref="InvalidOperationException">Thrown when an invalid configuration is detected.</exception>
    public async IAsyncEnumerable<ArtifactData> ListAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArtifactToolListOptions.Validate(Options, false);
        if (LogHandler != null) ArtifactTool.LogHandler = LogHandler;
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
                                yield break;
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
                yield return data;
            }
            yield break;
        }
        if (ArtifactTool is IArtifactToolDump dumpTool)
        {
            ArtifactDataManager previous = ArtifactTool.DataManager;
            try
            {
                InMemoryArtifactDataManager im = new();
                await dumpTool.DumpAsync(cancellationToken).ConfigureAwait(false);
                foreach ((ArtifactKey ak, List<ArtifactResourceInfo> resources) in im.Artifacts)
                {
                    if (await ArtifactTool.TryGetArtifactAsync(ak, cancellationToken).ConfigureAwait(false) is not { } info) continue;
                    ArtifactData data = new(info);
                    data.AddRange(resources);
                    yield return data;
                }
            }
            finally
            {
                ArtifactTool.DataManager = previous;
            }
            yield break;
        }
        throw new NotSupportedException("Artifact tool is not a supported type");
    }

    #endregion
}
