using System;
using System.CommandLine;
using System.Diagnostics.CodeAnalysis;

namespace Art.Tesler.Tests;

public class ToolsCommandTests : CommandTestBase
{
    protected ToolsCommand? Command;

    [MemberNotNull("Command")]
    protected void InitCommandDefault(IArtifactToolRegistryStore artifactToolRegistryStore)
    {
        Command = new ToolsCommand(artifactToolRegistryStore);
    }

    [Test]
    public void DefaultExecution_Empty_Success()
    {
        InitCommandDefault(GetEmptyStore());
        var console = CreateConsole();
        Assert.That(Command.Invoke(Array.Empty<string>(), console), Is.EqualTo(0));
        Assert.That(console.StringOut.StringWriter.ToString(), Is.Empty);
        Assert.That(console.StringError.StringWriter.ToString(), Is.Empty);
    }

    [Test]
    public void DefaultExecution_Single_Success()
    {
        InitCommandDefault(GetSingleStore<ExampleArtifactTool>());
        var console = CreateConsole();
        Assert.That(Command.Invoke(Array.Empty<string>(), console), Is.EqualTo(0));
        Assert.That(console.StringOut.StringWriter.ToString(), Contains.Substring(nameof(ExampleArtifactTool)));
        Assert.That(console.StringError.StringWriter.ToString(), Is.Empty);
    }
}
