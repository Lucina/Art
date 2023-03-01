using System.Diagnostics.CodeAnalysis;

namespace Art.Tesler;

public interface IProfileResolver
{
    bool TryGetProfiles(string text, [NotNullWhen(true)] out IEnumerable<ArtifactToolProfile>? profiles);
}
