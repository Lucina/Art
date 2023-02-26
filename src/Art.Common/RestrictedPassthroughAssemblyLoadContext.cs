using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.Loader;

namespace Art.Modular;

/// <summary>
/// Provides an <see cref="AssemblyLoadContext"/> that restricts passthrough of assemblies to a specified set.
/// </summary>
[RequiresUnreferencedCode("Loading artifact tools might require types that cannot be statically analyzed.")]
public class RestrictedPassthroughAssemblyLoadContext : AssemblyLoadContext
{
    // Need to share the core library so everyone uses the same Assembly instance and interface types from that instance
    private static readonly ImmutableHashSet<string> s_shared = new HashSet<string> { "Art" }.ToImmutableHashSet();

    /// <summary>
    /// Base path of resolver.
    /// </summary>
    public readonly string BasePath;

    private readonly ImmutableHashSet<string> _sharedAssemblies;
    private readonly AssemblyDependencyResolver _resolver;

    /// <summary>
    /// Initializes an instance of <see cref="RestrictedPassthroughAssemblyLoadContext"/>.
    /// </summary>
    /// <param name="basePath">Base path.</param>
    /// <param name="assembly">Assembly simple name.</param>
    /// <param name="sharedAssemblies">Shared assembly simple names that must be resolved by fallback contexts.</param>
    public RestrictedPassthroughAssemblyLoadContext(string basePath, string assembly, ImmutableHashSet<string> sharedAssemblies)
    {
        BasePath = basePath;
        _resolver = new AssemblyDependencyResolver(Path.Combine(basePath, assembly + ".dll"));
        _sharedAssemblies = sharedAssemblies;
    }

    /// <inheritdoc />
    protected override Assembly? Load(AssemblyName assemblyName)
    {
        string name = assemblyName.Name!;
        if (_sharedAssemblies.Contains(name))
        {
            return null;
        }
        string? asmPath = _resolver.ResolveAssemblyToPath(assemblyName);
        if (asmPath != null)
        {
            return LoadFromAssemblyPath(asmPath);
        }
        return null;
    }

    /// <inheritdoc />
    protected override nint LoadUnmanagedDll(string unmanagedDllName)
    {
        string? libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
        if (libraryPath != null)
        {
            return LoadUnmanagedDllFromPath(libraryPath);
        }
        return nint.Zero;
    }
}
