using System.CommandLine;
using System.Diagnostics.CodeAnalysis;

namespace Art.Tesler.Tests;

public class ChecksumTests : CommandTestBase
{
    private const string ProfileName = "profile";

    protected ArcCommand? Command;

    [MemberNotNull(nameof(Command))]
    protected void InitCommandDefault(
        IToolLogHandlerProvider toolLogHandlerProvider,
        IArtifactToolRegistryStore artifactToolRegistryStore,
        IDefaultPropertyProvider defaultPropertyProvider,
        ITeslerDataProvider dataProvider,
        ITeslerRegistrationProvider registrationProvider,
        IProfileResolver profileResolver)
    {
        Command = new ArcCommand(toolLogHandlerProvider, artifactToolRegistryStore, defaultPropertyProvider, dataProvider, registrationProvider, profileResolver);
    }

    [Test]
    public void NoChecksumPassed_Success()
    {
        var store = GetEmptyStore();
        var defaultPropertyProvider = CreateInMemoryDefaultPropertyProvider();
        var dataProvider = CreateSharedMemoryDataProvider();
        var registrationProvider = CreateSharedMemoryRegistrationProvider();
        var profileResolver = CreateDictionaryProfileResolver(ProfileName);
        CreateOutputs(out var toolOutput, out var console);
        InitCommandDefault(toolOutput, store, defaultPropertyProvider, dataProvider, registrationProvider, profileResolver);
        string[] line = { ProfileName };
        Assert.That(Command.Invoke(line, console), Is.EqualTo(0));
        Assert.That(Out.ToString(), Is.Empty);
        Assert.That(Error.ToString(), Is.Empty);
    }

    [Test]
    public void KnownChecksumPassed_Success()
    {
        var store = GetEmptyStore();
        var defaultPropertyProvider = CreateInMemoryDefaultPropertyProvider();
        var dataProvider = CreateSharedMemoryDataProvider();
        var registrationProvider = CreateSharedMemoryRegistrationProvider();
        var profileResolver = CreateDictionaryProfileResolver(ProfileName);
        CreateOutputs(out var toolOutput, out var console);
        InitCommandDefault(toolOutput, store, defaultPropertyProvider, dataProvider, registrationProvider, profileResolver);
        string[] line = { ProfileName, "--hash", "SHA256" };
        Assert.That(Command.Invoke(line, console), Is.EqualTo(0));
        Assert.That(Out.ToString(), Is.Empty);
        Assert.That(Error.ToString(), Is.Empty);
    }

    [Test]
    public void BadChecksumPassed_Fails()
    {
        var store = GetEmptyStore();
        var defaultPropertyProvider = CreateInMemoryDefaultPropertyProvider();
        var dataProvider = CreateSharedMemoryDataProvider();
        var registrationProvider = CreateSharedMemoryRegistrationProvider();
        var profileResolver = CreateDictionaryProfileResolver(ProfileName);
        CreateOutputs(out var toolOutput, out var console);
        InitCommandDefault(toolOutput, store, defaultPropertyProvider, dataProvider, registrationProvider, profileResolver);
        string[] line = { ProfileName, "--hash", "BAD_CHECKSUM" };
        Assert.That(Command.Invoke(line, console), Is.Not.EqualTo(0));
        Assert.That(Out.ToString(), Is.Empty);
        Assert.That(Error.ToString(), Is.Not.Empty);
    }
}
