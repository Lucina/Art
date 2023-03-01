using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.IO;

namespace Art.Tesler;

public abstract class CommandBase : Command
{
    protected CommandBase(string name, string? description = null) : base(name, description)
    {
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
            PrintExceptionMessage(e, context.Console);
            return -1;
        }
    }

    protected static void PrintExceptionMessage(Exception e, IConsole console)
    {
        PrintErrorMessage(e.Message, console);
    }

    protected static void PrintErrorMessage(string message, IConsole console)
    {
        console.Error.WriteLine(message);
    }

    protected static void PrintWarningMessage(string message, IConsole console)
    {
        console.Error.WriteLine(message);
    }

    protected abstract Task<int> RunAsync(InvocationContext context);
}
