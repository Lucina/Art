﻿using System.Diagnostics.CodeAnalysis;

namespace Art.Common;

/// <summary>
/// Provides a registry of artifact tools which can be created as needed.
/// </summary>
public class ArtifactToolRegistry : IArtifactToolSelectableRegistry<string>
{
    /// <summary>
    /// Mapping of entries.
    /// </summary>
    public IReadOnlyDictionary<ArtifactToolID, ArtifactToolRegistryEntry> Entries => _entries;

    private readonly Dictionary<ArtifactToolID, ArtifactToolRegistryEntry> _entries = new();

    /// <summary>
    /// Adds an entry to the registry.
    /// </summary>
    /// <param name="entry">Entry to add.</param>
    /// <exception cref="ArgumentException">Thrown if entry for artifact tool ID already exists.</exception>
    public void Add(ArtifactToolRegistryEntry entry)
    {
        if (_entries.ContainsKey(entry.Id))
        {
            throw new ArgumentException($"Registry already has an entry for artifact tool ID {entry.Id}");
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
    /// Adds an entry to the registry.
    /// </summary>
    /// <typeparam name="T">Factory type.</typeparam>
    /// <exception cref="ArgumentException">Thrown if entry for name already exists.</exception>
    public void AddSelectable<T>() where T : IArtifactToolFactory, IArtifactToolSelector<string>
    {
        Add(new ArtifactToolSelectableRegistryEntry<T>(T.GetArtifactToolId()));
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

    /// <inheritdoc />
    public bool Contains(ArtifactToolID artifactToolId)
    {
        return _entries.ContainsKey(artifactToolId);
    }

    /// <summary>
    /// Attempts to remove an entry from the registry.
    /// </summary>
    /// <param name="artifactToolId">Artifact tool ID.</param>
    /// <returns>True if successfully removed.</returns>
    public bool Remove(ArtifactToolID artifactToolId)
    {
        return _entries.Remove(artifactToolId);
    }

    /// <summary>
    /// Removes all contained entries.
    /// </summary>
    public void Clear()
    {
        _entries.Clear();
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

    /// <inheritdoc />
    public bool TryIdentify(string key, out ArtifactToolID artifactToolId, [NotNullWhen(true)] out string? artifactId)
    {
        foreach (var entry in _entries.Values)
        {
            if (entry is ArtifactToolSelectableRegistryEntry selectorEntry && selectorEntry.TryIdentify(key, out artifactToolId, out artifactId))
            {
                return true;
            }
        }
        artifactToolId = default;
        artifactId = null;
        return false;
    }
}
