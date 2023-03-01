using System;
using System.CommandLine;
using System.Diagnostics.CodeAnalysis;

namespace Art.Tesler.Tests;

public class ArcCommandTests : CommandTestBase
{
    protected ArcCommand? Command;
    private const string ProfileName = "profile";
    private const string BadProfileName = "profile_unknown";
    private static readonly ArtifactToolID s_toolId = new("Disease", "https://www.youtube.com/watch?v=GZG_HKfIz0U");
    private static readonly ArtifactToolID s_badToolId = new("BadApple", "https://www.nicovideo.jp/watch/sm8628149");

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
    public void NoProfilesPassed_Fails()
    {
        var store = GetEmptyStore();
        var defaultPropertyProvider = CreateInMemoryDefaultPropertyProvider();
        var toolLogHandlerProvider = CreateStyledToolLogHandlerProvider();
        var dataProvider = CreateSharedMemoryDataProvider();
        var registrationProvider = CreateSharedMemoryRegistrationProvider();
        var profileResolver = CreateDictionaryProfileResolver();
        InitCommandDefault(store, defaultPropertyProvider, toolLogHandlerProvider, dataProvider, registrationProvider, profileResolver);
        var console = CreateConsole();
        Assert.That(Command.Invoke(Array.Empty<string>(), console), Is.Not.EqualTo(0));
        Assert.That(console.StringOut.StringWriter.ToString(), Is.Not.Empty);
        Assert.That(console.StringError.StringWriter.ToString(), Is.Not.Empty);
    }

    [Test]
    public void OneItemPassed_Unmatched_Fails()
    {
        var store = GetEmptyStore();
        var defaultPropertyProvider = CreateInMemoryDefaultPropertyProvider();
        var toolLogHandlerProvider = CreateStyledToolLogHandlerProvider();
        var dataProvider = CreateSharedMemoryDataProvider();
        var registrationProvider = CreateSharedMemoryRegistrationProvider();
        var profileResolver = CreateDictionaryProfileResolver();
        InitCommandDefault(store, defaultPropertyProvider, toolLogHandlerProvider, dataProvider, registrationProvider, profileResolver);
        var console = CreateConsole();
        string[] line = { BadProfileName };
        Assert.That(Command.Invoke(line, console), Is.Not.EqualTo(0));
        Assert.That(console.StringOut.StringWriter.ToString(), Is.Empty);
        Assert.That(console.StringError.StringWriter.ToString(), Is.Not.Empty);
    }

    [Test]
    public void OneItemPassed_AllMatch_WithNoResultingProfiles_Success()
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
    public void OneItemPassed_AllMatch_WithResultingProfiles_ProfilesValid_Success()
    {
        int ctr = 0;
        var store = GetSingleStore(ProgrammableArtifactDumpTool.CreateRegistryEntry(s_toolId, _ => ctr++));
        var defaultPropertyProvider = CreateInMemoryDefaultPropertyProvider();
        var toolLogHandlerProvider = CreateStyledToolLogHandlerProvider();
        var dataProvider = CreateSharedMemoryDataProvider();
        var registrationProvider = CreateSharedMemoryRegistrationProvider();
        var profileResolver = CreateDictionaryProfileResolver(ProfileName, new ArtifactToolProfile(s_toolId.GetToolString(), null, null));
        InitCommandDefault(store, defaultPropertyProvider, toolLogHandlerProvider, dataProvider, registrationProvider, profileResolver);
        var console = CreateConsole();
        string[] line = { ProfileName };
        Assert.That(Command.Invoke(line, console), Is.EqualTo(0));
        Assert.That(console.StringOut.StringWriter.ToString(), Is.Empty);
        Assert.That(console.StringError.StringWriter.ToString(), Is.Empty);
        Assert.That(ctr, Is.EqualTo(1));
    }

    [Test]
    public void OneItemPassed_AllMatch_WithResultingProfiles_ProfilesInvalid_Fails()
    {
        int ctr = 0;
        var store = GetSingleStore(ProgrammableArtifactDumpTool.CreateRegistryEntry(s_toolId, _ => ctr++));
        var defaultPropertyProvider = CreateInMemoryDefaultPropertyProvider();
        var toolLogHandlerProvider = CreateStyledToolLogHandlerProvider();
        var dataProvider = CreateSharedMemoryDataProvider();
        var registrationProvider = CreateSharedMemoryRegistrationProvider();
        var profileResolver = CreateDictionaryProfileResolver(ProfileName, new ArtifactToolProfile(s_badToolId.GetToolString(), null, null));
        InitCommandDefault(store, defaultPropertyProvider, toolLogHandlerProvider, dataProvider, registrationProvider, profileResolver);
        var console = CreateConsole();
        string[] line = { ProfileName };
        Assert.That(Command.Invoke(line, console), Is.Not.EqualTo(0));
        Assert.That(console.StringOut.StringWriter.ToString(), Is.Empty);
        Assert.That(console.StringError.StringWriter.ToString(), Is.Not.Empty);
        Assert.That(ctr, Is.EqualTo(0));
    }
}
