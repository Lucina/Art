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
        ITeslerRegistrationProvider registrationProvider)
    {
        Command = new ArcCommand(artifactToolRegistryStore, defaultPropertyProvider, toolLogHandlerProvider, dataProvider, registrationProvider);
    }

    // TODO command should get abstraction for input file processing / proxy to registry as well...
}
