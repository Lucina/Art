using System.Diagnostics.CodeAnalysis;
using Art.Common;

namespace Art.Tesler.Tests;

public class CommandTestBase
{
    protected ITeslerRegistrationProvider? RegistrationProvider;
    protected ITeslerDataProvider? DataProvider;
    protected TestConsole? TestConsole;

    protected IArtifactToolRegistryStore GetEmptyStore() => new StaticArtifactToolRegistryStore(new ArtifactToolRegistry());

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
}
