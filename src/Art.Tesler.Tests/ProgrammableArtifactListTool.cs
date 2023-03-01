using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Art.Common;

namespace Art.Tesler.Tests;

internal class ProgrammableArtifactListTool : ArtifactTool, IArtifactListTool
{
    public delegate List<IArtifactData> SynchronousListDelegate(ProgrammableArtifactListTool tool);

    public readonly SynchronousListDelegate? ListFunc;

    public ProgrammableArtifactListTool(SynchronousListDelegate? listFunc)
    {
        ListFunc = listFunc;
    }

    public IAsyncEnumerable<IArtifactData> ListAsync(CancellationToken cancellationToken = default)
    {
        if (ListFunc == null)
        {
            throw new NotImplementedException();
        }
        return ListFunc(this).ToAsyncEnumerable();
    }

    public static ArtifactToolRegistryEntry CreateRegistryEntry(SynchronousListDelegate synchronousListDelegate)
    {
        return new CustomArtifactToolRegistryEntry(ArtifactToolIDUtil.CreateToolId<ProgrammableArtifactListTool>(), synchronousListDelegate);
    }

    private record CustomArtifactToolRegistryEntry(ArtifactToolID Id, SynchronousListDelegate SynchronousListDelegate) : ArtifactToolRegistryEntry(Id)
    {
        public override IArtifactTool CreateArtifactTool() => new ProgrammableArtifactListTool(SynchronousListDelegate);

        public override Type GetArtifactToolType() => typeof(ProgrammableArtifactListTool);
    }
}
