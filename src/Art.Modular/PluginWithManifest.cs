using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.Loader;

namespace Art.Modular;

[RequiresUnreferencedCode("Loading artifact tools might require types that cannot be statically analyzed.")]
public class PluginWithManifest : Plugin
{
    public ModuleManifest Manifest { get; }

    public PluginWithManifest(ModuleManifest manifest, AssemblyLoadContext context, Assembly baseAssembly) : base(context, baseAssembly)
    {
        Manifest = manifest;
    }
}
