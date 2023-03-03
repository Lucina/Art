using System.CommandLine;
using System.Diagnostics.CodeAnalysis;
using Art.Common;
using Art.TestsBase;

namespace Art.Tesler.Tests;

public class DumpCommandTests : CommandTestBase
{
    protected DumpCommand? Command;

    [MemberNotNull(nameof(Command))]
    protected void InitCommandDefault(
        IToolLogHandlerProvider toolLogHandlerProvider,
        IArtifactToolRegistryStore artifactToolRegistryStore,
        IDefaultPropertyProvider defaultPropertyProvider,
        ITeslerDataProvider dataProvider,
        ITeslerRegistrationProvider registrationProvider)
    {
        Command = new DumpCommand(toolLogHandlerProvider, artifactToolRegistryStore, defaultPropertyProvider, dataProvider, registrationProvider);
    }

    [Test]
    public void EmptyInvocation_Fails()
    {
        var store = GetSingleStore(ProgrammableArtifactDumpTool.CreateRegistryEntry(_ => { }));
        var defaultPropertyProvider = CreateInMemoryDefaultPropertyProvider();
        var dataProvider = CreateSharedMemoryDataProvider();
        var registrationProvider = CreateSharedMemoryRegistrationProvider();
        CreateOutputs(out var toolOutput, out var console);
        InitCommandDefault(toolOutput, store, defaultPropertyProvider, dataProvider, registrationProvider);
        Assert.That(Command.Invoke(Array.Empty<string>(), console), Is.Not.EqualTo(0));
        Assert.That(Out.ToString(), Is.Not.Empty);
        Assert.That(Error.ToString(), Is.Not.Empty);
    }

    [Test]
    public void MissingTool_Fails()
    {
        var store = GetSingleStore(ProgrammableArtifactDumpTool.CreateRegistryEntry(_ => { }));
        var defaultPropertyProvider = CreateInMemoryDefaultPropertyProvider();
        var dataProvider = CreateSharedMemoryDataProvider();
        var registrationProvider = CreateSharedMemoryRegistrationProvider();
        CreateOutputs(out var toolOutput, out var console);
        InitCommandDefault(toolOutput, store, defaultPropertyProvider, dataProvider, registrationProvider);
        string[] line = { "-t", new ArtifactToolID("NOT_AN_ASSEMBLY", "MALO").GetToolString() };
        Assert.That(Command.Invoke(line, console), Is.Not.EqualTo(0));
        Assert.That(Out.ToString(), Is.Empty);
        Assert.That(Error.ToString(), Is.Not.Empty);
    }

    [Test]
    public void NoopTool_Success()
    {
        var store = GetSingleStore(ProgrammableArtifactDumpTool.CreateRegistryEntry(_ => { }));
        var defaultPropertyProvider = CreateInMemoryDefaultPropertyProvider();
        var dataProvider = CreateSharedMemoryDataProvider();
        var registrationProvider = CreateSharedMemoryRegistrationProvider();
        CreateOutputs(out var toolOutput, out var console);
        InitCommandDefault(toolOutput, store, defaultPropertyProvider, dataProvider, registrationProvider);
        string[] line = { "-t", ArtifactToolIDUtil.CreateToolString<ProgrammableArtifactDumpTool>() };
        Assert.That(Command.Invoke(line, console), Is.EqualTo(0));
        Assert.That(Out.ToString(), Is.Empty);
        Assert.That(Error.ToString(), Is.Empty);
    }
}
