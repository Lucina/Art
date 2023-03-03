using Art.Common;

namespace Art.TestsBase;

public class ProgrammableArtifactDumpTool : ArtifactTool, IArtifactDumpTool
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
        return CreateRegistryEntry(ArtifactToolIDUtil.CreateToolId<ProgrammableArtifactDumpTool>(), synchronousDumpDelegate);
    }

    public static ArtifactToolRegistryEntry CreateRegistryEntry(ArtifactToolID artifactToolId, SynchronousDumpDelegate synchronousDumpDelegate)
    {
        return new CustomArtifactToolRegistryEntry(artifactToolId, synchronousDumpDelegate);
    }

    private record CustomArtifactToolRegistryEntry(ArtifactToolID Id, SynchronousDumpDelegate SynchronousDumpDelegate) : ArtifactToolRegistryEntry(Id)
    {
        public override IArtifactTool CreateArtifactTool() => new ProgrammableArtifactDumpTool(SynchronousDumpDelegate);

        public override Type GetArtifactToolType() => typeof(ProgrammableArtifactDumpTool);
    }
}

public class AsyncProgrammableArtifactDumpTool : ArtifactTool, IArtifactDumpTool
{
    public delegate Task AsyncDumpDelegate(AsyncProgrammableArtifactDumpTool tool);

    public readonly AsyncDumpDelegate? AsyncDumpAction;

    public AsyncProgrammableArtifactDumpTool(AsyncDumpDelegate asyncDumpAction)
    {
        AsyncDumpAction = asyncDumpAction;
    }

    public Task DumpAsync(CancellationToken cancellationToken = default)
    {
        if (AsyncDumpAction != null)
        {
            return AsyncDumpAction(this);
        }
        throw new NotImplementedException();
    }

    public static ArtifactToolRegistryEntry CreateRegistryEntry(AsyncDumpDelegate asyncDumpDelegate)
    {
        return CreateRegistryEntry(ArtifactToolIDUtil.CreateToolId<AsyncProgrammableArtifactDumpTool>(), asyncDumpDelegate);
    }

    public static ArtifactToolRegistryEntry CreateRegistryEntry(ArtifactToolID artifactToolId, AsyncDumpDelegate asyncDumpDelegate)
    {
        return new CustomArtifactToolRegistryEntry(artifactToolId, asyncDumpDelegate);
    }

    private record CustomArtifactToolRegistryEntry(ArtifactToolID Id, AsyncDumpDelegate AsyncDumpDelegate) : ArtifactToolRegistryEntry(Id)
    {
        public override IArtifactTool CreateArtifactTool() => new AsyncProgrammableArtifactDumpTool(AsyncDumpDelegate);

        public override Type GetArtifactToolType() => typeof(AsyncProgrammableArtifactDumpTool);
    }
}
