using System.Diagnostics.CodeAnalysis;
using Art.Common;

namespace Art.Tesler;

public class DiskProfileResolver : IProfileResolver
{
    public bool TryGetProfiles(string text, [NotNullWhen(true)] out IEnumerable<ArtifactToolProfile>? profiles)
    {
        if (File.Exists(text))
        {
            profiles= ArtifactToolProfileUtil.DeserializeProfilesFromFile(text);
            return true;
        }
        profiles = null;
        return false;
    }
}
