using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Art.Common;
using Art.Common.IO;

namespace Art.Tesler.Tests;

public class CommandTestBase
{
    protected StringWriter? Out;
    protected MemoryStream? OutStream;
    protected Queue<ObjectLog>? OutQueue;
    protected StringWriter? Error;
    protected Queue<ObjectLog>? ErrorQueue;
    protected IOutputPair? ToolOutput;
    protected TestConsole? TestConsole;
    protected IToolDefaultPropertyProvider? ToolDefaultPropertyProvider;
    protected ITeslerRegistrationProvider? RegistrationProvider;
    protected ITeslerDataProvider? DataProvider;
    protected IProfileResolver? ProfileResolver;

    [MemberNotNull(nameof(Out))]
    [MemberNotNull(nameof(OutStream))]
    [MemberNotNull(nameof(Error))]
    [MemberNotNull(nameof(ToolOutput))]
    [MemberNotNull(nameof(TestConsole))]
    internal void CreateStreamOutputs(out PlainToolLogHandlerProvider toolLogHandlerProvider, out TestConsole console, string? newLine = null, int windowWidth = 100, bool outputRedirected = true, bool errorRedirected = true, bool inputRedirected = true)
    {
        Out = new StringWriter();
        OutStream = new MemoryStream();
        Error = new StringWriter();
        if (newLine != null)
        {
            Out.NewLine = newLine;
            Error.NewLine = newLine;
        }
        OutStream = new MemoryStream();
        ToolOutput = toolLogHandlerProvider = new PlainToolLogHandlerProvider(Out, Error, () => new NonDisposingStream(OutStream));
        console = TestConsole = new TestConsole(Out, Error, windowWidth, outputRedirected, errorRedirected, inputRedirected);
    }

    [MemberNotNull(nameof(Out))]
    [MemberNotNull(nameof(Error))]
    [MemberNotNull(nameof(ToolOutput))]
    [MemberNotNull(nameof(TestConsole))]
    internal void CreateOutputs(out PlainToolLogHandlerProvider toolLogHandlerProvider, out TestConsole console, string? newLine = null, int windowWidth = 100, bool outputRedirected = true, bool errorRedirected = true, bool inputRedirected = true)
    {
        Out = new StringWriter();
        Error = new StringWriter();
        if (newLine != null)
        {
            Out.NewLine = newLine;
            Error.NewLine = newLine;
        }
        ToolOutput = toolLogHandlerProvider = new PlainToolLogHandlerProvider(Out, Error, () => throw new NotSupportedException());
        console = TestConsole = new TestConsole(Out, Error, windowWidth, outputRedirected, errorRedirected, inputRedirected);
    }

    [MemberNotNull(nameof(Out))]
    [MemberNotNull(nameof(OutQueue))]
    [MemberNotNull(nameof(Error))]
    [MemberNotNull(nameof(ErrorQueue))]
    [MemberNotNull(nameof(ToolOutput))]
    [MemberNotNull(nameof(TestConsole))]
    internal void CreateObjectOutputs(out ObjectToolLogHandlerProvider toolLogHandlerProvider, out TestConsole console, string? newLine = null, int windowWidth = 100, bool outputRedirected = true, bool errorRedirected = true, bool inputRedirected = true)
    {
        Out = new StringWriter();
        Error = new StringWriter();
        if (newLine != null)
        {
            Out.NewLine = newLine;
            Error.NewLine = newLine;
        }
        OutQueue = new Queue<ObjectLog>();
        ErrorQueue = new Queue<ObjectLog>();
        ToolOutput = toolLogHandlerProvider = new ObjectToolLogHandlerProvider(Out, Error, () => throw new NotSupportedException(), OutQueue, ErrorQueue);
        console = TestConsole = new TestConsole(Out, Error, windowWidth, outputRedirected, errorRedirected, inputRedirected);
    }

    [MemberNotNull(nameof(Error))]
    [MemberNotNull(nameof(ToolOutput))]
    [MemberNotNull(nameof(TestConsole))]
    internal void CreateOutputs(out PlainToolLogHandlerProvider toolLogHandlerProvider, out TestConsole console, Stream outStream, string? newLine = null, int windowWidth = 100, bool outputRedirected = true, bool errorRedirected = true, bool inputRedirected = true)
    {
        var outWriter = new StreamWriter(outStream, leaveOpen: true);
        Error = new StringWriter();
        if (newLine != null)
        {
            outWriter.NewLine = newLine;
            Error.NewLine = newLine;
        }
        ToolOutput = toolLogHandlerProvider = new PlainToolLogHandlerProvider(outWriter, Error, () => new NonDisposingStream(outStream));
        console = TestConsole = new TestConsole(outWriter, Error, windowWidth, outputRedirected, errorRedirected, inputRedirected);
    }

    private class NonDisposingStream : DelegatingStream
    {
        public NonDisposingStream(Stream innerStream) : base(innerStream)
        {
        }

        protected override void Dispose(bool disposing)
        {
        }

        public override ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }

    internal StaticArtifactToolRegistryStore GetEmptyStore() => new(new ArtifactToolRegistry());

    internal StaticArtifactToolRegistryStore GetSingleStore(ArtifactToolRegistryEntry artifactToolRegistryEntry)
    {
        var registry = new ArtifactToolRegistry();
        registry.Add(artifactToolRegistryEntry);
        return new StaticArtifactToolRegistryStore(registry);
    }

    internal StaticArtifactToolRegistryStore GetSingleStore<T>() where T : IArtifactToolFactory
    {
        var registry = new ArtifactToolRegistry();
        registry.Add<T>();
        return new StaticArtifactToolRegistryStore(registry);
    }

    internal StaticArtifactToolRegistryStore GetSingleSelectableStore<T>() where T : IArtifactToolFactory, IArtifactToolSelector<string>
    {
        var registry = new ArtifactToolRegistry();
        registry.AddSelectable<T>();
        return new StaticArtifactToolRegistryStore(registry);
    }

    [MemberNotNull(nameof(ToolDefaultPropertyProvider))]
    internal InMemoryToolDefaultPropertyProvider CreateInMemoryDefaultPropertyProvider()
    {
        var result = new InMemoryToolDefaultPropertyProvider(ImmutableDictionary<string, JsonElement>.Empty, ImmutableDictionary<ArtifactToolID, IReadOnlyDictionary<string, JsonElement>>.Empty);
        ToolDefaultPropertyProvider = result;
        return result;
    }

    [MemberNotNull(nameof(ToolDefaultPropertyProvider))]
    internal InMemoryToolDefaultPropertyProvider CreateInMemoryDefaultPropertyProvider(IReadOnlyDictionary<string, JsonElement> shared)
    {
        var result = new InMemoryToolDefaultPropertyProvider(shared, ImmutableDictionary<ArtifactToolID, IReadOnlyDictionary<string, JsonElement>>.Empty);
        ToolDefaultPropertyProvider = result;
        return result;
    }

    [MemberNotNull(nameof(ToolDefaultPropertyProvider))]
    internal InMemoryToolDefaultPropertyProvider CreateInMemoryDefaultPropertyProvider(IReadOnlyDictionary<string, JsonElement> shared, IReadOnlyDictionary<ArtifactToolID, IReadOnlyDictionary<string, JsonElement>> perTool)
    {
        var result = new InMemoryToolDefaultPropertyProvider(shared, perTool);
        ToolDefaultPropertyProvider = result;
        return result;
    }

    [MemberNotNull(nameof(DataProvider))]
    internal SharedMemoryDataProvider CreateSharedMemoryDataProvider()
    {
        var result = new SharedMemoryDataProvider();
        DataProvider = result;
        return result;
    }

    [MemberNotNull(nameof(RegistrationProvider))]
    internal SharedMemoryRegistrationProvider CreateSharedMemoryRegistrationProvider()
    {
        var result = new SharedMemoryRegistrationProvider();
        RegistrationProvider = result;
        return result;
    }

    [MemberNotNull(nameof(ProfileResolver))]
    internal DictionaryProfileResolver CreateDictionaryProfileResolver()
    {
        var result = new DictionaryProfileResolver(ImmutableDictionary<string, IReadOnlyCollection<ArtifactToolProfile>>.Empty);
        ProfileResolver = result;
        return result;
    }

    [MemberNotNull(nameof(ProfileResolver))]
    internal DictionaryProfileResolver CreateDictionaryProfileResolver(IReadOnlyDictionary<string, IReadOnlyCollection<ArtifactToolProfile>> map)
    {
        var result = new DictionaryProfileResolver(map);
        ProfileResolver = result;
        return result;
    }

    [MemberNotNull(nameof(ProfileResolver))]
    internal DictionaryProfileResolver CreateDictionaryProfileResolver(string profileName, params ArtifactToolProfile[] profiles)
    {
        return CreateDictionaryProfileResolver(new Dictionary<string, IReadOnlyCollection<ArtifactToolProfile>> { [profileName] = profiles });
    }
}
