﻿using Art.Common;

namespace Art.TestsBase;

public class ProgrammableArtifactFindTool : ArtifactTool, IArtifactFindTool
{
    public delegate IArtifactData? SynchronousFindDelegate(ProgrammableArtifactFindTool tool, string id);

    public readonly SynchronousFindDelegate? SynchronousFindFunc;

    public ProgrammableArtifactFindTool(SynchronousFindDelegate? synchronousFindFunc)
    {
        SynchronousFindFunc = synchronousFindFunc;
    }

    public Task<IArtifactData?> FindAsync(string id, CancellationToken cancellationToken = default)
    {
        if (SynchronousFindFunc != null)
        {
            return Task.FromResult(SynchronousFindFunc(this, id));
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

public class AsyncProgrammableArtifactFindTool : ArtifactTool, IArtifactFindTool
{
    public delegate Task<IArtifactData?> AsyncFindDelegate(AsyncProgrammableArtifactFindTool tool, string id);

    public readonly AsyncFindDelegate? AsyncFindFunc;

    public AsyncProgrammableArtifactFindTool(AsyncFindDelegate? asyncFindFunc)
    {
        AsyncFindFunc = asyncFindFunc;
    }

    public Task<IArtifactData?> FindAsync(string id, CancellationToken cancellationToken = default)
    {
        if (AsyncFindFunc != null)
        {
            return AsyncFindFunc(this, id);
        }
        throw new NotImplementedException();
    }

    public static ArtifactToolRegistryEntry CreateRegistryEntry(AsyncFindDelegate asyncFindDelegate)
    {
        return CreateRegistryEntry(ArtifactToolIDUtil.CreateToolId<AsyncProgrammableArtifactFindTool>(), asyncFindDelegate);
    }

    public static ArtifactToolRegistryEntry CreateRegistryEntry(ArtifactToolID artifactToolId, AsyncFindDelegate asyncFindDelegate)
    {
        return new CustomArtifactToolRegistryEntry(artifactToolId, asyncFindDelegate);
    }

    private record CustomArtifactToolRegistryEntry(ArtifactToolID Id, AsyncFindDelegate AsyncFindDelegate) : ArtifactToolRegistryEntry(Id)
    {
        public override IArtifactTool CreateArtifactTool() => new AsyncProgrammableArtifactFindTool(AsyncFindDelegate);

        public override Type GetArtifactToolType() => typeof(AsyncProgrammableArtifactFindTool);
    }
}
