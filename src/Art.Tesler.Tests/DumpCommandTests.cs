using System;
using System.CommandLine;
using System.Diagnostics.CodeAnalysis;
using Art.Common;

namespace Art.Tesler.Tests;

public class DumpCommandTests : CommandTestBase
{
    protected DumpCommand? Command;

    [MemberNotNull("Command")]
    protected void InitCommandDefault(
        IArtifactToolRegistryStore artifactToolRegistryStore,
        IDefaultPropertyProvider defaultPropertyProvider,
        IToolLogHandlerProvider toolLogHandlerProvider,
        ITeslerDataProvider dataProvider,
        ITeslerRegistrationProvider registrationProvider)
    {
        Command = new DumpCommand(artifactToolRegistryStore, defaultPropertyProvider, toolLogHandlerProvider, dataProvider, registrationProvider);
    }

    [Test]
    public void EmptyInvocation_Fails()
    {
        var store = GetSingleStore(ProgrammableArtifactDumpTool.CreateRegistryEntry(_ => { }));
        var defaultPropertyProvider = CreateInMemoryDefaultPropertyProvider();
        var toolLogHandlerProvider = CreateStyledToolLogHandlerProvider();
        var dataProvider = CreateSharedMemoryDataProvider();
        var registrationProvider = CreateSharedMemoryRegistrationProvider();
        InitCommandDefault(store, defaultPropertyProvider, toolLogHandlerProvider, dataProvider, registrationProvider);
        var console = CreateConsole();
        Assert.That(Command.Invoke(Array.Empty<string>(), console), Is.Not.EqualTo(0));
        Assert.That(console.StringOut.StringWriter.ToString(), Is.Not.Empty);
        Assert.That(console.StringError.StringWriter.ToString(), Is.Not.Empty);
    }

    [Test]
    public void Dump_MissingTool_Fails()
    {
        var store = GetSingleStore(ProgrammableArtifactDumpTool.CreateRegistryEntry(_ => { }));
        var defaultPropertyProvider = CreateInMemoryDefaultPropertyProvider();
        var toolLogHandlerProvider = CreateStyledToolLogHandlerProvider();
        var dataProvider = CreateSharedMemoryDataProvider();
        var registrationProvider = CreateSharedMemoryRegistrationProvider();
        InitCommandDefault(store, defaultPropertyProvider, toolLogHandlerProvider, dataProvider, registrationProvider);
        var console = CreateConsole();
        string[] line = { "-t", new ArtifactToolID("NOT_AN_ASSEMBLY", "MALO").GetToolString() };
        Assert.That(Command.Invoke(line, console), Is.Not.EqualTo(0));
        Assert.That(console.StringOut.StringWriter.ToString(), Is.Empty);
        Assert.That(console.StringError.StringWriter.ToString(), Is.Not.Empty);
    }

    [Test]
    public void Dump_NoopTool_Success()
    {
        var store = GetSingleStore(ProgrammableArtifactDumpTool.CreateRegistryEntry(_ => { }));
        var defaultPropertyProvider = CreateInMemoryDefaultPropertyProvider();
        var toolLogHandlerProvider = CreateStyledToolLogHandlerProvider();
        var dataProvider = CreateSharedMemoryDataProvider();
        var registrationProvider = CreateSharedMemoryRegistrationProvider();
        InitCommandDefault(store, defaultPropertyProvider, toolLogHandlerProvider, dataProvider, registrationProvider);
        var console = CreateConsole();
        string[] line = { "-t", ArtifactToolIDUtil.CreateToolString<ProgrammableArtifactDumpTool>() };
        Assert.That(Command.Invoke(line, console), Is.EqualTo(0));
        Assert.That(console.StringOut.StringWriter.ToString(), Is.Empty);
        Assert.That(console.StringError.StringWriter.ToString(), Is.Empty);
    }
}
