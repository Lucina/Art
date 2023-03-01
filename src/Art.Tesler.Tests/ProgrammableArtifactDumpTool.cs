using System;
using System.Threading;
using System.Threading.Tasks;
using Art.Common;

namespace Art.Tesler.Tests;

internal class ProgrammableArtifactDumpTool : ArtifactTool, IArtifactDumpTool
{
    public delegate void SynchronousDumpDelegate(ProgrammableArtifactDumpTool tool);

    public readonly SynchronousDumpDelegate? SynchronousDumpAction;

    public ProgrammableArtifactDumpTool(SynchronousDumpDelegate synchronousDumpAction)
    {
        SynchronousDumpAction = synchronousDumpAction;
    }

    public Task DumpAsync(CancellationToken cancellationToken = default)
    {
        if (SynchronousDumpAction != null)
        {
            SynchronousDumpAction(this);
            return Task.CompletedTask;
        }
        throw new NotImplementedException();
    }

    public static ArtifactToolRegistryEntry CreateRegistryEntry(SynchronousDumpDelegate synchronousDumpDelegate)
    {
        return new CustomArtifactToolRegistryEntry(ArtifactToolIDUtil.CreateToolId<ProgrammableArtifactDumpTool>(), synchronousDumpDelegate);
    }

    private record CustomArtifactToolRegistryEntry(ArtifactToolID Id, SynchronousDumpDelegate SynchronousDumpDelegate) : ArtifactToolRegistryEntry(Id)
    {
        public override IArtifactTool CreateArtifactTool() => new ProgrammableArtifactDumpTool(SynchronousDumpDelegate);

        public override Type GetArtifactToolType() => typeof(ProgrammableArtifactDumpTool);
    }
}
