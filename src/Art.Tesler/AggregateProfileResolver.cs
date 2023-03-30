using System.Diagnostics.CodeAnalysis;

namespace Art.Tesler;

public class AggregateProfileResolver : IProfileResolver
{
    private readonly IProfileResolver[] _profileResolvers;

    public AggregateProfileResolver(IProfileResolver[] profileResolvers)
    {
        _profileResolvers = profileResolvers;
    }

    public bool TryGetProfiles(string text, [NotNullWhen(true)] out IEnumerable<ArtifactToolProfile>? profiles, ProfileResolutionFlags profileResolutionFlags = ProfileResolutionFlags.Default)
    {
        foreach (var resolver in _profileResolvers)
        {
            if (resolver.TryGetProfiles(text, out profiles, profileResolutionFlags))
            {
                return true;
            }
        }
        profiles = null;
        return false;
    }
}
