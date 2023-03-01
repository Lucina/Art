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
        ITeslerDataProvider dataProvider,
        ITeslerRegistrationProvider registrationProvider)
    {
        Command = new DumpCommand(artifactToolRegistryStore, defaultPropertyProvider, dataProvider, registrationProvider);
    }

    [Test]
    public void EmptyInvocation_Fails()
    {
        var store = GetSingleStore(ProgrammableArtifactDumpTool.CreateRegistryEntry(_ => { }));
        InitCommandDefault(store, CreateInMemoryDefaultPropertyProvider(), CreateSharedMemoryDataProvider(), CreateSharedMemoryRegistrationProvider());
        var console = CreateConsole();
        Assert.That(Command.Invoke(Array.Empty<string>(), console), Is.Not.EqualTo(0));
        Assert.That(console.StringOut.StringWriter.ToString(), Is.Not.Empty);
        Assert.That(console.StringError.StringWriter.ToString(), Is.Not.Empty);
    }

    [Test]
    public void Dump_MissingTool_Fails()
    {
        var store = GetSingleStore(ProgrammableArtifactDumpTool.CreateRegistryEntry(_ => { }));
        InitCommandDefault(store, CreateInMemoryDefaultPropertyProvider(), CreateSharedMemoryDataProvider(), CreateSharedMemoryRegistrationProvider());
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
        InitCommandDefault(store, CreateInMemoryDefaultPropertyProvider(), CreateSharedMemoryDataProvider(), CreateSharedMemoryRegistrationProvider());
        var console = CreateConsole();
        string[] line = { "-t", ArtifactToolIDUtil.CreateToolString<ProgrammableArtifactDumpTool>() };
        Assert.That(Command.Invoke(line, console), Is.EqualTo(0));
        Assert.That(console.StringOut.StringWriter.ToString(), Is.Empty);
        Assert.That(console.StringError.StringWriter.ToString(), Is.Empty);
    }

    [Test]
    public void Dump_LogInfoTool_OutputMatches()
    {
        var store = GetSingleStore(ProgrammableArtifactDumpTool.CreateRegistryEntry(t => { t.LogInformation("info"); }));
        InitCommandDefault(store, CreateInMemoryDefaultPropertyProvider(), CreateSharedMemoryDataProvider(), CreateSharedMemoryRegistrationProvider());
        var console = CreateConsole();
        string[] line = { "-t", ArtifactToolIDUtil.CreateToolString<ProgrammableArtifactDumpTool>() };
        Assert.That(Command.Invoke(line, console), Is.EqualTo(0));
        // TODO match exact via output fmt check
        Assert.That(console.StringOut.StringWriter.ToString(), Is.Not.Empty);
        Assert.That(console.StringError.StringWriter.ToString(), Is.Empty);
    }

    [Test]
    public void Dump_LogErrorTool_OutputMatches()
    {
        var store = GetSingleStore(ProgrammableArtifactDumpTool.CreateRegistryEntry(t => { t.LogError("error"); }));
        InitCommandDefault(store, CreateInMemoryDefaultPropertyProvider(), CreateSharedMemoryDataProvider(), CreateSharedMemoryRegistrationProvider());
        var console = CreateConsole();
        string[] line = { "-t", ArtifactToolIDUtil.CreateToolString<ProgrammableArtifactDumpTool>() };
        Assert.That(Command.Invoke(line, console), Is.EqualTo(0));
        Assert.That(console.StringOut.StringWriter.ToString(), Is.Empty);
        // TODO match exact via output fmt check
        Assert.That(console.StringError.StringWriter.ToString(), Is.Not.Empty);
    }

    // TODO
}
