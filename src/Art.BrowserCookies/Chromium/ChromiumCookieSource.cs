using System.Net;
using System.Text.Json.Serialization;
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
        IChromiumKeychain? keychain = null;
        try
        {
            // NT epoch
            DateTime expiryBase = new(1601, 1, 1);
            string temp = Path.GetTempFileName();
            try
            {
                File.Copy(sourceFileName: GetPath(UserDataKind.Cookies), destFileName: temp, overwrite: true);
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
                await using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
                while (reader.Read())
                {
                    string value = reader.GetString(1);
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        value = (keychain ??= await GetKeychainAsync(cancellationToken).ConfigureAwait(false)).Unlock(ReadBytes(reader.GetStream(2)));
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
    protected abstract Task<IChromiumKeychain> GetKeychainAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the primary user data directory.
    /// </summary>
    /// <returns>Retrieved path.</returns>
    protected abstract string GetUserDataDirectory();

    /// <summary>
    /// Gets a path with the current configuration.
    /// </summary>
    /// <param name="kind">Kind of path to get.</param>
    /// <returns>Retrieved path.</returns>
    protected abstract string GetPath(UserDataKind kind);

    /// <summary>
    /// Represents Chromium preferences file.
    /// </summary>
    /// <param name="Profile">Profile.</param>
    protected record ChromiumPreferences([property: JsonPropertyName("profile")] ChromiumPreferencesProfile Profile);

    /// <summary>
    /// Represents Chromium preferences profile.
    /// </summary>
    /// <param name="Name">Profile name.</param>
    protected record ChromiumPreferencesProfile([property: JsonPropertyName("name")] string Name);

    /// <summary>
    /// Kind of user data.
    /// </summary>
    protected enum UserDataKind
    {
        /// <summary>
        /// Cookie file.
        /// </summary>
        Cookies,

        /// <summary>
        /// Preferences file.
        /// </summary>
        Preferences
    }
}
