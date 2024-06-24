using System.Text.Json;

namespace Art.Tesler;

public record struct ConfigProperty(ConfigScope ConfigScope, string Key, JsonElement Value);