namespace Art.Common.Modular;

/// <summary>
/// Specifies content for a <see cref="ModuleManifestProvider"/> module.
/// </summary>
/// <param name="Assembly">Target assembly simple name.</param>
/// <param name="Path">Sub-directory of manifest location to use to locate assemblies.</param>
public record ModuleManifestContent(string Assembly, string? Path);
