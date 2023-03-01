using System.Diagnostics.CodeAnalysis;

namespace Art.Tesler;

public class SelectableToolProfileResolver : IProfileResolver
{
    private List<IArtifactToolSelectableRegistry<string>>? _selectableRegistries;
    private readonly IArtifactToolRegistryStore _pluginStore;

    public SelectableToolProfileResolver(IArtifactToolRegistryStore pluginStore)
    {
        _pluginStore = pluginStore;
    }

    public bool TryGetProfiles(string text, [NotNullWhen(true)] out IEnumerable<ArtifactToolProfile>? profiles)
    {
        if (_selectableRegistries == null)
        {
            _selectableRegistries = new List<IArtifactToolSelectableRegistry<string>>();
            foreach (var registry in _pluginStore.LoadAllRegistries())
            {
                if (registry is IArtifactToolSelectableRegistry<string> selectableRegistry)
                {
                    _selectableRegistries.Add(selectableRegistry);
                }
            }
        }
        if (!PurificationUtil.TryIdentify(_selectableRegistries, text, out var profile))
        {
            profiles = null;
            return false;
        }
        profiles = new[] { profile };
        return true;
    }
}
