using System.CommandLine;
using System.Text;

namespace Art.Tesler;

public static class CommandHelper
{
    public static string GetOptionAlias(Option option)
    {
        return option.Aliases.FirstOrDefault() ?? option.Name;
    }

    public static string GetOptionAliasList(IEnumerable<Option> options, string separator = ", ")
    {
        return new StringBuilder()
            .AppendJoin(separator, options.Select(v => v.Aliases.FirstOrDefault() ?? v.Name))
            .ToString();
    }
}
