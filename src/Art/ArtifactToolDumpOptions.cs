namespace Art;

/// <summary>
/// Represents runtime options for dump operation.
/// </summary>
/// <param name="ResourceUpdate">Resource update mode.</param>
/// <param name="IncludeNonFull">Overwrite full entries with non-full if newer.</param>
/// <param name="SkipMode">Skip mode.</param>
/// <param name="EagerFlags">Eager evaluation flags.</param>
/// <param name="ChecksumId">Checksum algorithm ID.</param>
public record ArtifactToolDumpOptions(
    ResourceUpdateMode ResourceUpdate = ResourceUpdateMode.ArtifactHard,
    bool IncludeNonFull = true,
    ArtifactSkipMode SkipMode = ArtifactSkipMode.None,
    EagerFlags EagerFlags = EagerFlags.None,
    string? ChecksumId = null)
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
                if (constructor)
                    throw new ArgumentException($"Invalid {nameof(ArtifactToolDumpOptions)}.{nameof(SkipMode)}");
                else
                    throw new InvalidOperationException($"Invalid {nameof(ArtifactToolDumpOptions)}.{nameof(SkipMode)}");
        }
        switch (options.ResourceUpdate)
        {
            case ResourceUpdateMode.ArtifactSoft:
            case ResourceUpdateMode.ArtifactHard:
            case ResourceUpdateMode.Soft:
            case ResourceUpdateMode.Hard:
                break;
            default:
                if (constructor)
                    throw new ArgumentException($"Invalid {nameof(ArtifactToolDumpOptions)}.{nameof(ResourceUpdate)}");
                else
                    throw new InvalidOperationException($"Invalid {nameof(ArtifactToolDumpOptions)}.{nameof(ResourceUpdate)}");
        }
    }
}
