using System.Diagnostics.CodeAnalysis;
using Art.Common;

namespace Art.Tesler;

public class DiskProfileResolver : IProfileResolver
{
    public bool TryGetProfiles(string text, [NotNullWhen(true)] out IEnumerable<ArtifactToolProfile>? profiles, ProfileResolutionFlags profileResolutionFlags = ProfileResolutionFlags.Default)
    {
        if ((profileResolutionFlags & ProfileResolutionFlags.Files) != 0 && File.Exists(text))
        {
            profiles = ArtifactToolProfileUtil.DeserializeProfilesFromFile(text);
            return true;
        }
        profiles = null;
        return false;
    }
}
