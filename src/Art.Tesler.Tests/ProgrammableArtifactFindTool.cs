using System;
using System.Threading;
using System.Threading.Tasks;
using Art.Common;

namespace Art.Tesler.Tests;

internal class ProgrammableArtifactFindTool : ArtifactTool, IArtifactFindTool
{
    public delegate IArtifactData? SynchronousFindDelegate(ProgrammableArtifactFindTool tool, string id);

    public readonly SynchronousFindDelegate? FindFunc;

    public ProgrammableArtifactFindTool(SynchronousFindDelegate? findFunc)
    {
        FindFunc = findFunc;
    }

    public Task<IArtifactData?> FindAsync(string id, CancellationToken cancellationToken = default)
    {
        if (FindFunc != null)
        {
            return Task.FromResult(FindFunc(this, id));
        }
        throw new NotImplementedException();
    }

    public static ArtifactToolRegistryEntry CreateRegistryEntry(SynchronousFindDelegate synchronousFindDelegate)
    {
        return CreateRegistryEntry(ArtifactToolIDUtil.CreateToolId<ProgrammableArtifactFindTool>(), synchronousFindDelegate);
    }

    public static ArtifactToolRegistryEntry CreateRegistryEntry(ArtifactToolID artifactToolId, SynchronousFindDelegate synchronousFindDelegate)
    {
        return new CustomArtifactToolRegistryEntry(artifactToolId, synchronousFindDelegate);
    }

    private record CustomArtifactToolRegistryEntry(ArtifactToolID Id, SynchronousFindDelegate SynchronousFindDelegate) : ArtifactToolRegistryEntry(Id)
    {
        public override IArtifactTool CreateArtifactTool() => new ProgrammableArtifactFindTool(SynchronousFindDelegate);

        public override Type GetArtifactToolType() => typeof(ProgrammableArtifactFindTool);
    }
}
