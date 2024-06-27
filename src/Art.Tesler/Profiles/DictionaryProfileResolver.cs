using System.Diagnostics.CodeAnalysis;

namespace Art.Tesler.Profiles;

public class DictionaryProfileResolver : IProfileResolver
{
    private readonly IReadOnlyDictionary<string, IReadOnlyCollection<ArtifactToolProfile>> _profileMap;

    public DictionaryProfileResolver(IReadOnlyDictionary<string, IReadOnlyCollection<ArtifactToolProfile>> profileMap)
    {
        _profileMap = profileMap;
    }

    public bool TryGetProfiles(string text, [NotNullWhen(true)] out IEnumerable<ArtifactToolProfile>? profiles, ProfileResolutionFlags profileResolutionFlags = ProfileResolutionFlags.Default)
    {
        if ((profileResolutionFlags & ProfileResolutionFlags.Files) != 0 && _profileMap.TryGetValue(text, out var profileArr))
        {
            profiles = profileArr;
            return true;
        }
        profiles = null;
        return false;
    }
}
