using System.CommandLine;

namespace Art.Tesler.Database;

public class DatabaseCommand : Command
{
    public DatabaseCommand(
        IOutputControl toolOutput,
        ITeslerRegistrationProvider registrationProvider)
        : this(toolOutput, registrationProvider, "db", "Perform operations on database.")
    {
    }

    public DatabaseCommand(
        IOutputControl toolOutput,
        ITeslerRegistrationProvider registrationProvider,
        string name,
        string? description = null)
        : base(name, description)
    {
        AddCommand(new DatabaseCommandList(toolOutput, registrationProvider, "list", "List archives in database."));
        AddCommand(new DatabaseCommandDelete(toolOutput, registrationProvider, "delete", "Delete archives in database."));
    }
}
