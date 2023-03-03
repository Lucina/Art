using Art.Common;

namespace Art.TestsBase;

public class ProgrammableArtifactListTool : ArtifactTool, IArtifactListTool
{
    public delegate List<IArtifactData> SynchronousListDelegate(ProgrammableArtifactListTool tool);

    public readonly SynchronousListDelegate? SynchronousListFunc;

    public ProgrammableArtifactListTool(SynchronousListDelegate? synchronousListFunc)
    {
        SynchronousListFunc = synchronousListFunc;
    }

    public IAsyncEnumerable<IArtifactData> ListAsync(CancellationToken cancellationToken = default)
    {
        if (SynchronousListFunc == null)
        {
            throw new NotImplementedException();
        }
        return SynchronousListFunc(this).ToAsyncEnumerable();
    }

    public static ArtifactToolRegistryEntry CreateRegistryEntry(SynchronousListDelegate synchronousListDelegate)
    {
        return CreateRegistryEntry(ArtifactToolIDUtil.CreateToolId<ProgrammableArtifactListTool>(), synchronousListDelegate);
    }

    public static ArtifactToolRegistryEntry CreateRegistryEntry(ArtifactToolID artifactToolId, SynchronousListDelegate synchronousListDelegate)
    {
        return new CustomArtifactToolRegistryEntry(artifactToolId, synchronousListDelegate);
    }

    private record CustomArtifactToolRegistryEntry(ArtifactToolID Id, SynchronousListDelegate SynchronousListDelegate) : ArtifactToolRegistryEntry(Id)
    {
        public override IArtifactTool CreateArtifactTool() => new ProgrammableArtifactListTool(SynchronousListDelegate);

        public override Type GetArtifactToolType() => typeof(ProgrammableArtifactListTool);
    }
}

public class AsyncProgrammableArtifactListTool : ArtifactTool, IArtifactListTool
{
    public delegate IAsyncEnumerable<IArtifactData> AsyncListDelegate(AsyncProgrammableArtifactListTool tool);

    public readonly AsyncListDelegate? AsyncListFunc;

    public AsyncProgrammableArtifactListTool(AsyncListDelegate? asyncListFunc)
    {
        AsyncListFunc = asyncListFunc;
    }

    public IAsyncEnumerable<IArtifactData> ListAsync(CancellationToken cancellationToken = default)
    {
        if (AsyncListFunc == null)
        {
            throw new NotImplementedException();
        }
        return AsyncListFunc(this);
    }

    public static ArtifactToolRegistryEntry CreateRegistryEntry(AsyncListDelegate asyncListDelegate)
    {
        return CreateRegistryEntry(ArtifactToolIDUtil.CreateToolId<AsyncProgrammableArtifactListTool>(), asyncListDelegate);
    }

    public static ArtifactToolRegistryEntry CreateRegistryEntry(ArtifactToolID artifactToolId, AsyncListDelegate asyncListDelegate)
    {
        return new CustomArtifactToolRegistryEntry(artifactToolId, asyncListDelegate);
    }

    private record CustomArtifactToolRegistryEntry(ArtifactToolID Id, AsyncListDelegate AsyncListDelegate) : ArtifactToolRegistryEntry(Id)
    {
        public override IArtifactTool CreateArtifactTool() => new AsyncProgrammableArtifactListTool(AsyncListDelegate);

        public override Type GetArtifactToolType() => typeof(AsyncProgrammableArtifactListTool);
    }
}
