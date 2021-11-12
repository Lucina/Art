using System.Diagnostics.CodeAnalysis;

namespace Art;

/// <summary>
/// Represents an instance of an artifact dumper.
/// </summary>
public abstract class ArtifactDumper
{
    /// <summary>
    /// Data manager used by this instance.
    /// </summary>
    public ArtifactDataManager DataManager { get; }
    /// <summary>
    /// Origin dumping profile.
    /// </summary>
    public ArtifactDumpingProfile Profile { get; }

    /// <summary>
    /// Creates a new instance of <see cref="ArtifactDumper"/>.
    /// </summary>
    /// <param name="dataManager">Data manager to use for this instance.</param>
    /// <param name="artifactDumpingProfile">Origin dumping profile.</param>
    protected ArtifactDumper(ArtifactDataManager dataManager, ArtifactDumpingProfile artifactDumpingProfile) => (DataManager, Profile) = (dataManager, artifactDumpingProfile);

    /// <summary>
    /// Dump artifacts.
    /// </summary>
    /// <returns>Task.</returns>
    public abstract Task DumpAsync();

    /// <summary>
    /// Attempt to get option or throw exception if not found or if null.
    /// </summary>
    /// <param name="optKey">Key to search.</param>
    /// <param name="value">Value, if located and nonnull.</param>
    /// <exception cref="ArtifactDumperOptionNotFoundException">Thrown when option is not found or is null.</exception>
    protected void GetOptionOrExcept(string optKey, out string value)
    {
        if (!Profile.Options.TryGetValue(optKey, out string? vv) || vv == null) throw new ArtifactDumperOptionNotFoundException(optKey);
        value = vv;
    }

    /// <summary>
    /// Attempt to get option.
    /// </summary>
    /// <param name="optKey">Key to search.</param>
    /// <param name="value">Value, if located and nonnull.</param>
    /// <returns>True if value is located and nonnull.</returns>
    protected bool TryGetOption(string optKey, [NotNullWhen(true)] out string? value)
    {
        if (!Profile.Options.TryGetValue(optKey, out string? vv) || vv == null)
        {
            value = null;
            return false;
        }
        value = vv;
        return true;
    }
}
