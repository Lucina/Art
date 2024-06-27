using System.Diagnostics.CodeAnalysis;

namespace Art.Tesler.Profiles;

public interface IProfileResolver
{
    bool TryGetProfiles(string text, [NotNullWhen(true)] out IEnumerable<ArtifactToolProfile>? profiles, ProfileResolutionFlags profileResolutionFlags = ProfileResolutionFlags.Default);
}
