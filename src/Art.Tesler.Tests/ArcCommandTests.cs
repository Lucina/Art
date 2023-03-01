using System.Diagnostics.CodeAnalysis;

namespace Art.Tesler.Tests;

public class ArcCommandTests : CommandTestBase
{
    protected ArcCommand? Command;

    [MemberNotNull("Command")]
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
}
