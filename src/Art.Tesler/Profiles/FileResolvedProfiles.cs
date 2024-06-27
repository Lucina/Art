using Art.Common;

namespace Art.Tesler.Profiles;

public class FileResolvedProfiles : IWritableResolvedProfiles
{
    public IReadOnlyList<ArtifactToolProfile> Values { get; }
    public string FilePath { get; }

    public FileResolvedProfiles(IReadOnlyList<ArtifactToolProfile> values, string filePath)
    {
        Values = values;
        FilePath = filePath;
    }

    public void WriteProfiles(IReadOnlyList<ArtifactToolProfile> artifactToolProfiles)
    {
        ArtifactToolProfileUtil.SerializeProfilesToFile(FilePath, artifactToolProfiles);
    }
}
