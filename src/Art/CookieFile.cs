using System.Net;

namespace Art;

/// <summary>
/// Utility to work with cookie files.
/// </summary>
public static class CookieFile
{
    /// <summary>
    /// Loads a cookie file into a cookie container.
    /// </summary>
    /// <param name="cc">Cookie container to operate on.</param>
    /// <param name="tr">Source text of cookie file.</param>
    /// <exception cref="InvalidDataException">Thrown if cookie file was in an unexpected format.</exception>
    public static void LoadCookieFile(this CookieContainer cc, TextReader tr)
    {
        int i = 0;
        while (tr.ReadLine() is { } line)
        {
            i++;
            string[]? elem = line.Split(new char[] { ' ', '\t' }, options: StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (elem.Length == 0 || elem[0].StartsWith('#')) continue;
            if (elem.Length < 6 || elem.Length > 7) throw new InvalidDataException($"Line {i} had invalid number of elements {elem.Length}");
            string domain = elem[0];
            //bool access = elem[1].Equals("true", StringComparison.InvariantCultureIgnoreCase);
            string path = elem[2];
            bool secure = elem[3].Equals("true", StringComparison.InvariantCultureIgnoreCase);
            long expiration = long.Parse(elem[4]);
            string name = elem[5];
            string? value = elem.Length < 7 ? null : elem[6];
            cc.Add(new Cookie()
            {
                Expires = DateTime.UnixEpoch.AddSeconds(expiration),
                Secure = secure,
                Name = name,
                Value = value,
                Path = path,
                Domain = domain
            });

        }
    }
}
