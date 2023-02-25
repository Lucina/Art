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
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, WriteIndented = true };
        options.Converters.Add(new JsonStringEnumConverter());
        s_context = new SourceGenerationContext(options);
    }

    internal static readonly SourceGenerationContext s_context;

    /// <summary>
    /// Creates default JSON serializer options.
    /// </summary>
    /// <returns>New, preconfigured configured instance.</returns>
    public static JsonSerializerOptions CreateDefault() => new(s_context.Options);
}
