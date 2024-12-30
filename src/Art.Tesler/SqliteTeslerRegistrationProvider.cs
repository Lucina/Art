using System.CommandLine;
using System.CommandLine.Invocation;
using Art.EF.Sqlite;

namespace Art.Tesler;

public class SqliteTeslerRegistrationProvider : ITeslerRegistrationProvider
{
    protected Option<string> DatabaseOption;

    public SqliteTeslerRegistrationProvider()
    {
        DatabaseOption = new Option<string>(new[] { "-d", "--database" }, "Sqlite database file") { ArgumentHelpName = "file" };
        DatabaseOption.SetDefaultValue(Common.DefaultDbFile);
    }

    public SqliteTeslerRegistrationProvider(Option<string> databaseOption)
    {
        DatabaseOption = databaseOption;
    }

    public void Initialize(Command command)
    {
        command.AddOption(DatabaseOption);
    }

    public Type GetArtifactRegistrationManagerType() => typeof(SqliteArtifactRegistrationManager);

    public IArtifactRegistrationManager CreateArtifactRegistrationManager(InvocationContext context)
    {
        return new SqliteArtifactRegistrationManager(context.ParseResult.GetValueForOption(DatabaseOption)!);
    }
}
