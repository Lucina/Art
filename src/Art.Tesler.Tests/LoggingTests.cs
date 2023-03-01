using System;
using System.CommandLine;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Art.Common;
using Art.Common.Logging;

namespace Art.Tesler.Tests;

public class LoggingTests : CommandTestBase
{
    protected DumpCommand? Command;

    private const string Message = "message_here";
    private const string Group = "group_here";
    private const string OutputDelimiter = "🥔";

    [MemberNotNull(nameof(Command))]
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
    public void Dump_LogInfoTool_OutputMatches()
    {
        var toolLogHandlerProvider = CreatePlainToolLogHandlerProvider(OutputDelimiter.ToCharArray());
        var console = CreateConsole();
        string toolString = ArtifactToolIDUtil.CreateToolString<ProgrammableArtifactDumpTool>();
        int code = Execute(console, toolLogHandlerProvider, t => t.LogInformation(Message), new[] { "-t", toolString, "-g", Group });
        Assert.That(code, Is.EqualTo(0));
        string outputContent = console.StringOut.StringWriter.ToString();
        Assert.That(outputContent, Is.Not.Empty);
        Assert.That(outputContent, Is.EqualTo(ConstructOutput(OutputDelimiter, toolString, Group, Message, null, LogLevel.Information)));
        Assert.That(console.StringError.StringWriter.ToString(), Is.Empty);
    }

    [Test]
    public void Dump_LogTitleTool_OutputMatches()
    {
        var toolLogHandlerProvider = CreatePlainToolLogHandlerProvider(OutputDelimiter.ToCharArray());
        var console = CreateConsole();
        string toolString = ArtifactToolIDUtil.CreateToolString<ProgrammableArtifactDumpTool>();
        int code = Execute(console, toolLogHandlerProvider, t => t.LogTitle(Message), new[] { "-t", toolString, "-g", Group });
        Assert.That(code, Is.EqualTo(0));
        string outputContent = console.StringOut.StringWriter.ToString();
        Assert.That(outputContent, Is.Not.Empty);
        Assert.That(outputContent, Is.EqualTo(ConstructOutput(OutputDelimiter, toolString, Group, Message, null, LogLevel.Title)));
        Assert.That(console.StringError.StringWriter.ToString(), Is.Empty);
    }

    [Test]
    public void Dump_LogEntryTool_OutputMatches()
    {
        var toolLogHandlerProvider = CreatePlainToolLogHandlerProvider(OutputDelimiter.ToCharArray());
        var console = CreateConsole();
        string toolString = ArtifactToolIDUtil.CreateToolString<ProgrammableArtifactDumpTool>();
        int code = Execute(console, toolLogHandlerProvider, t => t.LogEntry(Message), new[] { "-t", toolString, "-g", Group });
        Assert.That(code, Is.EqualTo(0));
        string outputContent = console.StringOut.StringWriter.ToString();
        Assert.That(outputContent, Is.Not.Empty);
        Assert.That(outputContent, Is.EqualTo(ConstructOutput(OutputDelimiter, toolString, Group, Message, null, LogLevel.Entry)));
        Assert.That(console.StringError.StringWriter.ToString(), Is.Empty);
    }

    [Test]
    public void Dump_LogWarningTool_OutputMatches()
    {
        var toolLogHandlerProvider = CreatePlainToolLogHandlerProvider(OutputDelimiter.ToCharArray());
        var console = CreateConsole();
        string toolString = ArtifactToolIDUtil.CreateToolString<ProgrammableArtifactDumpTool>();
        int code = Execute(console, toolLogHandlerProvider, t => t.LogWarning(Message), new[] { "-t", toolString, "-g", Group });
        Assert.That(code, Is.EqualTo(0));
        Assert.That(console.StringOut.StringWriter.ToString(), Is.Empty);
        string errorContent = console.StringError.StringWriter.ToString();
        Assert.That(errorContent, Is.Not.Empty);
        Assert.That(errorContent, Is.EqualTo(ConstructErrorOutput(OutputDelimiter, toolString, Group, Message, null, LogLevel.Warning)));
    }

    [Test]
    public void Dump_LogErrorTool_OutputMatches()
    {
        var toolLogHandlerProvider = CreatePlainToolLogHandlerProvider(OutputDelimiter.ToCharArray());
        var console = CreateConsole();
        string toolString = ArtifactToolIDUtil.CreateToolString<ProgrammableArtifactDumpTool>();
        int code = Execute(console, toolLogHandlerProvider, t => t.LogError(Message), new[] { "-t", toolString, "-g", Group });
        Assert.That(code, Is.EqualTo(0));
        Assert.That(console.StringOut.StringWriter.ToString(), Is.Empty);
        string errorContent = console.StringError.StringWriter.ToString();
        Assert.That(errorContent, Is.Not.Empty);
        Assert.That(errorContent, Is.EqualTo(ConstructErrorOutput(OutputDelimiter, toolString, Group, Message, null, LogLevel.Error)));
    }

    private int Execute(IConsole console, IToolLogHandlerProvider toolLogHandlerProvider, Action<ProgrammableArtifactDumpTool> action, string[] line)
    {
        var store = GetSingleStore(ProgrammableArtifactDumpTool.CreateRegistryEntry(t => action(t)));
        var defaultPropertyProvider = CreateInMemoryDefaultPropertyProvider();
        var dataProvider = CreateSharedMemoryDataProvider();
        var registrationProvider = CreateSharedMemoryRegistrationProvider();
        InitCommandDefault(store, defaultPropertyProvider, toolLogHandlerProvider, dataProvider, registrationProvider);
        return Command.Invoke(line, console);
    }

    private static string ConstructOutput(string outputDelimiter, string toolString, string group, string? title, string? body, LogLevel logLevel)
    {
        var expectedOutput = new StringWriter { NewLine = outputDelimiter };
        var expectedOutputHandler = new PlainLogHandler(expectedOutput, TextWriter.Null, false);
        expectedOutputHandler.Log(toolString, group, title, body, logLevel);
        return expectedOutput.ToString();
    }

    private static string ConstructErrorOutput(string outputDelimiter, string toolString, string group, string? title, string? body, LogLevel logLevel)
    {
        var expectedErrorOutput = new StringWriter { NewLine = outputDelimiter };
        var expectedErrorOutputHandler = new PlainLogHandler(TextWriter.Null, expectedErrorOutput, false);
        expectedErrorOutputHandler.Log(toolString, group, title, body, logLevel);
        return expectedErrorOutput.ToString();
    }
}
