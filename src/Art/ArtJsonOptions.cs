using System.Text.Json;

namespace Art;

internal static class ArtJsonOptions
{
    internal static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
}
