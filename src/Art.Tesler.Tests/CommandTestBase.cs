using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using Art.Common;

namespace Art.Tesler.Tests;

public class CommandTestBase
{
    protected StringWriter? Out;
    protected StringWriter? Error;
    protected IOutputPair? ToolOutput;
    protected TestConsole? TestConsole;
    protected IDefaultPropertyProvider? DefaultPropertyProvider;
    protected IToolLogHandlerProvider? ToolLogHandlerProvider;
    protected ITeslerRegistrationProvider? RegistrationProvider;
    protected ITeslerDataProvider? DataProvider;
    protected IProfileResolver? ProfileResolver;

    private class StringWriterOutputPair : IOutputPair
    {
        public TextWriter Out { get; }

        public TextWriter Error { get; }

        public StringWriterOutputPair(StringWriter outWriter, StringWriter errorWriter)
        {
            Out = outWriter;
            Error = errorWriter;
        }
    }

    [MemberNotNull(nameof(Out))]
    [MemberNotNull(nameof(Error))]
    [MemberNotNull(nameof(ToolOutput))]
    [MemberNotNull(nameof(TestConsole))]
    internal void CreateOutputs(out IOutputPair toolOutput, out TestConsole console, string? newLine=null, int windowWidth = 100, bool outputRedirected = true, bool errorRedirected = true, bool inputRedirected = true)
    {
        Out = new StringWriter();
        Error = new StringWriter();
        if (newLine != null)
        {
            Out.NewLine = newLine;
            Error.NewLine = newLine;
        }
        toolOutput = ToolOutput = new StringWriterOutputPair(Out, Error);
        console = TestConsole = new TestConsole(Out, Error, windowWidth, outputRedirected, errorRedirected, inputRedirected);
    }

    internal StaticArtifactToolRegistryStore GetEmptyStore() => new StaticArtifactToolRegistryStore(new ArtifactToolRegistry());

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

    [MemberNotNull(nameof(DefaultPropertyProvider))]
    internal IDefaultPropertyProvider CreateInMemoryDefaultPropertyProvider()
    {
        var result = new InMemoryDefaultPropertyProvider(ImmutableDictionary<string, JsonElement>.Empty, ImmutableDictionary<ArtifactToolID, IReadOnlyDictionary<string, JsonElement>>.Empty);
        DefaultPropertyProvider = result;
        return result;
    }

    [MemberNotNull(nameof(DefaultPropertyProvider))]
    internal InMemoryDefaultPropertyProvider CreateInMemoryDefaultPropertyProvider(IReadOnlyDictionary<string, JsonElement> shared)
    {
        var result = new InMemoryDefaultPropertyProvider(shared, ImmutableDictionary<ArtifactToolID, IReadOnlyDictionary<string, JsonElement>>.Empty);
        DefaultPropertyProvider = result;
        return result;
    }

    [MemberNotNull(nameof(DefaultPropertyProvider))]
    internal InMemoryDefaultPropertyProvider CreateInMemoryDefaultPropertyProvider(IReadOnlyDictionary<string, JsonElement> shared, IReadOnlyDictionary<ArtifactToolID, IReadOnlyDictionary<string, JsonElement>> perTool)
    {
        var result = new InMemoryDefaultPropertyProvider(shared, perTool);
        DefaultPropertyProvider = result;
        return result;
    }

    [MemberNotNull(nameof(ToolLogHandlerProvider))]
    internal StyledToolLogHandlerProvider CreateStyledToolLogHandlerProvider(char[]? newLine = null)
    {
        var result = new StyledToolLogHandlerProvider(newLine ?? Environment.NewLine.ToCharArray());
        ToolLogHandlerProvider = result;
        return result;
    }

    [MemberNotNull(nameof(ToolLogHandlerProvider))]
    internal PlainToolLogHandlerProvider CreatePlainToolLogHandlerProvider(char[]? newLine = null)
    {
        var result = new PlainToolLogHandlerProvider(newLine ?? Environment.NewLine.ToCharArray());
        ToolLogHandlerProvider = result;
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
