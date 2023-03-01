using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Art.Common;

namespace Art.Tesler.Tests;

public class CommandTestBase
{
    protected IDefaultPropertyProvider? DefaultPropertyProvider;
    protected ITeslerRegistrationProvider? RegistrationProvider;
    protected ITeslerDataProvider? DataProvider;
    protected TestConsole? TestConsole;

    protected IArtifactToolRegistryStore GetEmptyStore() => new StaticArtifactToolRegistryStore(new ArtifactToolRegistry());

    protected IArtifactToolRegistryStore GetSingleStore(ArtifactToolRegistryEntry artifactToolRegistryEntry)
    {
        var registry = new ArtifactToolRegistry();
        registry.Add(artifactToolRegistryEntry);
        return new StaticArtifactToolRegistryStore(registry);
    }

    protected IArtifactToolRegistryStore GetSingleStore<T>() where T : IArtifactToolFactory
    {
        var registry = new ArtifactToolRegistry();
        registry.Add<T>();
        return new StaticArtifactToolRegistryStore(registry);
    }

    protected IArtifactToolRegistryStore GetSingleSelectableStore<T>() where T : IArtifactToolFactory, IArtifactToolSelector<string>
    {
        var registry = new ArtifactToolRegistry();
        registry.AddSelectable<T>();
        return new StaticArtifactToolRegistryStore(registry);
    }

    [MemberNotNull("TestConsole")]
    protected TestConsole CreateConsole(int windowWidth = 100, bool outputRedirected = true, bool errorRedirected = true, bool inputRedirected = true)
    {
        return TestConsole = new TestConsole(windowWidth, outputRedirected, errorRedirected, inputRedirected);
    }

    [MemberNotNull("DefaultPropertyProvider")]
    protected IDefaultPropertyProvider CreateInMemoryDefaultPropertyProvider()
    {
        return DefaultPropertyProvider = new InMemoryDefaultPropertyProvider(ImmutableDictionary<string, JsonElement>.Empty, ImmutableDictionary<ArtifactToolID, IReadOnlyDictionary<string, JsonElement>>.Empty);
    }

    [MemberNotNull("DefaultPropertyProvider")]
    protected IDefaultPropertyProvider CreateInMemoryDefaultPropertyProvider(IReadOnlyDictionary<string, JsonElement> shared)
    {
        return DefaultPropertyProvider = new InMemoryDefaultPropertyProvider(shared, ImmutableDictionary<ArtifactToolID, IReadOnlyDictionary<string, JsonElement>>.Empty);
    }

    [MemberNotNull("DefaultPropertyProvider")]
    protected IDefaultPropertyProvider CreateInMemoryDefaultPropertyProvider(IReadOnlyDictionary<string, JsonElement> shared, IReadOnlyDictionary<ArtifactToolID, IReadOnlyDictionary<string, JsonElement>> perTool)
    {
        return DefaultPropertyProvider = new InMemoryDefaultPropertyProvider(shared, perTool);
    }

    [MemberNotNull("DataProvider")]
    protected ITeslerDataProvider CreateSharedMemoryDataProvider()
    {
        return DataProvider = new SharedMemoryDataProvider();
    }

    [MemberNotNull("RegistrationProvider")]
    protected ITeslerRegistrationProvider CreateSharedMemoryRegistrationProvider()
    {
        return RegistrationProvider = new SharedMemoryRegistrationProvider();
    }
}
