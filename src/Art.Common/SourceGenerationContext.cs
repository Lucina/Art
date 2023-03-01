using System.Text.Json;
using System.Text.Json.Serialization;
using Art.Common.Modular;

namespace Art.Common;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(bool))]
[JsonSerializable(typeof(ulong))]
[JsonSerializable(typeof(long))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(string[]))]
[JsonSerializable(typeof(JsonElement))]
[JsonSerializable(typeof(ArtifactToolProfile))]
[JsonSerializable(typeof(ArtifactToolProfile[]))]
[JsonSerializable(typeof(ArtifactInfo))]
[JsonSerializable(typeof(ArtifactResourceInfo))]
[JsonSerializable(typeof(ModuleManifestContent))]
internal partial class SourceGenerationContext : JsonSerializerContext
{
    static SourceGenerationContext()
    {
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, WriteIndented = true, AllowTrailingCommas = true, ReadCommentHandling = JsonCommentHandling.Skip };
        options.Converters.Add(new JsonStringEnumConverter());
        s_context = new SourceGenerationContext(options);
    }

    internal static readonly SourceGenerationContext s_context;
}
