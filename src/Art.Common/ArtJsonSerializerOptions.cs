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
        s_jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, WriteIndented = true };
        s_jsonOptions.Converters.Add(new JsonStringEnumConverter());
        s_context = new SourceGenerationContext(s_jsonOptions);
    }

    internal static readonly JsonSerializerOptions s_jsonOptions;

    internal static readonly SourceGenerationContext s_context;

    /// <summary>
    /// Creates default JSON serializer options.
    /// </summary>
    /// <returns>New, preconfigured configured instance.</returns>
    public static JsonSerializerOptions CreateDefault() => new(s_jsonOptions);
}
