using System.Diagnostics.CodeAnalysis;

namespace Art.Tesler;

public class AggregateProfileResolver : IProfileResolver
{
    private readonly IProfileResolver[] _profileResolvers;

    public AggregateProfileResolver(IProfileResolver[] profileResolvers)
    {
        _profileResolvers = profileResolvers;
    }

    public bool TryGetProfiles(string text, [NotNullWhen(true)] out IEnumerable<ArtifactToolProfile>? profiles)
    {
        foreach (var resolver in _profileResolvers)
        {
            if (resolver.TryGetProfiles(text, out profiles))
            {
                return true;
            }
        }
        profiles = null;
        return false;
    }
}
