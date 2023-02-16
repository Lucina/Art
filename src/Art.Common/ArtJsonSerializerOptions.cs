using System.Text.Json;
using System.Text.Json.Serialization;

namespace Art.Common;

/// <summary>
/// Provides defaults for JSON serialization for artifact and profile storage.
/// </summary>
public static class ArtJsonSerializerOptions
{
    static ArtJsonSerializerOptions()
    {
        s_jsonOptions = SourceGenerationContext.Default.Options;
        s_jsonOptions.PropertyNameCaseInsensitive = true;
        s_jsonOptions.WriteIndented = true;
        s_jsonOptions.Converters.Add(new JsonStringEnumConverter());
    }

    internal static readonly JsonSerializerOptions s_jsonOptions;

    /// <summary>
    /// Creates default JSON serializer options.
    /// </summary>
    /// <returns>New, preconfigured configured instance.</returns>
    public static JsonSerializerOptions CreateDefault() => new(s_jsonOptions);
}
