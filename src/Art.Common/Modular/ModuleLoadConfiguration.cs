using System.Collections.Immutable;
using System.Runtime.Loader;

namespace Art.Common.Modular;

/// <summary>
/// Represents a configuration for loading on a <see cref="ModuleManifestProvider"/>.
/// </summary>
/// <param name="PassthroughAssemblies">Assemblies to pass through to default <see cref="AssemblyLoadContext"/>.</param>
public record ModuleLoadConfiguration(ImmutableHashSet<string> PassthroughAssemblies);
