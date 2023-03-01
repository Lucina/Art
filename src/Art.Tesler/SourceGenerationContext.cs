﻿using System.Text.Json;
using System.Text.Json.Serialization;

namespace Art.Tesler;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(ulong))]
[JsonSerializable(typeof(long))]
[JsonSerializable(typeof(double))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(string[]))]
[JsonSerializable(typeof(List<string>))]
[JsonSerializable(typeof(List<ArtifactToolProfile>))]
[JsonSerializable(typeof(JsonElement))]
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
