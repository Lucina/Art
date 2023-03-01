using System.CommandLine;
using System.CommandLine.Invocation;
using Art.Common.Management;

namespace Art.Tesler;

public class DiskTeslerDataProvider : ITeslerDataProvider
{
    protected Option<string> OutputOption;

    public DiskTeslerDataProvider()
    {
        OutputOption = new Option<string>(new[] { "-o", "--output" }, "Output directory") { ArgumentHelpName = "directory" };
        OutputOption.SetDefaultValue(Directory.GetCurrentDirectory());
    }

    public DiskTeslerDataProvider(Option<string> outputOption)
    {
        OutputOption = outputOption;
    }

    public void Initialize(Command command)
    {
        command.AddOption(OutputOption);
    }

    public IArtifactDataManager CreateArtifactDataManager(InvocationContext context)
    {
        return new DiskArtifactDataManager(context.ParseResult.GetValueForOption(OutputOption)!);
    }
}
