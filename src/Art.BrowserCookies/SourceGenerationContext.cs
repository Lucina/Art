using System.Text.Json.Serialization;

namespace Art.BrowserCookies;

[JsonSerializable(typeof(Chromium.ChromiumWindowsLocalState))]
[JsonSerializable(typeof(Chromium.ChromiumPreferences))]
internal partial class SourceGenerationContext : JsonSerializerContext
{
}
