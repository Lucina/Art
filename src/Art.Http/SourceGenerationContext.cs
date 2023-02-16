using System.Text.Json.Serialization;

namespace Art.Http;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(string[]))]
internal partial class SourceGenerationContext : JsonSerializerContext
{
}
