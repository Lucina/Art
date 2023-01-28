using System.Net;
using Microsoft.Data.Sqlite;

namespace Art.BrowserCookies.Chromium;

/// <summary>
/// Represents a <see cref="CookieSource"/> for a Chromium-based browser.
/// </summary>
public abstract record ChromiumCookieSource : CookieSource
{
    /// <inheritdoc />
    public override async Task LoadCookiesAsync(CookieContainer cookieContainer, string domain, CancellationToken cancellationToken = default)
    {
        if (domain.StartsWith('.'))
        {
            throw new ArgumentException("domain shouldn't start with leading \".\"");
        }
        IKeychain? keychain = null;
        try
        {
            // NT epoch
            DateTime expiryBase = new(1601, 1, 1);
            string temp = Path.GetTempFileName();
            try
            {
                File.Copy(sourceFileName: GetCookieFilePath(), destFileName: temp, overwrite: true);
                await using var connection = new SqliteConnection($"Data Source={temp};Pooling=False;");
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = """
                        SELECT name, "value", encrypted_value, path, expires_utc, is_secure, host_key
                        FROM cookies
                        WHERE host_key = $hostKey OR host_key = $dotHostKey
                        """;
                command.Parameters.AddWithValue("$hostKey", domain);
                command.Parameters.AddWithValue("$dotHostKey", "." + domain);
                await using var reader = await command.ExecuteReaderAsync(cancellationToken);
                while (reader.Read())
                {
                    string value = reader.GetString(1);
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        value = (keychain ??= await GetKeychainAsync(cancellationToken)).Unlock(ReadBytes(reader.GetStream(2)));
                    }
                    cookieContainer.Add(new Cookie
                    {
                        Expires = expiryBase.AddMicroseconds(reader.GetInt64(4)),
                        Secure = reader.GetBoolean(5),
                        Name = reader.GetString(0),
                        Value = value,
                        Path = reader.GetString(3),
                        Domain = reader.GetString(6)
                    });
                }
            }
            finally
            {
                File.Delete(temp);
            }
        }
        finally
        {
            keychain?.Dispose();
        }
    }

    private static byte[] ReadBytes(Stream stream)
    {
        byte[] buf = new byte[stream.Length];
        MemoryStream ms = new(buf);
        stream.CopyTo(ms);
        return buf;
    }

    /// <summary>
    /// Gets a keychain accessor corresponding to this browser for the current user.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task returning a keychain.</returns>
    public abstract Task<IKeychain> GetKeychainAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the path to the Chromium cookie file.
    /// </summary>
    /// <returns>Path to Chromium cookie file.</returns>
    public abstract string GetCookieFilePath();
}
