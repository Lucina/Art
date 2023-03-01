using System.CommandLine;
using System.Diagnostics.CodeAnalysis;

namespace Art.Tesler.Tests;

public class ChecksumTests : CommandTestBase
{
    private const string ProfileName = "profile";

    protected ArcCommand? Command;

    [MemberNotNull(nameof(Command))]
    protected void InitCommandDefault(
        IArtifactToolRegistryStore artifactToolRegistryStore,
        IDefaultPropertyProvider defaultPropertyProvider,
        IToolLogHandlerProvider toolLogHandlerProvider,
        ITeslerDataProvider dataProvider,
        ITeslerRegistrationProvider registrationProvider,
        IProfileResolver profileResolver)
    {
        Command = new ArcCommand(artifactToolRegistryStore, defaultPropertyProvider, toolLogHandlerProvider, dataProvider, registrationProvider, profileResolver);
    }

    [Test]
    public void NoChecksumPassed_Success()
    {
        var store = GetEmptyStore();
        var defaultPropertyProvider = CreateInMemoryDefaultPropertyProvider();
        var toolLogHandlerProvider = CreateStyledToolLogHandlerProvider();
        var dataProvider = CreateSharedMemoryDataProvider();
        var registrationProvider = CreateSharedMemoryRegistrationProvider();
        var profileResolver = CreateDictionaryProfileResolver(ProfileName);
        InitCommandDefault(store, defaultPropertyProvider, toolLogHandlerProvider, dataProvider, registrationProvider, profileResolver);
        var console = CreateConsole();
        string[] line = { ProfileName };
        Assert.That(Command.Invoke(line, console), Is.EqualTo(0));
        Assert.That(console.StringOut.StringWriter.ToString(), Is.Empty);
        Assert.That(console.StringError.StringWriter.ToString(), Is.Empty);
    }

    [Test]
    public void KnownChecksumPassed_Success()
    {
        var store = GetEmptyStore();
        var defaultPropertyProvider = CreateInMemoryDefaultPropertyProvider();
        var toolLogHandlerProvider = CreateStyledToolLogHandlerProvider();
        var dataProvider = CreateSharedMemoryDataProvider();
        var registrationProvider = CreateSharedMemoryRegistrationProvider();
        var profileResolver = CreateDictionaryProfileResolver(ProfileName);
        InitCommandDefault(store, defaultPropertyProvider, toolLogHandlerProvider, dataProvider, registrationProvider, profileResolver);
        var console = CreateConsole();
        string[] line = { ProfileName, "--hash", "SHA256" };
        Assert.That(Command.Invoke(line, console), Is.EqualTo(0));
        Assert.That(console.StringOut.StringWriter.ToString(), Is.Empty);
        Assert.That(console.StringError.StringWriter.ToString(), Is.Empty);
    }

    [Test]
    public void BadChecksumPassed_Fails()
    {
        var store = GetEmptyStore();
        var defaultPropertyProvider = CreateInMemoryDefaultPropertyProvider();
        var toolLogHandlerProvider = CreateStyledToolLogHandlerProvider();
        var dataProvider = CreateSharedMemoryDataProvider();
        var registrationProvider = CreateSharedMemoryRegistrationProvider();
        var profileResolver = CreateDictionaryProfileResolver(ProfileName);
        InitCommandDefault(store, defaultPropertyProvider, toolLogHandlerProvider, dataProvider, registrationProvider, profileResolver);
        var console = CreateConsole();
        string[] line = { ProfileName, "--hash", "BAD_CHECKSUM" };
        Assert.That(Command.Invoke(line, console), Is.Not.EqualTo(0));
        Assert.That(console.StringOut.StringWriter.ToString(), Is.Empty);
        Assert.That(console.StringError.StringWriter.ToString(), Is.Not.Empty);
    }
}
