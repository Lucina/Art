using System.Diagnostics.CodeAnalysis;

namespace Art.Common;

/// <summary>
/// Provides a registry of artifact tools which can be created as needed.
/// </summary>
public class ArtifactToolRegistry : IArtifactToolRegistry
{
    private readonly Dictionary<ArtifactToolID, ArtifactToolRegistryEntry> _entries = new();

    /// <summary>
    /// Adds an entry to the registry.
    /// </summary>
    /// <param name="entry">Entry to add.</param>
    /// <exception cref="ArgumentException">Thrown if entry for name already exists.</exception>
    public void Add(ArtifactToolRegistryEntry entry)
    {
        if (_entries.ContainsKey(entry.Id))
        {
            throw new ArgumentException($"Registry already has an entry by the name {entry}");
        }
        _entries.Add(entry.Id, entry);
    }

    /// <summary>
    /// Adds an entry to the registry.
    /// </summary>
    /// <typeparam name="T">Factory type.</typeparam>
    /// <exception cref="ArgumentException">Thrown if entry for name already exists.</exception>
    public void Add<T>() where T : IArtifactToolFactory
    {
        Add(new ArtifactToolRegistryEntry<T>(T.GetArtifactToolId()));
    }

    /// <summary>
    /// Attempts to add an entry to the registry.
    /// </summary>
    /// <param name="entry">Entry to add.</param>
    /// <returns>True if successfully added entry.</returns>
    public bool TryAdd(ArtifactToolRegistryEntry entry)
    {
        return _entries.TryAdd(entry.Id, entry);
    }

    /// <summary>
    /// Attempts to add an entry to the registry.
    /// </summary>
    /// <typeparam name="T">Factory type.</typeparam>
    /// <returns>True if successfully added entry.</returns>
    public bool TryAdd<T>() where T : IArtifactToolFactory
    {
        return TryAdd(new ArtifactToolRegistryEntry<T>(T.GetArtifactToolId()));
    }

    /// <summary>
    /// Attempts to create an artifact tool for the specified name.
    /// </summary>
    /// <param name="artifactToolId">Artifact tool ID.</param>
    /// <param name="tool">Crated artifact tool, if successful.</param>
    /// <returns>True if successful.</returns>
    public bool TryLoad(ArtifactToolID artifactToolId, [NotNullWhen(true)] out IArtifactTool? tool)
    {
        if (_entries.TryGetValue(artifactToolId, out var entry))
        {
            tool = entry.CreateArtifactTool();
            return true;
        }
        tool = null;
        return false;
    }
}
