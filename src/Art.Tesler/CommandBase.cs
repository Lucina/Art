using System.CommandLine;
using System.CommandLine.Invocation;

namespace Art.Tesler;

public abstract class CommandBase : Command
{
    protected IOutputPair ToolOutput;

    protected CommandBase(IOutputPair toolOutput, string name, string? description = null) : base(name, description)
    {
        ToolOutput = toolOutput;
        this.SetHandler(RunInternalAsync);
    }

    private async Task<int> RunInternalAsync(InvocationContext context)
    {
        try
        {
            return await RunAsync(context);
        }
        catch (ArtUserException e)
        {
            PrintExceptionMessage(e, ToolOutput);
            return -1;
        }
    }

    protected static void PrintExceptionMessage(Exception e, IOutputPair console)
    {
        PrintErrorMessage(e.Message, console);
    }

    protected static void PrintErrorMessage(string message, IOutputPair console)
    {
        console.Error.WriteLine(message);
    }

    protected static void PrintWarningMessage(string message, IOutputPair console)
    {
        console.Error.WriteLine(message);
    }

    protected abstract Task<int> RunAsync(InvocationContext context);
}
