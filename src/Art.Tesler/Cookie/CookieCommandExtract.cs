using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Net;
using System.Text;
using Art.BrowserCookies;

namespace Art.Tesler.Cookie;

public class CookieCommandExtract : CommandBase
{
    protected Option<string> BrowserOption;
    protected Option<string> BrowserProfileOption;
    protected Option<List<string>> DomainsOption;
    protected Option<string> OutputOption;
    protected Option<bool> NoSubdomainsOption;

    public CookieCommandExtract(IOutputPair toolOutput, string name, string? description = null) : base(toolOutput, name, description)
    {
        BrowserOption = new Option<string>(new[] { "-b", "--browser" }, "Browser name") { ArgumentHelpName = "name", IsRequired = true };
        AddOption(BrowserOption);
        BrowserProfileOption = new Option<string>(new[] { "-p", "--profile" }, "Browser profile") { ArgumentHelpName = "name" };
        AddOption(BrowserProfileOption);
        DomainsOption = new Option<List<string>>(new[] { "-d", "--domain" }, "Domain(s) to filter by") { ArgumentHelpName = "domain", IsRequired = true, Arity = ArgumentArity.OneOrMore };
        AddOption(DomainsOption);
        OutputOption = new Option<string>(new[] { "-o", "--output" }, "Output filename") { ArgumentHelpName = "file" };
        AddOption(OutputOption);
        NoSubdomainsOption = new Option<bool>(new[] { "--no-subdomains" }, "Do not include subdomains");
        AddOption(NoSubdomainsOption);
    }

    protected override async Task<int> RunAsync(InvocationContext context)
    {
        string browserName = context.ParseResult.GetValueForOption(BrowserOption)!;
        string? browserProfile = context.ParseResult.HasOption(BrowserProfileOption) ? context.ParseResult.GetValueForOption(BrowserProfileOption) : null;
        List<string> domains = context.ParseResult.GetValueForOption(DomainsOption)!;
        bool includeSubdomains = !context.ParseResult.GetValueForOption(NoSubdomainsOption);
        if (!CookieSource.TryGetBrowserFromName(browserName, out var source, browserProfile))
        {
            PrintErrorMessage(Common.GetInvalidCookieSourceBrowserMessage(browserName), ToolOutput);
            return 2;
        }
        if (context.ParseResult.HasOption(OutputOption))
        {
            await using var output = File.CreateText(context.ParseResult.GetValueForOption(OutputOption)!);
            await ExportAsync(source, domains, includeSubdomains, output);
        }
        else
        {
            await ExportAsync(source, domains, includeSubdomains, ToolOutput.Out);
        }
        return 0;
    }

    private static async Task ExportAsync(CookieSource source, IEnumerable<string> domains, bool includeSubdomains, TextWriter output)
    {
        CookieContainer cc = new();
        await source.LoadCookiesAsync(cc, domains.Select(v => new CookieFilter(v, includeSubdomains)).ToList());
        output.Write("# Netscape HTTP Cookie File\n");
        StringBuilder sb = new();
        foreach (object? cookie in cc.GetAllCookies())
        {
            System.Net.Cookie c = (System.Net.Cookie)cookie;
            sb.Append(c.Domain).Append('\t');
            sb.Append(c.Domain.StartsWith('.') ? "TRUE" : "FALSE").Append('\t');
            sb.Append(c.Path).Append('\t');
            sb.Append(c.Secure ? "TRUE" : "FALSE").Append('\t');
            sb.Append(c.Expires == DateTime.MinValue ? 0 : (long)c.Expires.Subtract(DateTime.UnixEpoch).TotalSeconds).Append('\t');
            sb.Append(c.Name).Append('\t');
            sb.Append(c.Value).Append('\n');
            output.Write(sb.ToString());
            sb.Clear();
        }
    }
}
