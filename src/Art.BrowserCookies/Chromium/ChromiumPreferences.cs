using System.Text.Json.Serialization;

namespace Art.BrowserCookies.Chromium;

/// <summary>
/// Represents Chromium preferences file.
/// </summary>
/// <param name="Profile">Profile.</param>
public record ChromiumPreferences([property: JsonPropertyName("profile")] ChromiumPreferencesProfile Profile);

/// <summary>
/// Represents Chromium preferences profile.
/// </summary>
/// <param name="Name">Profile name.</param>
public record ChromiumPreferencesProfile([property: JsonPropertyName("name")] string Name);
