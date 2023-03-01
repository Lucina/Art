using System.CommandLine;

namespace Art.Tesler.Database;

public class DatabaseCommand : Command
{
    public DatabaseCommand(ITeslerRegistrationProvider registrationProvider) : this(registrationProvider, "db", "Perform operations on database.")
    {
    }

    public DatabaseCommand(ITeslerRegistrationProvider registrationProvider, string name, string? description = null) : base(name, description)
    {
        AddCommand(new DatabaseCommandList(registrationProvider, "list", "List archives in database."));
        AddCommand(new DatabaseCommandDelete(registrationProvider, "delete", "Delete archives in database."));
    }
}
