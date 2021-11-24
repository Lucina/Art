using System.Text.Json;
using System.Text.Json.Serialization;

namespace Art;

internal static class ArtJsonOptions
{
    static ArtJsonOptions()
    {
        s_jsonOptions = new() { PropertyNameCaseInsensitive = true };
        s_jsonOptions.Converters.Add(new JsonStringEnumConverter());
    }

    internal static readonly JsonSerializerOptions s_jsonOptions;
}
