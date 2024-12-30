using System.CommandLine.Invocation;

namespace Art.Tesler.Database;

public class DatabaseCommandCleanup : DatabaseCommandBase
{
    public DatabaseCommandCleanup(
        IOutputControl toolOutput,
        ITeslerRegistrationProvider registrationProvider,
        string name,
        string? description = null)
        : base(toolOutput, registrationProvider, name, description)
    {
    }

    protected override async Task<int> RunAsync(InvocationContext context)
    {
        using var arm = RegistrationProvider.CreateArtifactRegistrationManager(context);
        if (arm is not IArtifactRegistrationManagerCleanup armCleanup)
        {
            ToolOutput.Error.WriteLine($"Artifact registration provider {arm} does not support cleanup.");
            return 5;
        }
        await armCleanup.CleanupDatabaseAsync();
        return 0;
    }
}
