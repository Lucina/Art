namespace Art.Proxies;

/// <summary>
/// Represents runtime options for dump operation.
/// </summary>
/// <param name="ResourceUpdate">Resource update mode.</param>
/// <param name="IncludeNonFull">Overwrite full entries with non-full if newer.</param>
/// <param name="SkipMode">Skip mode.</param>
/// <param name="ChecksumId">Checksum algorithm ID.</param>
/// <param name="FailureFlags">Failure bypass flags.</param>
/// <param name="EagerFlags">Eager evaluation flags.</param>
public record ArtifactToolDumpOptions(
    ResourceUpdateMode ResourceUpdate = ResourceUpdateMode.ArtifactHard,
    bool IncludeNonFull = true,
    ArtifactSkipMode SkipMode = ArtifactSkipMode.None,
    string? ChecksumId = null,
    FailureFlags FailureFlags = FailureFlags.None,
    EagerFlags EagerFlags = EagerFlags.None)
{
    /// <summary>
    /// Default options.
    /// </summary>
    public static readonly ArtifactToolDumpOptions Default = new();

    /// <summary>
    /// Validates an instance of <see cref="ArtifactToolDumpOptions"/>.
    /// </summary>
    /// <param name="options">Options to validate.</param>
    /// <param name="constructor">True if called from an object constructor.</param>
    /// <exception cref="ArgumentException">Exception thrown for invalid configuration in constructor.</exception>
    /// <exception cref="InvalidOperationException">Exception thrown for invalid configuration anywhere except constructor.</exception>
    public static void Validate(ArtifactToolDumpOptions options, bool constructor)
    {
        if (options == null) throw new ArgumentNullException(nameof(options));
        switch (options.SkipMode)
        {
            case ArtifactSkipMode.None:
            case ArtifactSkipMode.FastExit:
            case ArtifactSkipMode.Known:
                break;
            default:
                ArtUtils.ThrowArgumentExceptionOrInvalidOperationExceptionWithMessage(constructor, $"Invalid {nameof(ArtifactToolDumpOptions)}.{nameof(SkipMode)}");
                break;

        }
        switch (options.ResourceUpdate)
        {
            case ResourceUpdateMode.ArtifactSoft:
            case ResourceUpdateMode.ArtifactHard:
            case ResourceUpdateMode.Soft:
            case ResourceUpdateMode.Hard:
                break;
            default:
                ArtUtils.ThrowArgumentExceptionOrInvalidOperationExceptionWithMessage(constructor, $"Invalid {nameof(ArtifactToolDumpOptions)}.{nameof(ResourceUpdate)}");
                return;
        }
    }
}
