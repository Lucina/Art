using System.Text.Json.Serialization;

namespace Art.BrowserCookies.Chromium;

internal record ChromiumWindowsLocalState([property: JsonPropertyName("os_crypt")] ChromiumWindowsLocalStateOsCrypt OsCrypt);

internal record ChromiumWindowsLocalStateOsCrypt([property: JsonPropertyName("encrypted_key")] string EncryptedKey);
